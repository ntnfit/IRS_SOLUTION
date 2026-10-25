using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class PickAndPackRepository : Repository<DataTable>, IPickAndPacks, IRepository<DataTable>, IDisposable
{
	private readonly IUnitOfWork _unitOfWork;

	public PickAndPackRepository(Apzon.Services.IUnitOfWork serUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public HttpResult Save(DataSet ds, string sessionId)
	{
		try
		{
			DataTable dtLine = ds.Tables[2];
			HttpResult httpResult = _unitOfWork.UDOs.Save(ds, sessionId);
			if (!string.IsNullOrEmpty(httpResult.Message) || httpResult.Content == null)
			{
				return httpResult;
			}
			int docEntry = Function.ParseInt(httpResult.Content);
			string text = UpdatePackNo(dtLine, docEntry);
			if (!string.IsNullOrEmpty(text))
			{
				return new HttpResult(Enums.MessageCode.Error, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Success);
	}

	public HttpResult Update(DataTable udoTable, DataTable dtFather, DataSet dsChild, string sessionId)
	{
		try
		{
			string text = "";
			int num = Function.ParseInt(dtFather.Rows[0]["DocEntry"]);
			string text2 = "";
			text2 = ((num <= 0) ? _unitOfWork.UDOs.MappingXml(udoTable, dtFather, dsChild, sessionId, "Add") : _unitOfWork.UDOs.MappingXml(udoTable, dtFather, dsChild, sessionId, "Update"));
			text = text + "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n                                    <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                      <env:Header>\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                      </env:Header>\r\n                                      <env:Body>";
			text += text2;
			if (num == 0)
			{
				text += XmlUpdatePackNo(dsChild.Tables[0], num);
			}
			text += "</env:Body></env:Envelope>";
			XmlElement xmlElement = DataService.Process(text).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("DocEntry"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("DocEntry"));
				if (xElement3 != null)
				{
					int num2 = Function.ParseInt(xElement3.Value);
					return new HttpResult(Enums.MessageCode.Success, "", num2);
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

	private string XmlUpdatePackNo(DataTable dtLine, int docEntry)
	{
		string text = "";
		try
		{
			if (dtLine.IsNotNull() && dtLine.Rows[0].IsExistsColumn("U_DistNumber"))
			{
				for (int i = 0; i < dtLine.Rows.Count; i++)
				{
					string text2 = Function.ToString(dtLine.Rows[i]["U_DistNumber"]);
					if (!string.IsNullOrEmpty(text2))
					{
						string sql = string.Format("UPDATE dbo.OBTN SET U_PackNo = (SELECT MAX(DocEntry) FROM [@OPKL]) WHERE DistNumber = '{1}'", docEntry, text2);
						text += _unitOfWork.UDOs.GetXmlExecuteSQL(sql, "", isGetBody: true);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	private string UpdatePackNo(DataTable dtLine, int docEntry)
	{
		string text = "";
		try
		{
			if (dtLine.IsNotNull())
			{
				string text2 = "";
				for (int i = 0; i < dtLine.Rows.Count; i++)
				{
					string text3 = Function.ToString(dtLine.Rows[i]["U_DistNumber"]);
					if (!string.IsNullOrEmpty(text3))
					{
						text2 = text2 + text3 + ",";
					}
				}
				if (!string.IsNullOrEmpty(text2))
				{
					text2 = text2.Substring(0, text2.Length - 1);
				}
				text = $"UPDATE OBTN SET \"U_PackNo\" = {docEntry} WHERE \"DistNumber\" IN (SELECT data FROM Split('{text2}',','));";
				if (ExecuteNonQuery(text, CommandType.Text) <= 0)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Update Pack No false!");
					return "Update Pack No false!";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public void Dispose()
	{
	}
}
