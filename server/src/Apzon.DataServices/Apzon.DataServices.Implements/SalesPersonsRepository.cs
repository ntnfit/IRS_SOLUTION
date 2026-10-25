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
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class SalesPersonsRepository : Repository<DataTable>, ISalesPersons, IRepository<DataTable>, IDisposable
{
	public SalesPersonsRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public virtual string Save(string sessionId, DataTable dt)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return "SalesPersonsRepository->Save(): No rows to save";
			}
			string text = "";
			List<bool> list = new List<bool>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (Exist(Function.ParseInt(dt.Rows[i]["SlpCode"]), sessionId))
				{
					text += Mapping(dt.Rows[i], sessionId, EnumCommandType.Update);
					list.Add(item: true);
				}
				else
				{
					text += Mapping(dt.Rows[i], sessionId, EnumCommandType.Add);
					list.Add(item: false);
				}
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelope>";
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
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey") && xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey")) != null)
			{
				XElement[] array = (from t in xElement.Descendants()
					where t.Name.LocalName.Equals("RetKey")
					select t).ToArray();
				for (int num = 0; num < array.Length; num++)
				{
					int num2 = Function.ParseInt(array[num].Value);
					UpdateAdditionalFields(num2, dt.Rows[num]);
					int empID = Function.ParseInt(dt.Rows[num]["empID1"]);
					if (list[num])
					{
						UpdateEmployee(num2, empID, GetOldEmpID(Function.ParseInt(num2)));
						Logging.Write("WATCH", $"Update sales employee - {num2}");
					}
					else
					{
						UpdateEmployee(num2, empID);
						Logging.Write("WATCH", $"Add sales employee - {num2}");
					}
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual HttpResult AddDoc(string sessionId, DataTable dt)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.DataIsEmpty, "SalesPersonsRepository->Save(): No rows to save");
			}
			string text = Mapping(dt.Rows[0], sessionId, EnumCommandType.Add);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelope>";
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
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey") && xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey")) != null)
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					UpdateAdditionalFields(num, dt.Rows[0]);
					int empID = Function.ParseInt(dt.Rows[0]["EmpID"]);
					UpdateEmployee(num, empID);
					Logging.Write("WATCH", $"Add sales employee - {num}");
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected string UpdateEmployee(int slpCode, int empID, int oldEmpID = 0)
	{
		try
		{
			if (empID == 0)
			{
				if (oldEmpID > 0)
				{
					string query = "UPDATE OHEM SET \"salesPrson\"=null WHERE \"empID\"=@empID;";
					List<SqlParameter> list = new List<SqlParameter>();
					list.Add(new SqlParameter("@empID", oldEmpID));
					if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) <= 0)
					{
						return "Update Employee - No row updated";
					}
				}
			}
			else
			{
				string query2 = "UPDATE OHEM SET \"salesPrson\"=@salesPrson WHERE \"empID\"=@empID;";
				List<SqlParameter> list2 = new List<SqlParameter>();
				list2.Add(new SqlParameter("@empID", empID));
				list2.Add(new SqlParameter("@salesPrson", slpCode));
				if (ExecuteNonQuery(query2, CommandType.Text, list2.ToArray()) <= 0)
				{
					return "Update Employee - No row updated";
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected void UpdateAdditionalFields(int slpCode, DataRow row)
	{
		try
		{
			string query = "update OSLP set \"Telephone\" = @tel, \"Mobil\" = @mob, \"Fax\" = @fax, \"Email\" = @ema where \"SlpCode\" = @slpCode;";
			SqlParameter[] parameters = new SqlParameter[5]
			{
				new SqlParameter("@slpCode", slpCode),
				new SqlParameter("@tel", Function.ToString(row["Telephone"])),
				new SqlParameter("@mob", Function.ToString(row["Mobil"])),
				new SqlParameter("@fax", Function.ToString(row["Fax"])),
				new SqlParameter("@ema", Function.ToString(row["Email"]))
			};
			ExecuteNonQuery(query, CommandType.Text, parameters);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private int GetOldEmpID(int slpCode)
	{
		try
		{
			string query = "Select empID From OHEM Where salesPrson=@slpCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@slpCode", slpCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	protected virtual string Mapping(DataRow row, string sessionId, EnumCommandType commandType)
	{
		string text = "";
		try
		{
			string text2 = "";
			switch (commandType)
			{
			case EnumCommandType.Update:
				text2 = "UpdateObject";
				break;
			case EnumCommandType.Add:
				text2 = "AddObject";
				break;
			}
			text = text + "<dis:" + text2 + " xmlns:dis=\"http://www.sap.com/SBO/DIS\">";
			text += "<BOM>\r\n                          <BO>\r\n                          <AdmInfo>\r\n                            <Object>oSalesPersons</Object>\r\n                          </AdmInfo>";
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><SalesEmployeeCode>" + Function.ParseInt(row["SlpCode"]) + "</SalesEmployeeCode></QueryParams>";
			}
			text += " <SalesPersons>\r\n                            <row>";
			text = text + "<SalesEmployeeName>" + Function.RawText(row["SlpName"]) + "</SalesEmployeeName>";
			text = text + "<Remarks>" + Function.ToString(row["Memo"]) + "</Remarks>";
			text = text + "<CommissionForSalesEmployee>" + Function.ParseDouble(row["Commission"]) + "</CommissionForSalesEmployee>";
			if (Function.ParseInt(row["GroupCode"]) > 0)
			{
				text = text + "<CommissionGroup>" + Function.ParseInt(row["GroupCode"]) + "</CommissionGroup>";
			}
			text = ((!(Function.ToString(row["Active"]) == "Y")) ? (text + "<Active>tNO</Active>") : (text + "<Active>tYES</Active>"));
			text += DataService.MappingUdf(row);
			text += "</row></SalesPersons>";
			text = text + "</BO></BOM></dis:" + text2 + ">";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public virtual string Remove(string sessionId, int slpCode)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                        <env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                            <BOM>\r\n                                <BO>\r\n                                 <AdmInfo>\r\n                                  <Object>oSalesPersons</Object>\r\n                                 </AdmInfo>\r\n                                 <QueryParams>\r\n                                  <SalesEmployeeCode>" + slpCode + "</SalesEmployeeCode>\r\n                                 </QueryParams>\r\n                                </BO>\r\n                               </BOM>\r\n                            </dis:RemoveObject></env:Body></env:Envelopes>";
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
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Remove sales employee - {num}");
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected bool Exist(int slpCode, string sessionId)
	{
		try
		{
			string text = ExecuteScarlar("select 1 from OSLP where \"SlpCode\" = @slpCode;", CommandType.Text, new SqlParameter("@slpCode", slpCode));
			if (!string.IsNullOrEmpty(text) && text.Equals("1"))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
		return false;
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
