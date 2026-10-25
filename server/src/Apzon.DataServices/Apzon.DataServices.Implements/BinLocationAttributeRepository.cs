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

public class BinLocationAttributeRepository : Repository<DataTable>, IBinLocationAttribute, IDisposable
{
	public BinLocationAttributeRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual string Save(string sessionId, DataTable doc)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BinLocationAttributesService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>" + Mapping(sessionId, doc) + "</soap:Body>  \r\n                            </soap:Envelope>";
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
					Logging.Write("WATCH", $"Bin Location Attribute - {num}");
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
				if (!string.IsNullOrEmpty(Function.ToString(row["Type"])))
				{
					string text3 = ((Function.RawText(row["Type"]) == "A") ? "Add" : "Update");
					text2 = text2 + "<" + text3 + " xmlsn = \"BinLocationAttributesService\"><BinLocationAttribute>";
					if (text3 == "Update")
					{
						text2 = text2 + "<AbsEntry>" + Function.RawText(row["AbsEntry"]) + "</AbsEntry>";
					}
					text2 = text2 + "<Attribute>" + Function.RawText(row["FldAbs"]) + "</Attribute>";
					text2 = text2 + "<Code>" + Function.RawText(row["AttrValue"]) + "</Code>";
					text2 = text2 + "</BinLocationAttribute></" + text3 + ">";
				}
				else
				{
					text2 = text2 + "<Delete xmlsn = \"BinLocationAttributesService\"><BinLocationAttribute><AbsEntry>" + Function.RawText(row["AbsEntry"]) + "</AbsEntry></BinLocationAttribute></Delete>";
				}
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

	public void Dispose()
	{
	}
}
