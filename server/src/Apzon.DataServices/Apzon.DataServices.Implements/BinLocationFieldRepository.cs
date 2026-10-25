using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class BinLocationFieldRepository : Repository<DataTable>, IBinLocationField
{
	public BinLocationFieldRepository(Apzon.Services.IUnitOfWork serviUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviUnitOfWork, dataService)
	{
	}

	public virtual string Save(string sessionId, DataTable doc)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BinLocationFieldsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>" + Mapping(sessionId, doc) + "</soap:Body>  \r\n                            </soap:Envelope>";
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
					Logging.Write("WATCH", $"Bin Location Field - {num}");
					return Function.ToString(num);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public virtual string Mapping(string sessionId, DataTable doc)
	{
		string text = "";
		try
		{
			foreach (DataRow row in doc.Rows)
			{
				string text2 = "";
				text2 += "<Update xmlsn = \"BinLocationFieldsService\"><BinLocationField>";
				text2 = text2 + "<AbsEntry>" + Function.RawText(row["AbsEntry"]) + "</AbsEntry>";
				text2 = text2 + "<Activated>" + Function.RawText(row["Activated"]) + "</Activated>";
				text2 = text2 + "<DefaultFieldName>" + Function.RawText(row["DftName"]) + "</DefaultFieldName>";
				text2 = text2 + "<FieldNumber>" + Function.RawText(row["FldNum"]) + "</FieldNumber>";
				text2 = text2 + "<FieldType>" + Function.RawText(row["FldType"]) + "</FieldType>";
				text2 = text2 + "<Name>" + Function.RawText(row["DispName"]) + "</Name>";
				text2 += "</BinLocationField></Update>";
				text += text2;
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}
}
