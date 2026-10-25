using System;
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

public class WarehouseSublevelRepository : Repository<DataTable>, IWarehouseSublevel, IDisposable
{
	public WarehouseSublevelRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual HttpResult Save(string sessionId, DataTable doc)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>WarehouseSublevelCodesService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>" + Mapping(sessionId, doc) + "</soap:Body>  \r\n                            </soap:Envelope>";
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
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Warehouse Sublevel - {num}");
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Success);
	}

	public virtual HttpResult SaveGen(string sessionId, DataTable data)
	{
		try
		{
			if (data == null || data.Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "No data to save");
			}
			foreach (DataRow row in data.Rows)
			{
				DataTable dataTable = data.Clone();
				dataTable.ImportRow(row);
				HttpResult httpResult = Save(sessionId, dataTable);
				row["Error"] = ((httpResult.MessageCode == Enums.MessageCode.Success) ? "" : httpResult.Message);
			}
			data.AcceptChanges();
			return new HttpResult(Enums.MessageCode.Success, "", data);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual string Mapping(string sessionId, DataTable doc)
	{
		try
		{
			string text = "";
			foreach (DataRow row in doc.Rows)
			{
				string text2 = "";
				string text3 = Function.ToString(row["Type"]);
				if (text3 == "A" || text3 == "U")
				{
					string text4 = ((text3 == "A") ? "Add" : "Update");
					text2 = text2 + "<" + text4 + " xmlsn = \"WarehouseSublevelCodesService\"><WarehouseSublevelCode>";
					if (text3 == "U")
					{
						text2 = text2 + "<AbsEntry>" + Function.RawText(row["AbsEntry"]) + "</AbsEntry>";
					}
					text2 = text2 + "<WarehouseSublevel>" + Function.RawText(row["FldAbs"]) + "</WarehouseSublevel>";
					text2 = text2 + "<Code>" + Function.RawText(row["SLCode"]).ToUpper() + "</Code>";
					text2 = text2 + "<Description>" + Function.RawText(row["Descr"]) + "</Description>";
					text2 = text2 + "</WarehouseSublevelCode></" + text4 + ">";
				}
				else
				{
					text2 = text2 + "<Delete xmlsn = \"WarehouseSublevelCodesService\"><WarehouseSublevelCode><AbsEntry>" + Function.RawText(row["AbsEntry"]) + "</AbsEntry></WarehouseSublevelCode></Delete>";
				}
				text += text2;
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public bool Exist(string slCode, int fldAbs)
	{
		try
		{
			SqlParameter[] parameters = new SqlParameter[2]
			{
				new SqlParameter("@fldAbs", fldAbs),
				new SqlParameter("@slCode", slCode)
			};
			return Function.ParseInt(ServiceUnitOfWork.Commons.ExecuteScalar("select 1 from OBSL where \"FldAbs\"=@fldAbs and \"SLCode\"=@slCode;", CommandType.Text, parameters)) == 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
