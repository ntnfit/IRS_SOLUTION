using System;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Documents;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class StockTransferDraftRepository : Repository<DocumentsImpl>, IStockTransferDraft, IRepository<DocumentsImpl>, IDisposable
{
	public StockTransferDraftRepository(Apzon.Services.IUnitOfWork serviUnitOfWork, IDataService dataService)
		: base(serviUnitOfWork, dataService)
	{
	}

	public void Dispose()
	{
	}

	public string Close(int docEntry, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                 <env:Header>\r\n                                  <SessionID>" + sessionId + "</SessionID>\r\n                                 </env:Header>\r\n                                 <env:Body>\r\n                                  <dis:CloseObject  xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                  <BOM>\r\n                                   <BO>\r\n                                    <AdmInfo>\r\n                                     <Object>" + Function.ToString(ApzObjectTypes.oStockTransferDraft) + "</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams>\r\n                                     <DocEntry>" + Function.ToString(docEntry) + "</DocEntry>\r\n                                    </QueryParams>\r\n                                   </BO>\r\n                                  </BOM>\r\n                                  </dis:CloseObject >\r\n                                 </env:Body>\r\n                                </env:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", $"Close document - {ApzObjectTypes.oStockTransferDraft} - {xElement2.Value}");
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Close document - {ApzObjectTypes.oStockTransferDraft} - DocEntry - {num}");
					return num.ToString();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}
}
