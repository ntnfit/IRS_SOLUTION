using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class StockTransferRequestRepository : StockTransferRepository, IStockTransferRequest, IRepository<DocumentsImpl>
{
	private new readonly IUnitOfWork _unitOfWork;

	public StockTransferRequestRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<HttpResult> Save(DocumentsImpl doc, bool isDraft, string sessionId)
	{
		try
		{
			if (isDraft && doc.draftKey > 0)
			{
				doc.DocEntry = doc.draftKey;
			}
			if (doc.DocEntry == 0)
			{
				if (doc.Document_ApprovalRequests != null && doc.Document_ApprovalRequests.Count > 0)
				{
					return await _unitOfWork.Documents.AddApprovalRequest(doc, sessionId, ApzObjectTypes.oInventoryTransferRequest);
				}
				if (doc.IsDraft && !isDraft)
				{
					doc.DocObjectCode = ApzObjectTypes.oInventoryTransferRequest;
					return SaveDraftToDocument(doc, sessionId);
				}
				return Add(doc, isDraft, sessionId);
			}
			return Update(doc, isDraft, sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual HttpResult Update(DocumentsImpl doc, bool isDraft, string sessionId)
	{
		try
		{
			Logging.Write("WATCH", "Start update transfer request");
			List<int> list = new List<int>();
			string text = string.Join(",", doc.Lines.Select((Document_LinesImpl t) => t.LineNum));
			if (string.IsNullOrEmpty(text))
			{
				text = "-1";
			}
			DataTable dataTable = ExecuteDataTable("SELECT * FROM WTQ1 WHERE DocEntry=@DocEntry AND LineNum NOT IN (" + text + ")", CommandType.Text, new SqlParameter("@DocEntry", doc.DocEntry));
			if (dataTable.IsNotNull())
			{
				list.AddRange(from t in dataTable.AsEnumerable()
					select Function.ParseInt(t["LineNum"]));
				if (list.Count > 0)
				{
					DeleteLines(doc, sessionId, ApzObjectTypes.oInventoryTransferRequest, list);
				}
			}
			string xmlObject = MappingXml(ApzObjectTypes.oInventoryTransferRequest, doc, isDraft, EnumCommandType.Update);
			XmlElement xmlElement = DataService.UpdateObject(ApzObjectTypes.oInventoryTransferRequest, sessionId, xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Update Transfer request" + xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", "Update Transfer request" + num);
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual HttpResult Add(DocumentsImpl doc, bool isDraft, string sessionId)
	{
		Logging.Write("WATCH", "Start add transfer request");
		string xmlObject = MappingXml(ApzObjectTypes.oInventoryTransferRequest, doc, isDraft);
		XmlElement xmlElement = DataService.AddObject(ApzObjectTypes.oInventoryTransferRequest, sessionId, xmlObject).Content as XmlElement;
		XElement xElement = XElement.Parse(xmlElement.OuterXml);
		if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
		{
			XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
			if (xElement2 != null)
			{
				Logging.Write("ERROR", "Add Transfer request" + xElement2.Value);
				return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
			}
		}
		else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
		{
			XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
			if (xElement3 != null)
			{
				int num = Function.ParseInt(xElement3.Value);
				Logging.Write("WATCH", "Add Transfer request " + num);
				return new HttpResult(Enums.MessageCode.Success, "", num);
			}
		}
		return new HttpResult();
	}

	public new string Close(int docEntry, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                 <env:Header>\r\n                                  <SessionID>" + sessionId + "</SessionID>\r\n                                 </env:Header>\r\n                                 <env:Body>\r\n                                  <dis:CloseObject  xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                  <BOM>\r\n                                   <BO>\r\n                                    <AdmInfo>\r\n                                     <Object>" + Function.ToString(ApzObjectTypes.oInventoryTransferRequest) + "</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams>\r\n                                     <DocEntry>" + Function.ToString(docEntry) + "</DocEntry>\r\n                                    </QueryParams>\r\n                                   </BO>\r\n                                  </BOM>\r\n                                  </dis:CloseObject >\r\n                                 </env:Body>\r\n                                </env:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", $"Close document - {ApzObjectTypes.oInventoryTransferRequest} - {xElement2.Value}");
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Close document - {ApzObjectTypes.oInventoryTransferRequest} - DocEntry - {num}");
					return "";
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

	public string GetTransferRequestByKey(string sessionId, ApzObjectTypes objType, string docEntry)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>InventoryTransferRequestsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetByParams xmlns=\"InventoryTransferRequestsService\">\r\n                                     <StockTransfer>   <DocEntry>" + docEntry + "</DocEntry></StockTransfer>\r\n                                    </GetByParams>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					throw new Exception(xElement2.Value);
				}
			}
			return xmlElement.OuterXml;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	protected virtual string DeleteLines(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, List<int> lstRemoved)
	{
		try
		{
			string transferRequestByKey = GetTransferRequestByKey(sessionId, objType, Function.ToString(doc.DocEntry));
			if (string.IsNullOrEmpty(transferRequestByKey))
			{
				throw new Exception("[DI-SERVER] No matching record found.");
			}
			XElement xElement = XElement.Parse(transferRequestByKey).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("StockTransfer"));
			DataSet dataSet = new DataSet();
			using (StringReader reader = new StringReader(Function.ToString(xElement)))
			{
				dataSet.ReadXml(reader);
			}
			if (dataSet.Tables.Contains("StockTransferLine"))
			{
				bool flag = false;
				DataTable dataTable = dataSet.Tables["StockTransferLine"];
				if (lstRemoved.Count > 0)
				{
					flag = true;
					foreach (int i in lstRemoved)
					{
						DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]).Equals(i));
						if (dataRow != null)
						{
							dataTable.Rows.Remove(dataRow);
						}
					}
				}
				if (!flag)
				{
					return string.Empty;
				}
				string xml = dataSet.GetXml();
				if (!string.IsNullOrEmpty(xml))
				{
					string xmlObject = "<?xml version=\"1.0\" ?>\r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>InventoryTransferRequestsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"InventoryTransferRequestsService\">" + xml + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
					XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
					XElement xElement2 = XElement.Parse(xmlElement.OuterXml);
					if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
					{
						XElement xElement3 = xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
						if (xElement3 != null)
						{
							throw new Exception(xElement3.Value);
						}
					}
					return string.Empty;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}
}
