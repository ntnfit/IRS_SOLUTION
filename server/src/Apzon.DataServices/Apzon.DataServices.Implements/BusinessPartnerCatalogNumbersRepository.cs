using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class BusinessPartnerCatalogNumbersRepository : Repository<DataTable>, IBusinessPartnerCatalogNumbers, IDisposable
{
	private readonly IUnitOfWork _unitOfWork;

	public BusinessPartnerCatalogNumbersRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public void Dispose()
	{
	}

	public virtual HttpResult Save(DataTable model, ApzObjectTypes objtype, string sessionId)
	{
		try
		{
			DataTable existed = GetExisted(model, objtype);
			string text = "";
			if (existed.IsNotNull())
			{
				DataRow[] array = (from t in existed.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["OldSub"]))
					select t).ToArray();
				if (array.Length != 0)
				{
					for (int num = 0; num < array.Length; num++)
					{
						if (!string.IsNullOrEmpty(Function.ToString(array[num]["ItemCode"])) || !string.IsNullOrEmpty(Function.ToString(array[num]["CardCode"])))
						{
							text = text + "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + Mapping(array[num], EnumCommandType.Update) + "</dis:UpdateObject>";
						}
					}
				}
				DataRow[] array2 = (from t in existed.AsEnumerable()
					where (!string.IsNullOrEmpty(Function.ToString(t["ItemCode"])) || !string.IsNullOrEmpty(Function.ToString(t["CardCode"]))) && string.IsNullOrEmpty(Function.ToString(t["OldSub"]))
					select t).ToArray();
				if (array2.Length != 0)
				{
					text += "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">";
					text += Mapping(array2);
					text += "</dis:AddObject> ";
				}
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual string Remove(string code, string subtitle, string item, ApzObjectTypes objtype, string sessionId)
	{
		try
		{
			DataTable dataByCode = GetDataByCode(code, subtitle, item, objtype);
			if (dataByCode.IsNotNull())
			{
				string text = "";
				for (int i = 0; i < dataByCode.Rows.Count; i++)
				{
					text = text + "<dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + Mapping(dataByCode.Rows[i], EnumCommandType.Remove) + "</dis:RemoveObject> ";
				}
				string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
				XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						return xElement2.Value;
					}
				}
				return string.Empty;
			}
			return Messages.D404;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	protected DataTable GetDataByCode(string code, string subtitle, string item, ApzObjectTypes objtype)
	{
		try
		{
			switch (objtype)
			{
			case ApzObjectTypes.oBusinessPartners:
			{
				SqlParameter[] parameters = new SqlParameter[3]
				{
					new SqlParameter("@Substitute", subtitle),
					new SqlParameter("@CarCode", code),
					new SqlParameter("@ItemCode", item)
				};
				return ServiceUnitOfWork.Commons.ExecuteDataTable("SELECT *,\"Substitute\" \"OldSub\" FROM OSCN WHERE \"Substitute\"=@Substitute and \"CardCode\"=@CarCode and \"ItemCode\"=@ItemCode;", CommandType.Text, parameters);
			}
			case ApzObjectTypes.oItems:
				return ServiceUnitOfWork.Commons.ExecuteDataTable("SELECT *,\"Substitute\" \"OldSub\" FROM OSCN WHERE \"ItemCode\"=@ItemCode;", CommandType.Text, new SqlParameter("@ItemCode", item));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public string RemoveItem(DataTable model, ApzObjectTypes objtype, string sessionId)
	{
		try
		{
			if (!model.IsNotNull())
			{
				return "No Data!";
			}
			for (int i = 0; i < model.Rows.Count; i++)
			{
				DataTable dataByBPItem = GetDataByBPItem(Function.ToString(model.Rows[i]["CardCode"]), Function.ToString(model.Rows[i]["ItemCode"]));
				if (!dataByBPItem.IsNotNull())
				{
					continue;
				}
				string text = "";
				for (int j = 0; j < dataByBPItem.Rows.Count; j++)
				{
					text = text + "<dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + Mapping(dataByBPItem.Rows[j], EnumCommandType.Remove) + "</dis:RemoveObject> ";
				}
				string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
				XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						return xElement2.Value;
					}
				}
				return string.Empty;
			}
			return Messages.D404;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	private DataTable GetDataByBPItem(string cardCode, string itemCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CardCode", cardCode));
			list.Add(new SqlParameter("@ItemCode", itemCode));
			return ServiceUnitOfWork.Commons.ExecuteDataTable("SELECT *,\"Substitute\" \"OldSub\" FROM OSCN WHERE \"CardCode\"=@CardCode AND \"ItemCode\" = @ItemCode;", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	protected virtual string Mapping(DataRow dataRow, EnumCommandType commandType)
	{
		string text = "<BOM><BO>\r\n                                        <AdmInfo>\r\n                                        <Object>oAlternateCatNum</Object>\r\n                                        </AdmInfo>";
		if (commandType == EnumCommandType.Update || commandType == EnumCommandType.Remove)
		{
			text = text + "<QueryParams><CardCode>" + Function.RawText(dataRow["CardCode"]) + "</CardCode>\r\n                        <ItemCode>" + Function.RawText(dataRow["ItemCode"]) + "</ItemCode>\r\n                        <Substitute>" + Function.RawText(dataRow["OldSub"]) + "</Substitute></QueryParams>";
		}
		if (commandType != EnumCommandType.Remove)
		{
			text += "<AlternateCatNum>";
			text += "<row>";
			text = text + "<ItemCode>" + Function.RawText(dataRow["ItemCode"]) + "</ItemCode>";
			text = text + "<CardCode>" + Function.RawText(dataRow["CardCode"]) + "</CardCode>";
			text = text + "<Substitute>" + Function.RawText(dataRow["Substitute"]) + "</Substitute>";
			if (!string.IsNullOrEmpty(Function.RawText(dataRow["ShowSCN"])) || commandType == EnumCommandType.Update)
			{
				text = text + "<DisplayBPCatalogNumber>" + ((Function.RawText(dataRow["ShowSCN"]) == "N") ? "tNO" : "tYES") + "</DisplayBPCatalogNumber>";
			}
			text += DataService.MappingUdf(dataRow, commandType);
			text += "</row>";
			text += "</AlternateCatNum>";
		}
		return text + "</BO></BOM>";
	}

	protected virtual string Mapping(DataRow[] dt)
	{
		string text = "<BOM><BO>\r\n                                        <AdmInfo>\r\n                                        <Object>oAlternateCatNum</Object>\r\n                                        </AdmInfo>";
		text += "<AlternateCatNum>";
		foreach (DataRow dataRow in dt)
		{
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["ItemCode"])) || !string.IsNullOrEmpty(Function.ToString(dataRow["CardCode"])))
			{
				text += "<row>";
				text = text + "<ItemCode>" + Function.RawText(dataRow["ItemCode"]) + "</ItemCode>";
				text = text + "<CardCode>" + Function.RawText(dataRow["CardCode"]) + "</CardCode>";
				text = text + "<Substitute>" + Function.RawText(dataRow["Substitute"]) + "</Substitute>";
				if (!string.IsNullOrEmpty(Function.RawText(dataRow["ShowSCN"])))
				{
					text = text + "<DisplayBPCatalogNumber>" + ((Function.RawText(dataRow["ShowSCN"]) == "N") ? "tNO" : "tYES") + "</DisplayBPCatalogNumber>";
				}
				text += DataService.MappingUdf(dataRow);
				text += "</row>";
			}
		}
		return text + "</AlternateCatNum></BO></BOM>";
	}

	protected DataTable GetExisted(DataTable model, ApzObjectTypes fatherObjectTypes)
	{
		return ServiceUnitOfWork.BusinessPartnerCatalogNumbers.GetExisted(model, fatherObjectTypes);
	}

	public string CheckExist(string code, string subtitle, string item, ApzObjectTypes objtype, string sessionId)
	{
		try
		{
			switch (objtype)
			{
			case ApzObjectTypes.oBusinessPartners:
			{
				SqlParameter[] parameters = new SqlParameter[3]
				{
					new SqlParameter("@Substitute", subtitle),
					new SqlParameter("@CarCode", code),
					new SqlParameter("@ItemCode", item)
				};
				DataTable dataTable2 = ServiceUnitOfWork.Commons.ExecuteDataTable("SELECT *,\"Substitute\" \"OldSub\" FROM OSCN WHERE \"Substitute\"=@Substitute and \"CardCode\"=@CarCode and \"ItemCode\"=@ItemCode;", CommandType.Text, parameters);
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					return "Y";
				}
				return "N";
			}
			case ApzObjectTypes.oItems:
			{
				DataTable dataTable = ServiceUnitOfWork.Commons.ExecuteDataTable("SELECT *,\"Substitute\" \"OldSub\" FROM OSCN WHERE \"ItemCode\"=@ItemCode;", CommandType.Text, new SqlParameter("@ItemCode", item));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return "Y";
				}
				return "N";
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return "N";
	}
}
