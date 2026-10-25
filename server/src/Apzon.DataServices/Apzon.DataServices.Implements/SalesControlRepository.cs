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

public class SalesControlRepository : Repository<DataTable>, ISalesControl, IRepository<DataTable>, IDisposable
{
	public SalesControlRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public string AddDocumentByBase(int docEntry, int fromObjType, int toObjType, string sessionId)
	{
		try
		{
			string text = "";
			string[] array = sessionId.Split(',');
			if (array != null && array.Length > 1)
			{
				_ = sessionId.Split(',')[0];
				text = sessionId.Split(',')[1];
			}
			else
			{
				text = sessionId;
			}
			return Execute(docEntry, fromObjType, toObjType, text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public string Execute(int docEntry, int fromObjType, int toObjType, string sessionId)
	{
		try
		{
			string xmlObject = $"<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                     <env:Header>\r\n                                                      <SessionID>{sessionId}</SessionID>\r\n                                                       </env:Header>\r\n                                                       <env:Body>\r\n                                                            <dis:GetByKey xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n\t                                                        <Object>oQuotations</Object>\r\n\t                                                        <DocEntry>{docEntry}</DocEntry>\r\n\t                                                        </dis:GetByKey>\r\n                                                        </env:Body>\r\n                                                    </env:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			string xmlObject2 = string.Format("<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                     <env:Header>\r\n                                                      <SessionID>{0}</SessionID>\r\n                                                       </env:Header>\r\n                                                       <env:Body>\r\n                                                            <dis:GetBusinessObjectTemplate xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n\t                                                        <Object>oOrders</Object>\r\n\t                                                        </dis:GetBusinessObjectTemplate>\r\n                                                        </env:Body>\r\n                                                    </env:Envelope>", sessionId, docEntry);
			XmlElement xmlElement2 = DataService.Process(xmlObject2).Content as XmlElement;
			XmlDocument xmlDocument = new XmlDocument();
			XmlDocument xmlDocument2 = new XmlDocument();
			xmlDocument.LoadXml(xElement.ToString());
			xmlDocument2.LoadXml(xmlElement2.OuterXml.ToString());
			foreach (XmlNodeList childNode in xmlDocument.ChildNodes)
			{
				_ = childNode.Count;
			}
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
					Function.ParseInt(xElement3.Value);
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

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
