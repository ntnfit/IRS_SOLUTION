using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Kit;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class DocumentsRepository : Repository<DocumentsImpl>, Apzon.DataServices.Interfaces.IDocuments, IRepository<DocumentsImpl>, IDisposable
{
	protected static Dictionary<string, string> MainCurrency = new Dictionary<string, string>();

	protected string _connectionString;

	public static Dictionary<string, List<string>> ListUdfHeaderNotValidated = new Dictionary<string, List<string>>();

	public static Dictionary<string, List<string>> ListUdfLineNotValidated = new Dictionary<string, List<string>>();

	protected readonly IUnitOfWork _unitOfWork;

	protected readonly Apzon.Services.IUnitOfWork _serviceUnitOfWork;

	public DocumentsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
		_serviceUnitOfWork = serviceUnitOfWork;
		_connectionString = serviceUnitOfWork.ConnectionString;
	}

	public async Task<HttpResult> AddApprovalRequest(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, string serverPath = "")
	{
		_ = 3;
		try
		{
			doc.AuthorizationStatus = DocumentAuthorizationStatusEnum.dasPending;
			List<Document_ApprovalRequestsImpl> approvalRequest = doc.Document_ApprovalRequests;
			doc.Document_ApprovalRequests = null;
			int docEntry = doc.draftKey;
			if (doc.draftKey > 0 && doc.DocEntry <= 0)
			{
				doc.DocEntry = doc.draftKey;
				doc.draftKey = 0;
				Logging.Write("WATCH", $"Remove document from approval process- docEntry {doc.DocEntry} - object type Draft");
				HttpResult httpResult;
				if (objType == ApzObjectTypes.oInventoryTransferRequest)
				{
					httpResult = await _unitOfWork.StockTransferRequest.Save(doc, isDraft: true, sessionId);
				}
				else
				{
					HttpResult httpResult2 = ((objType != ApzObjectTypes.oStockTransfer) ? (await UpdateData(doc, sessionId, ApzObjectTypes.oDrafts, (ApzObjectTypes)doc.ObjType)) : _unitOfWork.StockTransfer.Save(doc, sessionId, isDraft: true));
					httpResult = httpResult2;
				}
				HttpResult httpResult3 = httpResult;
				if (httpResult3.MessageCode != Enums.MessageCode.Success)
				{
					return httpResult3;
				}
				doc.DocEntry = 0;
			}
			else
			{
				HttpResult httpResult;
				if (objType == ApzObjectTypes.oInventoryTransferRequest)
				{
					httpResult = await _unitOfWork.StockTransferRequest.Save(doc, isDraft: true, sessionId);
				}
				else
				{
					HttpResult httpResult2 = ((objType != ApzObjectTypes.oStockTransfer) ? (await AddNew(doc, sessionId, ApzObjectTypes.oDrafts, (ApzObjectTypes)doc.ObjType)) : _unitOfWork.StockTransfer.Save(doc, sessionId, isDraft: true));
					httpResult = httpResult2;
				}
				HttpResult httpResult4 = httpResult;
				if (httpResult4.MessageCode != Enums.MessageCode.Success)
				{
					return httpResult4;
				}
				docEntry = Function.ParseInt(httpResult4.Content);
			}
			string query = "select \"DocNum\" from ODRF where \"DocEntry\"=" + docEntry + ";";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			string text = "";
			if (dataTable.IsNotNull())
			{
				text = Function.ToString(dataTable.Rows[0]["DocNum"]);
			}
			if (string.IsNullOrEmpty(text))
			{
				return new HttpResult(Enums.MessageCode.Error, "Could not get draft num");
			}
			foreach (var item in (from c in approvalRequest
				group c by new { c.ApprovalTemplatesID, c.ReqId, c.Remarks } into gcs
				select new
				{
					ApprovalTemplatesID = gcs.Key.ApprovalTemplatesID,
					ReqId = gcs.Key.ReqId,
					Remarks = gcs.Key.Remarks,
					Receipt = gcs.ToList()
				}).ToList())
			{
				SqlParameter[] paramater = new SqlParameter[6]
				{
					new SqlParameter("@WtmCode", Function.ParseInt(item.ApprovalTemplatesID)),
					new SqlParameter("@OwnerId", item.ReqId),
					new SqlParameter("@DocEntry", docEntry),
					new SqlParameter("@DocDate", doc.DocDate),
					new SqlParameter("@Remarks", item.Remarks),
					new SqlParameter("@UpdateEntry", doc.DocEntry)
				};
				int num = ServiceUnitOfWork.Documents.InsertDocumentApprovalRequest(paramater);
				if (num > 0)
				{
					bool flag = false;
					MailHelper mailHelper = new MailHelper();
					DataSet data = _serviceUnitOfWork.ContextAction.GetData("H$ApprovalDecision", item.ApprovalTemplatesID.ToString());
					if (data.IsNotNull() && data.Tables[0].IsNotNull())
					{
						flag = true;
						mailHelper.EmailConfig = _serviceUnitOfWork.MailSetting.GetDoc();
						mailHelper.UnlockedUsers = _serviceUnitOfWork.SubActivities.GetUnlockedUsers();
					}
					bool flag2 = false;
					foreach (Document_ApprovalRequestsImpl item2 in item.Receipt)
					{
						string message = _unitOfWork.Message.GetMessage("Request for Approving Document IRS Generation", item.Remarks, "Request for Document Approval", $"{objType} based on draft no. {text}", "122", $"{num.ToString()}", item2.ReceiptName);
						ProcessSendMessage(sessionId, message);
						flag2 = true;
						try
						{
							if (!flag)
							{
								continue;
							}
							new DataTable();
							string query2 = "apz_approval_emailcontent";
							DataTable dataTable2 = ExecuteDataTable(query2, CommandType.StoredProcedure, new SqlParameter("@WddCode", num), new SqlParameter("@WtmCode", item.ApprovalTemplatesID), new SqlParameter("@DraftKey", docEntry), new SqlParameter("@ObjType", objType), new SqlParameter("@Remarks", item.Remarks), new SqlParameter("@ReqId", item.ReqId), new SqlParameter("@ReceiptId", item2.ReceiptId));
							if (!dataTable2.IsNotNull())
							{
								continue;
							}
							ActionHandle actionHandle = new ActionHandle();
							MailImpl mailContent = actionHandle.GetMailContent(data, dataTable2);
							if (mailContent != null)
							{
								string text2 = Function.ToString(dataTable2.Rows[0]["DbName"]);
								string encryptedText = Function.ToString(dataTable2.Rows[0]["Pwd"]);
								string api = Function.ToString(dataTable2.Rows[0]["ApiAddress"]);
								Guid guid = Guid.NewGuid();
								string attPath = serverPath + "\\" + text2 + "\\Action";
								mailContent.BodyText = GetMailBodyText(text2, item2.ReceiptName, Function.Decrypt(RSA.PrivateKey, encryptedText), num, api, guid.ToString());
								string commandText = "";
								string fileName = "";
								ReportDocument crystalReport = actionHandle.GetCrystalReport(data, dataTable2, attPath, out commandText, out fileName);
								if (!string.IsNullOrEmpty(commandText))
								{
									DataTable dataSource = ExecuteDataTable(commandText, CommandType.Text);
									crystalReport.SetDataSource(dataSource);
									crystalReport.Refresh();
									mailContent.AttachStream = new Dictionary<string, Stream>();
									mailContent.AttachStream[fileName + ".pdf"] = crystalReport.ExportToStream(ExportFormatType.PortableDocFormat);
									string updateExtnQuery = GetUpdateExtnQuery();
									ExecuteNonQuery(updateExtnQuery, CommandType.Text, new SqlParameter("@Guid", guid));
									string text3 = mailHelper.SendEmail(item.ReqId, mailContent.AddressTo, mailContent.Subject, mailContent.BodyText, mailContent.AddressFrom, mailContent.AddressCc, mailContent.AddressBcc, mailContent.AttachStream);
									Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text3);
								}
							}
						}
						catch (Exception ex)
						{
							Logging.Write("ERROR", ex);
						}
					}
					if (!flag2)
					{
						Logging.Write("Could not send alert");
					}
					continue;
				}
				return new HttpResult(Enums.MessageCode.Error, "Can't insert approval request.");
			}
			return new HttpResult(Enums.MessageCode.Success, "", docEntry);
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	protected virtual string GetUpdateExtnQuery()
	{
		return "insert into APZ_EXRL(DocEntry,Token,Status) SELECT isnull((SELECT Max(DocEntry) FROM APZ_EXRL),0)+1,@Guid,'N'";
	}

	private string GetMailBodyText(string dbname, string username, string password, int wddcode, string api, string guid)
	{
		string value = Function.Encrypt(RSA.PublicKey, dbname + ";" + username + ";" + password + ";" + wddcode);
		value = WebUtility.UrlEncode(value);
		guid = WebUtility.UrlEncode(guid);
		return "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">  <html xmlns= \"http://www.w3.org/1999/xhtml\">\r\n                        <head><meta http-equiv=\"Content -Type\" content=\"text/html; charset=utf-8\"/><title></title>\r\n                        </head>   \r\n                       <body>  \r\n                        <span>\r\n                        Yêu cầu phê duyệt được gửi từ hệ thống.\r\n                        </span>\r\n                        <br/>\r\n                        Vui lòng nhấn vào đường dẫn dưới đây để thực hiện phê duyệt\r\n                        <br/>\r\n                        <a href=\"" + api + "/api/ApprovalDecision?token=" + guid + "&key=" + value + "\">" + api + "/api/ApprovalDecision?token=" + guid + "&key=" + value + "</a>\r\n                        </body>\r\n                </html> ";
	}

	protected virtual void ProcessSendMessage(string sessionId, string messageString)
	{
		string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>\r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>MessagesService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <SendMessage xmlns=\"MessagesService\">" + messageString + "</SendMessage> \r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
		new DiServices().Process(xmlObject);
	}

	public DataTable GetApprovalTemplate(DocumentsImpl doc, string sessionId, string userName, string viewMode, ApzObjectTypes type)
	{
		try
		{
			string query = "USP_GetApprovalTemplate";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@UserName", userName), new SqlParameter("@ObjType", (int)type), new SqlParameter("@DocEntry", doc.DocEntry), new SqlParameter("@ViewMode", viewMode ?? ""), new SqlParameter("@DraftKey", doc.draftKey));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public virtual async Task<HttpResult> SaveDocument(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts, bool ignoreAttach = false, bool ignoreUpdateEmptyValue = false, bool ignoreCheckexists = false, string serverpath = "")
	{
		_ = 2;
		try
		{
			bool flag = doc.DocEntry <= 0 || (!ignoreCheckexists && !Exist(doc, sessionId, objType));
			if (!ignoreAttach)
			{
				HttpResult httpResult = ServiceUnitOfWork.Files.SaveAttachmentFiles(doc.AttachmentFiles, new Dictionary<string, object> { 
				{
					"DocEntry,int",
					(!flag) ? doc.DocEntry : 0
				} }, GetTableNameByObjectType(objType));
				if (httpResult.MessageCode != Enums.MessageCode.Success)
				{
					return httpResult;
				}
				doc.AtcEntry = Function.ParseInt(httpResult.Content);
			}
			if (flag)
			{
				if (doc.Document_ApprovalRequests != null && doc.Document_ApprovalRequests.Count > 0)
				{
					return await _unitOfWork.Documents.AddApprovalRequest(doc, sessionId, objType);
				}
				if (doc.IsDraft && objType != ApzObjectTypes.oDrafts)
				{
					doc.DocObjectCode = objType;
					return SaveDraftToDocument(doc, sessionId);
				}
				return await AddNew(doc, sessionId, objType, docObjectCode);
			}
			if (objType == ApzObjectTypes.oDrafts)
			{
				doc.DocEntry = doc.draftKey;
			}
			return await UpdateData(doc, sessionId, objType, docObjectCode, ignoreUpdateEmptyValue);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<HttpResult> Save(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts, bool ignoreAttach = false, bool ignoreUpdateEmptyValue = false, bool ignoreCheckexists = false, string serverPath = "")
	{
		_ = 4;
		try
		{
			bool flag = doc.DocEntry <= 0 || (!ignoreCheckexists && !Exist(doc, sessionId, objType));
			if (!ignoreAttach && doc.AttachmentFiles != null && doc.AttachmentFiles.Count != 0)
			{
				HttpResult httpResult = SaveAttachment(doc, sessionId, objType, flag);
				if (httpResult.MessageCode != Enums.MessageCode.Success)
				{
					return httpResult;
				}
				doc.AtcEntry = Function.ParseInt(httpResult.Content);
			}
			if (flag)
			{
				if (doc.Document_ApprovalRequests != null && doc.Document_ApprovalRequests.Count > 0)
				{
					return await AddApprovalRequest(doc, sessionId, objType, serverPath);
				}
				if (doc.IsDraft && objType != ApzObjectTypes.oDrafts)
				{
					doc.DocObjectCode = objType;
					return SaveDraftToDocument(doc, sessionId);
				}
				return await AddNew(doc, sessionId, objType, docObjectCode);
			}
			if (doc.Document_ApprovalRequests != null && doc.Document_ApprovalRequests.Count > 0)
			{
				if (doc.DocEntry > 0 && ServiceUnitOfWork.Documents.GetWddStatus(objType, doc.DocEntry) == "W")
				{
					return await UpdateData(doc, sessionId, objType, docObjectCode, ignoreUpdateEmptyValue);
				}
				return await AddApprovalRequest(doc, sessionId, objType, serverPath);
			}
			if (objType == ApzObjectTypes.oDrafts)
			{
				doc.DocEntry = doc.draftKey;
			}
			if (doc.IsDraft && objType != ApzObjectTypes.oDrafts)
			{
				doc.DocObjectCode = objType;
				return SaveDraftToDocument(doc, sessionId);
			}
			return await UpdateData(doc, sessionId, objType, docObjectCode, ignoreUpdateEmptyValue);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual HttpResult SaveAttachment(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, bool addNew)
	{
		try
		{
			return ServiceUnitOfWork.Files.SaveAttachmentFiles(doc.AttachmentFiles, new Dictionary<string, object> { 
			{
				"DocEntry,int",
				(!addNew) ? doc.DocEntry : 0
			} }, GetTableNameByObjectType(objType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual HttpResult CancelDoc(int docEntry, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                 <env:Header>\r\n                                  <SessionID>" + sessionId + "</SessionID>\r\n                                 </env:Header>\r\n                                 <env:Body>\r\n                                  <dis:CancelObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                  <BOM>\r\n                                   <BO>\r\n                                    <AdmInfo>\r\n                                     <Object>" + Function.ToString(objType) + "</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams>\r\n                                     <DocEntry>" + Function.ToString(docEntry) + "</DocEntry>\r\n                                    </QueryParams>\r\n                                   </BO>\r\n                                  </BOM>\r\n                                  </dis:CancelObject>\r\n                                 </env:Body>\r\n                                </env:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", $"Cancel Document Error\t{objType}\t{xElement2.Value}");
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Cancel Document\t{objType}\t{num}");
					return new HttpResult(Enums.MessageCode.Success, "", num.ToString());
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, "Unknown error");
	}

	public virtual HttpResult CloseDoc(int docEntry, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                 <env:Header>\r\n                                  <SessionID>" + sessionId + "</SessionID>\r\n                                 </env:Header>\r\n                                 <env:Body>\r\n                                  <dis:CloseObject  xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                  <BOM>\r\n                                   <BO>\r\n                                    <AdmInfo>\r\n                                     <Object>" + Function.ToString(objType) + "</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams>\r\n                                     <DocEntry>" + Function.ToString(docEntry) + "</DocEntry>\r\n                                    </QueryParams>\r\n                                   </BO>\r\n                                  </BOM>\r\n                                  </dis:CloseObject >\r\n                                 </env:Body>\r\n                                </env:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", $"Close document - {objType} - {xElement2.Value}");
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Close document - {objType} - DocEntry - {num}");
					return new HttpResult(Enums.MessageCode.Success, "", num.ToString());
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, "Unknown Error");
	}

	protected virtual bool Exist(DocumentsImpl doc, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			List<string> tableName = Function.GetTableName(objType);
			if (doc.DocEntry <= 0)
			{
				return false;
			}
			if (tableName.Count == 0)
			{
				throw new Exception("Can not check exitsts data");
			}
			return !string.IsNullOrEmpty(ExecuteScarlar($"select 1 from {tableName[0]} where DocEntry = @docEntry", CommandType.Text, new SqlParameter("@docEntry", doc.DocEntry)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return !doc.DocEntry.Equals(0);
		}
	}

	protected virtual async Task<HttpResult> UpdateData(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts, bool ignoreUpdateEmptyValue = false)
	{
		try
		{
			string text = string.Empty;
			if (doc != null && doc.Udos != null && doc.Udos.Count > 0)
			{
				foreach (DocumentUdo udo2 in doc.Udos)
				{
					if (udo2.dsChild.Tables.Count > 0 && udo2.UdoTable.IsNotNull())
					{
						int udoEntry = GetUdoEntry(doc, udo2, "OPMT", "@OPMT", "U_paymentEntry");
						DataTable udo = GetUdo("OPMT");
						if (udo != null && udo.Rows.Count > 0)
						{
							text = ((udoEntry != 0) ? (text + _unitOfWork.UDOs.MappingXml(udo, udo2.dtFather, udo2.dsChild, sessionId, "Update")) : (text + _unitOfWork.UDOs.MappingXml(udo, udo2.dtFather, udo2.dsChild, sessionId, "Add")));
						}
					}
				}
			}
			string text2 = string.Empty;
			List<int> list = new List<int>();
			if (new ApzObjectTypes[5]
			{
				ApzObjectTypes.oQuotations,
				ApzObjectTypes.oOrders,
				ApzObjectTypes.oPurchaseRequest,
				ApzObjectTypes.oPurchaseQuotations,
				ApzObjectTypes.oPurchaseOrders
			}.Contains(objType) || objType == ApzObjectTypes.oDrafts || (new ApzObjectTypes[2]
			{
				ApzObjectTypes.oDownPayments,
				ApzObjectTypes.oPurchaseDownPayments
			}.Contains(objType) && doc.Posted == "0"))
			{
				List<string> tableName = Function.GetTableName(objType);
				IEnumerable<int> source = from t in doc.Lines
					select new { t.LineNum, t.VisualOrder } into t
					orderby t.VisualOrder
					select t.LineNum;
				string sql = "UPDATE T0\r\n                                            SET T0.VisOrder=T1.Id-1\r\n                                         FROM " + tableName[1] + " T0 INNER JOIN (SELECT * FROM dbo.Split('" + string.Join(",", source.ToArray()) + "',',')) T1 ON T0.LineNum=T1.Data\r\n                                         WHERE T0.DocEntry=" + doc.DocEntry;
				text2 = _unitOfWork.UDOs.GetXmlExecuteSQL(sql, "", isGetBody: true);
				string text3 = string.Join(",", doc.Lines.Select((Document_LinesImpl t) => t.LineNum));
				if (!string.IsNullOrEmpty(text3))
				{
					DataTable dt = ExecuteDataTable("SELECT * FROM " + tableName[1] + " WHERE DocEntry=@DocEntry AND LineNum NOT IN (" + text3 + ")", CommandType.Text, new SqlParameter("@DocEntry", doc.DocEntry));
					if (dt.IsNotNull())
					{
						List<Document_LinesImpl> list2 = dt.ToDocumentLine();
						if (list2 != null && list2.Count > 0)
						{
							list.AddRange(list2.Select((Document_LinesImpl t) => t.LineNum));
							doc.Lines.AddRange(list2);
						}
					}
				}
			}
			doc.Lines = doc.Lines.OrderBy((Document_LinesImpl t) => t.LineNum).ToList();
			string text4 = Mapping(doc, EnumCommandType.Update, objType, docObjectCode, ignoreUpdateEmptyValue);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text4 + "</dis:UpdateObject>" + text + text2 + "</env:Body></env:Envelopes>";
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
					Logging.Write("WATCH", $"Update document\t{objType}\t{num}");
					if (new ApzObjectTypes[7]
					{
						ApzObjectTypes.oQuotations,
						ApzObjectTypes.oOrders,
						ApzObjectTypes.oPurchaseRequest,
						ApzObjectTypes.oPurchaseQuotations,
						ApzObjectTypes.oPurchaseOrders,
						ApzObjectTypes.oDownPayments,
						ApzObjectTypes.oPurchaseDownPayments
					}.Contains(objType) || objType == ApzObjectTypes.oDrafts)
					{
						DeleteLines(doc, sessionId, objType, list);
					}
				}
			}
			return new HttpResult(Enums.MessageCode.Success, "", doc.DocEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult UpdateDataByService(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, ApzObjectTypes docObjectCode, string xml, DataTable removedBatchSerial)
	{
		try
		{
			string byParams = GetByParams(sessionId, objType, doc);
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[GetByPr] No matching record found.");
			}
			XElement element = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Document"));
			DataSet ds = new DataSet();
			LoadXml(ds, element);
			if (objType == ApzObjectTypes.oDrafts)
			{
				ds.Tables["Document"].Rows[0]["DocObjectCode"] = Function.ToString((int)docObjectCode);
			}
			if (!doc.SlpCode.Equals(0))
			{
				ds.Tables["Document"].Rows[0]["SalesPersonCode"] = Function.ParseInt(doc.SlpCode);
			}
			if (!string.IsNullOrEmpty(doc.Comments))
			{
				ds.Tables["Document"].Rows[0]["Comments"] = Function.RawText(doc.Comments);
			}
			if (doc.CntctCode != 0)
			{
				ds.Tables["Document"].Rows[0]["ContactPersonCode"] = Function.ParseInt(doc.CntctCode);
			}
			if (doc.AtcEntry != 0 && doc.AttachmentFiles != null && doc.AttachmentFiles.Count > 0)
			{
				ds.Tables["Document"].Rows[0]["AttachmentEntry"] = Function.ParseInt(doc.AtcEntry);
			}
			string text = Function.ToString(doc.DocStatus).ToUpper();
			if ((objType == ApzObjectTypes.oDrafts || objType == ApzObjectTypes.oOrders || objType == ApzObjectTypes.oQuotations || objType == ApzObjectTypes.oPurchaseOrders || objType == ApzObjectTypes.oPurchaseQuotations || objType == ApzObjectTypes.oPurchaseRequest) && (text.Equals("O") || text.Equals("OPEN")))
			{
				if (!string.IsNullOrEmpty(doc.Address))
				{
					ds.Tables["Document"].Rows[0]["Address"] = Function.RawText(doc.Address);
				}
				if ((!string.IsNullOrEmpty(doc.isIns) && new ApzObjectTypes[4]
				{
					ApzObjectTypes.oInvoices,
					ApzObjectTypes.oPurchaseInvoices,
					ApzObjectTypes.oReserveInvoices,
					ApzObjectTypes.oAPReserveInvoice
				}.Contains(objType)) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[4]
				{
					ApzObjectTypes.oInvoices,
					ApzObjectTypes.oPurchaseInvoices,
					ApzObjectTypes.oReserveInvoices,
					ApzObjectTypes.oAPReserveInvoice
				}.Contains(docObjectCode)))
				{
					ds.Tables["Document"].Rows[0]["ReserveInvoice"] = Function.RawText(doc.isIns);
				}
				if (!string.IsNullOrEmpty(doc.Address2))
				{
					ds.Tables["Document"].Rows[0]["Address2"] = Function.RawText(doc.Address2);
				}
				if (!string.IsNullOrEmpty(doc.AgentCode))
				{
					ds.Tables["Document"].Rows[0]["AgentCode"] = Function.RawText(doc.AgentCode);
				}
				if (doc.AnnualInvoiceDeclarationReference != 0)
				{
					ds.Tables["Document"].Rows[0]["AnnualInvoiceDeclarationReference"] = Function.ParseInt(doc.AnnualInvoiceDeclarationReference);
				}
				if (!string.IsNullOrEmpty(doc.BPChCode))
				{
					ds.Tables["Document"].Rows[0]["BPChannelCode"] = Function.RawText(doc.BPChCode);
				}
				if (doc.BPChCntc != 0)
				{
					ds.Tables["Document"].Rows[0]["BPChannelContact"] = Function.ParseInt(doc.BPChCntc);
				}
				if (doc.BPLId != 0 && doc.BPLId != -1)
				{
					ds.Tables["Document"].Rows[0]["BPL_IDAssignedToInvoice"] = Function.ParseInt(doc.BPLId);
				}
				if (doc.BlanketAgreementNumber != 0)
				{
					ds.Tables["Document"].Rows[0]["BlanketAgreementNumber"] = Function.ParseInt(doc.BlanketAgreementNumber);
				}
				if (!string.IsNullOrEmpty(doc.Box1099))
				{
					ds.Tables["Document"].Rows[0]["Box1099"] = Function.RawText(doc.Box1099);
				}
				if (!string.IsNullOrEmpty(doc.CardCode))
				{
					ds.Tables["Document"].Rows[0]["CardCode"] = Function.RawText(doc.CardCode);
				}
				if (!string.IsNullOrEmpty(doc.CardName))
				{
					ds.Tables["Document"].Rows[0]["CardName"] = Function.RawText(doc.CardName);
				}
				if (doc.CdcOffset != 0)
				{
					ds.Tables["Document"].Rows[0]["CashDiscountDateOffset"] = Function.ParseInt(doc.CdcOffset);
				}
				if (!string.IsNullOrEmpty(doc.CntrlBnk))
				{
					ds.Tables["Document"].Rows[0]["CentralBankIndicator"] = Function.RawText(doc.CntrlBnk);
				}
				if (!string.IsNullOrEmpty(doc.Footer))
				{
					ds.Tables["Document"].Rows[0]["ClosingRemarks"] = Function.RawText(doc.Footer);
				}
				if (!string.IsNullOrEmpty(doc.CtlAccount))
				{
					ds.Tables["Document"].Rows[0]["ControlAccount"] = Function.RawText(doc.CtlAccount);
				}
				if (!doc.DiscPrcnt.Equals(0.0))
				{
					ds.Tables["Document"].Rows[0]["DiscountPercent"] = Function.ParseDecimal(doc.DiscPrcnt);
				}
				if (!string.IsNullOrEmpty(doc.DocCur))
				{
					ds.Tables["Document"].Rows[0]["DocCurrency"] = Function.RawText(doc.DocCur);
				}
				if (doc.DocDate > new DateTime(1900, 1, 1))
				{
					ds.Tables["Document"].Rows[0]["DocDate"] = Convert.ToDateTime(doc.DocDate).ToString("yyyy-MM-dd");
				}
				if (doc.DocDueDate > new DateTime(1900, 1, 1))
				{
					ds.Tables["Document"].Rows[0]["DocDueDate"] = Convert.ToDateTime(doc.DocDueDate).ToString("yyyy-MM-dd");
				}
				if (!doc.DocRate.Equals(0.0))
				{
					ds.Tables["Document"].Rows[0]["DocRate"] = Function.ParseDecimal(doc.DocRate);
				}
				if (doc.DocDate > new DateTime(1900, 1, 1))
				{
					ds.Tables["Document"].Rows[0]["Comments"] = Convert.ToDateTime(doc.DocDate).ToString("yyyy-MM-dd");
				}
				if (!string.IsNullOrEmpty(doc.DocType))
				{
					if (doc.DocType == "S")
					{
						ds.Tables["Document"].Rows[0]["DocType"] = "dDocument_Service";
					}
					else
					{
						ds.Tables["Document"].Rows[0]["DocType"] = "dDocument_Items";
					}
				}
				if (doc.OwnerCode > 0)
				{
					ds.Tables["Document"].Rows[0]["DocumentsOwner"] = Function.ParseInt(doc.OwnerCode);
				}
				if (!doc.DownPayment.Equals(0.0))
				{
					ds.Tables["Document"].Rows[0]["DownPayment"] = Function.ParseDecimal(doc.DownPayment);
				}
				if (!doc.DpmAmnt.Equals(0.0))
				{
					ds.Tables["Document"].Rows[0]["DownPaymentAmount"] = Function.ParseDecimal(doc.DpmAmnt);
				}
				if (!doc.DpmAmntFC.Equals(0.0))
				{
					ds.Tables["Document"].Rows[0]["DownPaymentAmountFC"] = Function.ParseDecimal(doc.DpmAmntFC);
				}
				if (!doc.DpmAmntSC.Equals(0.0))
				{
					ds.Tables["Document"].Rows[0]["DownPaymentAmountSC"] = Function.ParseDecimal(doc.DpmAmntSC);
				}
				if (!doc.DpmPrcnt.Equals(0.0))
				{
					ds.Tables["Document"].Rows[0]["DownPaymentPercentage"] = Function.ParseDecimal(doc.DpmPrcnt);
				}
				if (doc.TaxDate > new DateTime(1900, 1, 1))
				{
					ds.Tables["Document"].Rows[0]["TaxDate"] = Convert.ToDateTime(doc.TaxDate).ToString("yyyy-MM-dd");
				}
				if (!doc.TrnspCode.Equals(0))
				{
					ds.Tables["Document"].Rows[0]["TransportationCode"] = Function.ParseInt(doc.TrnspCode);
				}
				if (doc.VatDate > new DateTime(1900, 1, 1))
				{
					ds.Tables["Document"].Rows[0]["VatDate"] = Convert.ToDateTime(doc.VatDate).ToString("yyyy-MM-dd");
				}
				if (!doc.VatPercent.Equals(0.0))
				{
					ds.Tables["Document"].Rows[0]["VatPercent"] = Function.ParseDecimal(doc.VatPercent);
				}
				if (objType == ApzObjectTypes.oPurchaseRequest || docObjectCode == ApzObjectTypes.oPurchaseRequest)
				{
					if (!string.IsNullOrEmpty(doc.Requester))
					{
						ds.Tables["Document"].Rows[0]["Requester"] = Function.ToString(doc.Requester);
					}
					if (!string.IsNullOrEmpty(doc.Email))
					{
						ds.Tables["Document"].Rows[0]["RequesterEmail"] = Function.ToString(doc.Email);
					}
					if (!string.IsNullOrEmpty(doc.ReqName))
					{
						ds.Tables["Document"].Rows[0]["RequesterName"] = Function.ToString(doc.ReqName);
					}
					if (!Function.ParseInt(doc.Branch).Equals(0))
					{
						ds.Tables["Document"].Rows[0]["RequesterBranch"] = Function.ParseInt(doc.Branch);
					}
					if (!Function.ParseInt(doc.Department).Equals(0))
					{
						ds.Tables["Document"].Rows[0]["RequesterDepartment"] = Function.ParseInt(doc.Department);
					}
					if (!Function.ParseInt(doc.ReqType).Equals(0))
					{
						ds.Tables["Document"].Rows[0]["ReqType"] = Function.ParseInt(doc.ReqType);
					}
				}
				if (doc.ReqDate > new DateTime(1900, 1, 1))
				{
					ds.Tables["Document"].Rows[0]["RequriedDate"] = Convert.ToDateTime(doc.ReqDate).ToString("yyyy-MM-dd");
				}
				if (!string.IsNullOrEmpty(doc.ShipToCode))
				{
					ds.Tables["Document"].Rows[0]["ShipToCode"] = Function.RawText(doc.ShipToCode);
				}
				if (!string.IsNullOrEmpty(doc.PayToCode))
				{
					ds.Tables["Document"].Rows[0]["PayToCode"] = Function.RawText(doc.PayToCode);
				}
			}
			if (objType != ApzObjectTypes.oInventoryGenEntry && objType != ApzObjectTypes.oInventoryGenExit)
			{
				if (!string.IsNullOrEmpty(doc.JrnlMemo))
				{
					ds.Tables["Document"].Rows[0]["JournalMemo"] = Function.RawText(doc.JrnlMemo);
				}
				if (!string.IsNullOrEmpty(doc.Project))
				{
					ds.Tables["Document"].Rows[0]["Project"] = Function.RawText(doc.Project);
				}
				if (!string.IsNullOrEmpty(doc.Indicator))
				{
					ds.Tables["Document"].Rows[0]["Indicator"] = Function.RawText(doc.Indicator);
				}
				if (!string.IsNullOrEmpty(doc.LicTradNum))
				{
					ds.Tables["Document"].Rows[0]["FederalTaxID"] = Function.RawText(doc.LicTradNum);
				}
				if (text.Equals("O") || text.Equals("OPEN"))
				{
					if (Function.ParseInt(doc.GroupNum) > 0)
					{
						ds.Tables["Document"].Rows[0]["PaymentGroupCode"] = Function.RawText(doc.GroupNum);
					}
					if (!string.IsNullOrEmpty(doc.PeyMethod))
					{
						ds.Tables["Document"].Rows[0]["PaymentMethod"] = Function.RawText(doc.PeyMethod);
					}
					if (objType == ApzObjectTypes.oDrafts)
					{
						if (Function.ParseInt(doc.ExtraMonth) > 0)
						{
							ds.Tables["Document"].Rows[0]["ExtraMonth"] = Function.RawText(doc.ExtraMonth);
							BoPayTermDueTypes boPayTermDueTypes;
							try
							{
								boPayTermDueTypes = doc.StartFrom;
							}
							catch
							{
								boPayTermDueTypes = BoPayTermDueTypes.pdt_None;
							}
							ds.Tables["Document"].Rows[0]["StartFrom"] = Function.RawText(boPayTermDueTypes);
						}
						if (Function.ParseInt(doc.ExtraDays) > 0)
						{
							ds.Tables["Document"].Rows[0]["ExtraDays"] = Function.RawText(doc.ExtraDays);
						}
					}
					if (Function.ParseInt(doc.CdcOffset) > 0)
					{
						ds.Tables["Document"].Rows[0]["CashDiscountDateOffset"] = Function.RawText(doc.CdcOffset);
					}
				}
			}
			MappingUdfTable(ref ds, "Document", doc.UserFields, EnumCommandType.Update);
			if (doc.Lines != null && doc.Lines.Count > 0 && ds.Tables.Contains("DocumentLine"))
			{
				(new DataColumn[1])[0] = ds.Tables["DocumentLine"].Columns["LineNum"];
				for (int num = 0; num < doc.Lines.Count; num++)
				{
					Document_LinesImpl model = doc.Lines[num];
					string text2 = Function.ToString(model.LineStatus).ToUpper();
					DataRow dataRow = ds.Tables["DocumentLine"].AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]) == model.LineNum);
					if (dataRow == null)
					{
						dataRow = ds.Tables["DocumentLine"].NewRow();
						for (int num2 = 0; num2 < dataRow.Table.Columns.Count; num2++)
						{
							Type dataType = dataRow.Table.Columns[num2].DataType;
							string columnName = dataRow.Table.Columns[num2].ColumnName;
							if (new Type[3]
							{
								typeof(int),
								typeof(double),
								typeof(decimal)
							}.Contains(dataType))
							{
								dataRow[columnName] = 0;
							}
						}
						dataRow["LineNum"] = model.LineNum;
						ds.Tables["DocumentLine"].Rows.InsertAt(dataRow, num);
						ds.AcceptChanges();
					}
					if ((objType == ApzObjectTypes.oDrafts || objType == ApzObjectTypes.oOrders || objType == ApzObjectTypes.oQuotations || objType == ApzObjectTypes.oPurchaseOrders || objType == ApzObjectTypes.oPurchaseQuotations || objType == ApzObjectTypes.oPurchaseRequest) && (text.Equals("O") || text.Equals("OPEN")) && (text2.Equals("O") || text2.Equals("OPEN")))
					{
						if (!string.IsNullOrEmpty(model.AcctCode))
						{
							dataRow["AccountCode"] = Function.RawText(model.AcctCode);
						}
						if (!model.ActBaseEnt.Equals(0))
						{
							dataRow["ActualBaseEntry"] = Function.ParseInt(model.ActBaseEnt);
						}
						if (model.ActDelDate > new DateTime(1900, 1, 1))
						{
							dataRow["AccountCode"] = Convert.ToDateTime(model.ActDelDate).ToString("yyyy-MM-dd");
						}
						if (!string.IsNullOrEmpty(model.Address))
						{
							dataRow["Address"] = Function.RawText(model.Address);
						}
						if (!model.AgrNo.Equals(0))
						{
							dataRow["AgreementNo"] = Function.ParseInt(model.AgrNo);
						}
						if (!model.AgrLnNum.Equals(0))
						{
							dataRow["AgreementRowNumber"] = Function.ParseInt(model.AgrLnNum);
						}
						if (!string.IsNullOrEmpty(model.CodeBars))
						{
							dataRow["BarCode"] = Function.RawText(model.CodeBars);
						}
						if (!model.BaseType.Equals(0) && !model.BaseType.Equals(-1) && objType != ApzObjectTypes.oQuotations && objType != ApzObjectTypes.oPurchaseQuotations)
						{
							dataRow["BaseType"] = Function.ParseInt(model.BaseType);
							if (!model.BaseEntry.Equals(0))
							{
								dataRow["BaseEntry"] = Function.ParseInt(model.BaseEntry);
							}
							if (!model.BaseLine.Equals(-1))
							{
								dataRow["BaseLine"] = Function.ParseInt(model.BaseLine);
							}
						}
						if (!model.Commission.Equals(0.0))
						{
							dataRow["CommisionPercent"] = Function.ParseDecimal(model.Commission);
						}
						if (!string.IsNullOrEmpty(model.OcrCode))
						{
							dataRow["CostingCode"] = Function.RawText(model.OcrCode);
						}
						if (!string.IsNullOrEmpty(model.OcrCode2))
						{
							if (!ds.Tables["DocumentLine"].IsExistsColumn("CostingCode2"))
							{
								ds.Tables["DocumentLine"].Columns.Add("CostingCode2", typeof(string));
							}
							dataRow["CostingCode2"] = Function.RawText(model.OcrCode2);
						}
						if (!string.IsNullOrEmpty(model.OcrCode3))
						{
							if (!ds.Tables["DocumentLine"].IsExistsColumn("CostingCode3"))
							{
								ds.Tables["DocumentLine"].Columns.Add("CostingCode3", typeof(string));
							}
							dataRow["CostingCode3"] = Function.RawText(model.OcrCode3);
						}
						if (!string.IsNullOrEmpty(model.OcrCode4))
						{
							if (!ds.Tables["DocumentLine"].IsExistsColumn("CostingCode4"))
							{
								ds.Tables["DocumentLine"].Columns.Add("CostingCode4", typeof(string));
							}
							dataRow["CostingCode4"] = Function.RawText(model.OcrCode4);
						}
						if (!string.IsNullOrEmpty(model.OcrCode5))
						{
							if (!ds.Tables["DocumentLine"].IsExistsColumn("CostingCode5"))
							{
								ds.Tables["DocumentLine"].Columns.Add("CostingCode5", typeof(string));
							}
							dataRow["CostingCode5"] = Function.RawText(model.OcrCode5);
						}
						if (!string.IsNullOrEmpty(model.Currency))
						{
							dataRow["Currency"] = Function.RawText(model.Currency);
						}
						if (!model.DiscPrcnt.Equals(0.0))
						{
							dataRow["DiscountPercent"] = Function.ParseDecimal(model.DiscPrcnt);
						}
						if (!model.ExciseAmt.Equals(0.0))
						{
							dataRow["ExciseAmount"] = Function.ParseDecimal(model.ExciseAmt);
						}
						if (!string.IsNullOrEmpty(model.ItemCode))
						{
							dataRow["ItemCode"] = Function.RawText(model.ItemCode);
						}
						if (!string.IsNullOrEmpty(model.Dscription))
						{
							dataRow["ItemDescription"] = Function.RawText(model.Dscription);
						}
						if (!model.InvQty.Equals(0.0))
						{
							dataRow["InventoryQuantity"] = Function.ParseDecimal(model.InvQty);
						}
						if (!string.IsNullOrEmpty(model.Text))
						{
							dataRow["ItemDetails"] = Function.RawText(model.Text);
						}
						if (model.LineNum >= 0)
						{
							dataRow["LineNum"] = Function.ParseInt(model.LineNum);
						}
						if (!string.IsNullOrEmpty(model.VendorNum))
						{
							dataRow["VendorNum"] = Function.RawText(model.VendorNum);
						}
						if (!model.LocCode.Equals(0))
						{
							dataRow["LocationCode"] = Function.ParseInt(model.LocCode);
						}
						if (!string.IsNullOrEmpty(model.unitMsr))
						{
							dataRow["MeasureUnit"] = Function.RawText(model.unitMsr);
						}
						if (!model.PriceBefDi.Equals(0.0))
						{
							dataRow["UnitPrice"] = Function.ParseDecimal(model.PriceBefDi);
						}
						if (!string.IsNullOrEmpty(model.Project))
						{
							dataRow["ProjectCode"] = Function.RawText(model.Project);
						}
						if (!model.Quantity.Equals(0.0))
						{
							dataRow["Quantity"] = Function.ParseDecimal(model.Quantity);
						}
						if (!model.Rate.Equals(0.0) && (model.TreeType == "N" || (model.TreeType == "I" && model.BOMTreeType != "0" && Function.ParseInt(model.BOMTreeType) == 0)))
						{
							dataRow["Rate"] = Function.ParseDecimal(model.Rate);
						}
						if (model.PQTReqDate > new DateTime(1900, 1, 1))
						{
							dataRow["RequiredDate"] = Convert.ToDateTime(model.PQTReqDate).ToString("yyyy-MM-dd");
						}
						if (!model.PQTReqQty.Equals(0.0))
						{
							dataRow["RequiredQuantity"] = Function.ParseDecimal(model.PQTReqQty);
						}
						if (!model.PackQty.Equals(0.0))
						{
							dataRow["PackageQuantity"] = Function.ParseDecimal(model.PackQty);
						}
						if (!doc.SlpCode.Equals(0))
						{
							dataRow["SalesPersonCode"] = Function.ParseInt(doc.SlpCode);
						}
						if (!model.SlpCode.Equals(0))
						{
							dataRow["SalesPersonCode"] = Function.ParseInt(model.SlpCode);
						}
						_ = model.ShipDate;
						if (model.ShipDate > new DateTime(1900, 1, 1))
						{
							dataRow["ShipDate"] = Convert.ToDateTime(model.ShipDate).ToString("yyyy-MM-dd");
						}
						if (!model.TrnsCode.Equals(0))
						{
							dataRow["ShippingMethod"] = Function.ParseInt(model.TrnsCode);
						}
						if (!string.IsNullOrEmpty(model.ShipToCode))
						{
							dataRow["ShipToCode"] = Function.RawText(model.ShipToCode);
						}
						if (!string.IsNullOrEmpty(model.ShipToDesc))
						{
							dataRow["ShipToDescription"] = Function.RawText(model.ShipToDesc);
						}
						if (!string.IsNullOrEmpty(model.TaxCode))
						{
							dataRow["TaxCode"] = Function.RawText(model.TaxCode);
						}
						if (model.NumPerMsr > 0.0)
						{
							dataRow["UnitsOfMeasurment"] = Function.ParseDecimal(model.NumPerMsr);
						}
						else
						{
							dataRow["UnitsOfMeasurment"] = 1;
						}
						if (!string.IsNullOrEmpty(model.UseBaseUn) && new string[4] { "N", "Y", "tYES", "tNO" }.Contains(model.UseBaseUn))
						{
							dataRow["UseBaseUnits"] = Function.RawText(model.UseBaseUn);
						}
						if (!model.UomEntry.Equals(0))
						{
							dataRow["UoMEntry"] = Function.ParseInt(model.UomEntry);
						}
						if (!string.IsNullOrEmpty(model.VatGroup))
						{
							dataRow["VatGroup"] = Function.RawText(model.VatGroup);
						}
						if (!string.IsNullOrEmpty(model.WhsCode))
						{
							dataRow["WarehouseCode"] = Function.RawText(model.WhsCode);
						}
						if (!string.IsNullOrEmpty(model.SubCatNum))
						{
							if (!ds.Tables["DocumentLine"].IsExistsColumn("SupplierCatNum"))
							{
								ds.Tables["DocumentLine"].Columns.Add("SupplierCatNum", typeof(string));
							}
							dataRow["SupplierCatNum"] = Function.ToString(model.SubCatNum);
						}
						if (!string.IsNullOrEmpty(model.LineVendor))
						{
							dataRow["LineVendor"] = Function.ToString(model.LineVendor);
						}
						if (new ApzObjectTypes[2]
						{
							ApzObjectTypes.oCreditNotes,
							ApzObjectTypes.oPurchaseCreditNotes
						}.Contains(objType) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[2]
						{
							ApzObjectTypes.oCreditNotes,
							ApzObjectTypes.oPurchaseCreditNotes
						}.Contains(docObjectCode)))
						{
							dataRow["WithoutInventoryMovement"] = ((model.NoInvtryMv == "Y") ? "tYES" : "tNO");
						}
						if (!string.IsNullOrEmpty(model.TreeType) && !"0".Equals(model.TreeType))
						{
							BoItemTreeTypes boItemTreeTypes = BoItemTreeTypes.iNotATree;
							switch (model.TreeType)
							{
							case "N":
								boItemTreeTypes = BoItemTreeTypes.iNotATree;
								break;
							case "A":
								boItemTreeTypes = BoItemTreeTypes.iAssemblyTree;
								break;
							case "S":
								boItemTreeTypes = BoItemTreeTypes.iSalesTree;
								break;
							case "I":
								boItemTreeTypes = BoItemTreeTypes.iIngredient;
								break;
							case "P":
								boItemTreeTypes = BoItemTreeTypes.iProductionTree;
								break;
							case "T":
								boItemTreeTypes = BoItemTreeTypes.iTemplateTree;
								break;
							}
							dataRow["TreeType"] = Function.ToString(boItemTreeTypes);
						}
					}
					if ((text.Equals("O") || text.Equals("OPEN") || text2.Equals("OPEN") || text2.Equals("O")) && !string.IsNullOrEmpty(model.FreeTxt))
					{
						dataRow["FreeText"] = Function.RawText(model.FreeTxt);
					}
					MappingUdfTable(ref ds, "DocumentLine", model.UserFields, EnumCommandType.Update);
				}
				if (doc.Lines != null && doc.Lines.Count > 0 && doc.Lines.Any((Document_LinesImpl t) => t.BatchNumbers != null && t.BatchNumbers.Count > 0))
				{
					List<DataRelation> list = ds.Relations.Cast<DataRelation>().ToList();
					ds.Relations.Clear();
					if (!ds.Tables.Contains("BatchNumbers"))
					{
						DataTable dataTable = new DataTable("BatchNumbers");
						DataColumn dataColumn = new DataColumn();
						dataColumn.ColumnMapping = MappingType.Hidden;
						dataColumn.DataType = typeof(int);
						dataColumn.ColumnName = "BatchNumbers_Id";
						dataTable.Columns.Add(dataColumn);
						dataColumn = new DataColumn();
						dataColumn.ColumnMapping = MappingType.Hidden;
						dataColumn.DataType = typeof(int);
						dataColumn.ColumnName = "DocumentLine_Id";
						dataTable.Columns.Add(dataColumn);
						ds.Tables.Add(dataTable.Copy());
					}
					if (!ds.Tables.Contains("BatchNumber"))
					{
						DataTable dataTable2 = new DataTable("BatchNumber");
						ds.Tables.Add(dataTable2.Copy());
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("BatchNumbers_Id"))
					{
						ds.Tables["BatchNumber"].Columns.Add(new DataColumn
						{
							ColumnName = "BatchNumbers_Id",
							DataType = typeof(int),
							ColumnMapping = MappingType.Hidden
						});
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("BatchNumber_Id"))
					{
						ds.Tables["BatchNumber"].Columns.Add(new DataColumn
						{
							ColumnName = "BatchNumber_Id",
							DataType = typeof(int),
							ColumnMapping = MappingType.Hidden
						});
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("BatchNumber_Id_0"))
					{
						ds.Tables["BatchNumber"].Columns.Add(new DataColumn
						{
							ColumnName = "BatchNumber_Id_0",
							DataType = typeof(int),
							ColumnMapping = MappingType.Hidden
						});
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("AddmisionDate"))
					{
						ds.Tables["BatchNumber"].Columns.Add("AddmisionDate", typeof(string));
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("BaseLineNumber"))
					{
						ds.Tables["BatchNumber"].Columns.Add("BaseLineNumber", typeof(int));
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("BatchNumber"))
					{
						ds.Tables["BatchNumber"].Columns.Add("BatchNumber", typeof(string));
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("ExpiryDate"))
					{
						ds.Tables["BatchNumber"].Columns.Add("ExpiryDate", typeof(string));
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("InternalSerialNumber"))
					{
						ds.Tables["BatchNumber"].Columns.Add("InternalSerialNumber", typeof(string));
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("Location"))
					{
						ds.Tables["BatchNumber"].Columns.Add("Location", typeof(string));
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("ManufacturerSerialNumber"))
					{
						ds.Tables["BatchNumber"].Columns.Add("ManufacturerSerialNumber", typeof(string));
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("ManufacturingDate"))
					{
						ds.Tables["BatchNumber"].Columns.Add("ManufacturingDate", typeof(string));
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("Notes"))
					{
						ds.Tables["BatchNumber"].Columns.Add("Notes", typeof(string));
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("Quantity"))
					{
						ds.Tables["BatchNumber"].Columns.Add("Quantity", typeof(decimal));
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("TrackingNote"))
					{
						ds.Tables["BatchNumber"].Columns.Add("TrackingNote", typeof(int));
					}
					if (!ds.Tables["BatchNumber"].IsExistsColumn("TrackingNoteLine"))
					{
						ds.Tables["BatchNumber"].Columns.Add("TrackingNoteLine", typeof(int));
					}
					if (list.Any((DataRelation t) => t.RelationName == "BatchNumber_BatchNumber"))
					{
						list.Remove(list.FirstOrDefault((DataRelation t) => t.RelationName == "BatchNumber_BatchNumber"));
					}
					if (!list.Any((DataRelation t) => t.RelationName == "DocumentLine_BatchNumbers"))
					{
						if (ds.Tables["DocumentLine"].IsExistsColumn("BatchNumbers"))
						{
							ds.Tables["DocumentLine"].Columns.Remove(ds.Tables["DocumentLine"].Columns["BatchNumbers"]);
						}
						if (!ds.Tables["DocumentLine"].IsExistsColumn("DocumentLine_Id"))
						{
							ds.Tables["DocumentLine"].Columns.Add(new DataColumn
							{
								ColumnMapping = MappingType.Hidden,
								ColumnName = "DocumentLine_Id",
								DataType = typeof(int)
							});
						}
						DataRelation dataRelation = new DataRelation("DocumentLine_BatchNumbers", ds.Tables["DocumentLine"].Columns["DocumentLine_Id"], ds.Tables["BatchNumbers"].Columns["DocumentLine_Id"]);
						dataRelation.Nested = true;
						list.Insert(0, dataRelation);
					}
					if (!list.Any((DataRelation t) => t.RelationName == "BatchNumbers_BatchNumber"))
					{
						DataRelation dataRelation2 = new DataRelation("BatchNumbers_BatchNumber", ds.Tables["BatchNumbers"].Columns["BatchNumbers_Id"], ds.Tables["BatchNumber"].Columns["BatchNumbers_Id"]);
						dataRelation2.Nested = true;
						list.Insert(0, dataRelation2);
					}
					ds.Relations.AddRange(list.ToArray());
					int num3 = 0;
					for (int i = 0; i < doc.Lines.Count; i++)
					{
						if (doc.Lines[i].BatchNumbers == null || doc.Lines[i].BatchNumbers.Count <= 0)
						{
							continue;
						}
						int lineNum = doc.Lines[i].LineNum;
						ds.Tables["DocumentLine"].Rows[i]["DocumentLine_Id"] = i;
						if (ds.Tables["BatchNumbers"].Select("DocumentLine_Id=" + i).Length == 0)
						{
							DataRow dataRow2 = ds.Tables["BatchNumbers"].NewRow();
							dataRow2["DocumentLine_Id"] = i;
							dataRow2["BatchNumbers_Id"] = i;
							ds.Tables["BatchNumbers"].Rows.Add(dataRow2);
						}
						DataRow[] array = ds.Tables["BatchNumber"].Select("BaseLineNumber = " + lineNum);
						for (int num4 = 0; num4 < array.Length; num4++)
						{
							ds.Tables["BatchNumber"].Rows.Remove(array[num4]);
						}
						for (int num5 = 0; num5 < doc.Lines[i].BatchNumbers.Count; num5++)
						{
							BatchNumbersImpl batch = doc.Lines[i].BatchNumbers[num5];
							DataRow dataRow3 = ds.Tables["BatchNumber"].AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["BatchNumber"]) == batch.BatchNumber && Function.ParseInt(t["BaseLineNumber"]) == i);
							if (dataRow3 == null)
							{
								dataRow3 = ds.Tables["BatchNumber"].NewRow();
								ds.Tables["BatchNumber"].Rows.Add(dataRow3);
							}
							dataRow3["BatchNumbers_Id"] = i;
							dataRow3["BatchNumber_Id"] = i;
							dataRow3["BatchNumber_Id_0"] = num3;
							if (!string.IsNullOrEmpty(batch.BatchNumber))
							{
								dataRow3["BatchNumber"] = Function.RawText(batch.BatchNumber);
							}
							if (!string.IsNullOrEmpty(batch.ManufacturerSerialNumber))
							{
								dataRow3["ManufacturerSerialNumber"] = Function.ToString(batch.ManufacturerSerialNumber);
							}
							if (!string.IsNullOrEmpty(batch.InternalSerialNumber))
							{
								dataRow3["InternalSerialNumber"] = Function.ToString(batch.InternalSerialNumber);
							}
							if (batch.ExpiryDate.HasValue && batch.ExpiryDate > new DateTime(1900, 1, 1))
							{
								dataRow3["ExpiryDate"] = Convert.ToDateTime(batch.ExpiryDate).ToString("yyyyMMdd");
							}
							if (batch.ManufacturingDate.HasValue && batch.ManufacturingDate > new DateTime(1900, 1, 1))
							{
								dataRow3["ManufacturingDate"] = Convert.ToDateTime(batch.ManufacturingDate).ToString("yyyyMMdd");
							}
							if (batch.AddmisionDate.HasValue && batch.AddmisionDate > new DateTime(1900, 1, 1))
							{
								dataRow3["AddmisionDate"] = Convert.ToDateTime(batch.AddmisionDate).ToString("yyyyMMdd");
							}
							if (!string.IsNullOrEmpty(batch.Location))
							{
								dataRow3["Location"] = Function.RawText(batch.Location);
							}
							if (!string.IsNullOrEmpty(batch.Notes))
							{
								dataRow3["Notes"] = Function.RawText(batch.Notes);
							}
							if (!batch.Quantity.Equals(0.0))
							{
								dataRow3["Quantity"] = Function.ParseDecimal(batch.Quantity);
							}
							if (batch.BaseLineNumber >= 0)
							{
								dataRow3["BaseLineNumber"] = batch.BaseLineNumber;
							}
							if (0 < batch.TrackingNote)
							{
								dataRow3["TrackingNote"] = Function.ParseInt(batch.TrackingNote);
							}
							if (0 < batch.TrackingNoteLine)
							{
								dataRow3["TrackingNoteLine"] = Function.ParseInt(batch.TrackingNoteLine);
							}
						}
						num3++;
					}
				}
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					List<DataRelation> list2 = ds.Relations.Cast<DataRelation>().ToList();
					ds.Relations.Clear();
					if (!ds.Tables.Contains("SerialNumbers"))
					{
						DataTable dataTable3 = new DataTable("SerialNumbers");
						DataColumn dataColumn2 = new DataColumn();
						dataColumn2.ColumnMapping = MappingType.Hidden;
						dataColumn2.DataType = typeof(int);
						dataColumn2.ColumnName = "SerialNumbers_Id";
						dataTable3.Columns.Add(dataColumn2);
						dataColumn2 = new DataColumn();
						dataColumn2.ColumnMapping = MappingType.Hidden;
						dataColumn2.DataType = typeof(int);
						dataColumn2.ColumnName = "DocumentLine_Id";
						dataTable3.Columns.Add(dataColumn2);
						ds.Tables.Add(dataTable3.Copy());
					}
					if (!ds.Tables.Contains("SerialNumber"))
					{
						DataTable dataTable4 = new DataTable("SerialNumber");
						ds.Tables.Add(dataTable4.Copy());
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("SerialNumbers_Id"))
					{
						ds.Tables["SerialNumber"].Columns.Add(new DataColumn
						{
							ColumnName = "SerialNumbers_Id",
							DataType = typeof(int),
							ColumnMapping = MappingType.Hidden
						});
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("SerialNumber_Id"))
					{
						ds.Tables["SerialNumber"].Columns.Add(new DataColumn
						{
							ColumnName = "SerialNumber_Id",
							DataType = typeof(int),
							ColumnMapping = MappingType.Hidden
						});
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("BatchID"))
					{
						ds.Tables["SerialNumber"].Columns.Add("BatchID", typeof(string));
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("ManufacturerSerialNumber"))
					{
						ds.Tables["SerialNumber"].Columns.Add("ManufacturerSerialNumber", typeof(string));
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("ExpiryDate"))
					{
						ds.Tables["SerialNumber"].Columns.Add("ExpiryDate", typeof(string));
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("InternalSerialNumber"))
					{
						ds.Tables["SerialNumber"].Columns.Add("InternalSerialNumber", typeof(string));
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("ManufactureDate"))
					{
						ds.Tables["SerialNumber"].Columns.Add("ManufactureDate", typeof(string));
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("ReceptionDate"))
					{
						ds.Tables["SerialNumber"].Columns.Add("ReceptionDate", typeof(string));
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("Location"))
					{
						ds.Tables["SerialNumber"].Columns.Add("Location", typeof(string));
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("Notes"))
					{
						ds.Tables["SerialNumber"].Columns.Add("Notes", typeof(string));
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("Quantity"))
					{
						ds.Tables["SerialNumber"].Columns.Add("Quantity", typeof(string));
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("BaseLineNumber"))
					{
						ds.Tables["SerialNumber"].Columns.Add("BaseLineNumber", typeof(string));
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("TrackingNote"))
					{
						ds.Tables["SerialNumber"].Columns.Add("TrackingNote", typeof(string));
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("TrackingNoteLine"))
					{
						ds.Tables["SerialNumber"].Columns.Add("TrackingNoteLine", typeof(string));
					}
					if (!ds.Tables["SerialNumber"].IsExistsColumn("SystemSerialNumber"))
					{
						ds.Tables["SerialNumber"].Columns.Add("SystemSerialNumber", typeof(int));
					}
					if (!list2.Any((DataRelation t) => t.RelationName == "DocumentLine_SerialNumbers"))
					{
						if (ds.Tables["DocumentLine"].IsExistsColumn("SerialNumbers"))
						{
							ds.Tables["DocumentLine"].Columns.Remove(ds.Tables["DocumentLine"].Columns["SerialNumbers"]);
						}
						if (!ds.Tables["DocumentLine"].IsExistsColumn("DocumentLine_Id"))
						{
							ds.Tables["DocumentLine"].Columns.Add(new DataColumn
							{
								ColumnMapping = MappingType.Hidden,
								ColumnName = "DocumentLine_Id",
								DataType = typeof(int)
							});
						}
						DataRelation dataRelation3 = new DataRelation("DocumentLine_SerialNumbers", ds.Tables["DocumentLine"].Columns["DocumentLine_Id"], ds.Tables["SerialNumbers"].Columns["DocumentLine_Id"]);
						dataRelation3.Nested = true;
						list2.Insert(0, dataRelation3);
					}
					if (!list2.Any((DataRelation t) => t.RelationName == "SerialNumbers_SerialNumber"))
					{
						DataRelation dataRelation4 = new DataRelation("SerialNumbers_SerialNumber", ds.Tables["SerialNumbers"].Columns["SerialNumbers_Id"], ds.Tables["SerialNumber"].Columns["SerialNumbers_Id"]);
						dataRelation4.Nested = true;
						list2.Insert(0, dataRelation4);
					}
					ds.Relations.AddRange(list2.ToArray());
					int num6 = 0;
					for (int i2 = 0; i2 < doc.Lines.Count; i2++)
					{
						int linenum = doc.Lines[i2].LineNum;
						if ((doc.Lines[i2].SerialNumbers != null && doc.Lines[i2].SerialNumbers.Count == 0) || doc.Lines[i2].SerialNumbers == null)
						{
							DataRow[] array2 = (from t in ds.Tables["SerialNumber"].AsEnumerable()
								where Function.ParseInt(t["BaseLineNumber"]) == linenum
								select t).ToArray();
							if (array2 != null && array2.Length != 0)
							{
								foreach (DataRow item in from t in removedBatchSerial.AsEnumerable()
									where Function.ParseInt(t["OrderLine"]) == linenum
									select t)
								{
									DataRow dataRow4 = array2.FirstOrDefault((DataRow t) => t.RowState != DataRowState.Detached && t.RowState != DataRowState.Deleted && Function.ParseInt(t["SystemSerialNumber"]) == Function.ParseInt(item["SysNumber"]));
									if (dataRow4 != null)
									{
										dataRow4["Quantity"] = 0;
									}
								}
								ds.Tables["SerialNumber"].AcceptChanges();
							}
						}
						if (doc.Lines[i2].SerialNumbers == null || doc.Lines[i2].SerialNumbers.Count <= 0)
						{
							continue;
						}
						int documentlineId = linenum;
						DataRow dataRow5 = ds.Tables["DocumentLine"].AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]) == linenum);
						if (dataRow5 != null)
						{
							documentlineId = Function.ParseInt(dataRow5["DocumentLine_Id"]);
						}
						int num7 = linenum;
						if (ds.Tables["SerialNumbers"].IsNotNull())
						{
							num7 = ds.Tables["SerialNumbers"].AsEnumerable().Max((DataRow t) => Function.ParseInt(t["SerialNumbers_Id"])) + 1;
						}
						DataRow dataRow6 = ds.Tables["SerialNumbers"].AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["DocumentLine_Id"]) == documentlineId);
						if (dataRow6 == null)
						{
							DataRow dataRow7 = ds.Tables["SerialNumbers"].NewRow();
							dataRow7["DocumentLine_Id"] = documentlineId;
							dataRow7["SerialNumbers_Id"] = num7;
							ds.Tables["SerialNumbers"].Rows.Add(dataRow7);
						}
						else
						{
							num7 = Function.ParseInt(dataRow6["SerialNumbers_Id"]);
						}
						DataRow[] array3 = (from t in ds.Tables["SerialNumber"].AsEnumerable()
							where Function.ParseInt(t["BaseLineNumber"]) == linenum
							select t).ToArray();
						if (array3 != null && array3.Length != 0)
						{
							num7 = Function.ParseInt(array3[0]["SerialNumbers_Id"]);
							if (removedBatchSerial.IsNotNull())
							{
								foreach (DataRow item2 in from t in removedBatchSerial.AsEnumerable()
									where Function.ParseInt(t["OrderLine"]) == linenum
									select t)
								{
									DataRow dataRow8 = array3.FirstOrDefault((DataRow t) => t.RowState != DataRowState.Detached && t.RowState != DataRowState.Deleted && Function.ParseInt(t["SystemSerialNumber"]) == Function.ParseInt(item2["SysNumber"]));
									if (dataRow8 != null)
									{
										ds.Tables["SerialNumber"].Rows.Remove(dataRow8);
									}
								}
								ds.Tables["SerialNumber"].AcceptChanges();
							}
						}
						for (int num8 = 0; num8 < doc.Lines[i2].SerialNumbers.Count; num8++)
						{
							SerialNumbersImpl serial = doc.Lines[i2].SerialNumbers[num8];
							int systemnumber = Function.ParseInt(serial.SystemSerialNumber);
							if (array3.FirstOrDefault((DataRow t) => t.RowState != DataRowState.Detached && t.RowState != DataRowState.Deleted && Function.ParseInt(t["SystemSerialNumber"]) == systemnumber) == null)
							{
								DataRow dataRow9 = ds.Tables["SerialNumber"].AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["InternalSerialNumber"]) == serial.InternalSerialNumber && Function.ParseInt(t["BaseLineNumber"]) == i2);
								if (dataRow9 == null)
								{
									dataRow9 = ds.Tables["SerialNumber"].NewRow();
									ds.Tables["SerialNumber"].Rows.Add(dataRow9);
								}
								dataRow9["SerialNumbers_Id"] = num7;
								dataRow9["SerialNumber_Id"] = num8;
								if (!string.IsNullOrEmpty(serial.BatchID))
								{
									dataRow9["BatchID"] = Function.RawText(serial.BatchID);
								}
								if (!string.IsNullOrEmpty(serial.ManufacturerSerialNumber))
								{
									dataRow9["ManufacturerSerialNumber"] = Function.RawText(serial.ManufacturerSerialNumber);
								}
								if (!string.IsNullOrEmpty(serial.InternalSerialNumber))
								{
									dataRow9["InternalSerialNumber"] = Function.RawText(serial.InternalSerialNumber);
								}
								if (serial.ExpiryDate.HasValue && serial.ExpiryDate > new DateTime(1900, 1, 1))
								{
									dataRow9["ExpiryDate"] = Convert.ToDateTime(serial.ExpiryDate).ToString("yyyy-MM-dd");
								}
								if (serial.ManufactureDate.HasValue && serial.ManufactureDate > new DateTime(1900, 1, 1))
								{
									dataRow9["ManufacturingDate"] = Convert.ToDateTime(serial.ManufactureDate).ToString("yyyy-MM-dd");
								}
								if (serial.ReceptionDate.HasValue && serial.ReceptionDate > new DateTime(1900, 1, 1))
								{
									dataRow9["ReceptionDate"] = Convert.ToDateTime(serial.ReceptionDate).ToString("yyyy-MM-dd");
								}
								if (!string.IsNullOrEmpty(serial.Location))
								{
									dataRow9["Location"] = Function.RawText(serial.Location);
								}
								if (!string.IsNullOrEmpty(serial.Notes))
								{
									dataRow9["Notes"] = Function.RawText(serial.Notes);
								}
								if (!serial.Quantity.Equals(0.0))
								{
									dataRow9["Quantity"] = Function.ParseDecimal(serial.Quantity);
								}
								if (serial.BaseLineNumber >= 0)
								{
									dataRow9["BaseLineNumber"] = Function.ParseInt(serial.BaseLineNumber);
								}
								if (0 < serial.TrackingNote)
								{
									dataRow9["TrackingNote"] = Function.RawText(serial.TrackingNote);
								}
								if (0 < serial.TrackingNoteLine)
								{
									dataRow9["TrackingNoteLine"] = Function.RawText(serial.TrackingNoteLine);
								}
								if (0 < serial.SystemSerialNumber)
								{
									dataRow9["SystemSerialNumber"] = systemnumber;
								}
							}
						}
						num6++;
					}
					stopwatch.Stop();
					Console.WriteLine(stopwatch.ElapsedMilliseconds.ToString());
				}
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					xml += "<DocumentLinesBinAllocations>";
					for (int num9 = 0; num9 < doc.Lines.Count; num9++)
					{
						if (doc.Lines[num9].BinAllocations == null || doc.Lines[num9].BinAllocations.Count <= 0)
						{
							continue;
						}
						for (int num10 = 0; num10 < doc.Lines[num9].BinAllocations.Count; num10++)
						{
							xml += "<row>";
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = doc.Lines[num9].BinAllocations[num10];
							if (!documentLinesBinAllocationsImpl.BinAbsEntry.Equals(0))
							{
								xml = xml + "<BinAbsEntry>" + Function.ParseInt(documentLinesBinAllocationsImpl.BinAbsEntry) + "</BinAbsEntry>";
							}
							if (!documentLinesBinAllocationsImpl.Quantity.Equals(0.0))
							{
								xml = xml + "<Quantity>" + Function.ParseDecimal(documentLinesBinAllocationsImpl.Quantity) + "</Quantity>";
							}
							xml = xml + "<SerialAndBatchNumbersBaseLine>" + ((documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine >= 0) ? documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine : new int?(-1)) + "</SerialAndBatchNumbersBaseLine>";
							if (documentLinesBinAllocationsImpl.BaseLineNumber >= 0)
							{
								xml = xml + "<BaseLineNumber>" + Function.ParseInt(documentLinesBinAllocationsImpl.BaseLineNumber) + "</BaseLineNumber>";
							}
							xml += "</row>";
						}
					}
					xml += "</DocumentLinesBinAllocations>";
				}
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					xml += "<Document_LinesAdditionalExpenses>";
					for (int num11 = 0; num11 < doc.Lines.Count; num11++)
					{
						Document_LinesImpl document_LinesImpl = doc.Lines[num11];
						if (document_LinesImpl == null)
						{
							continue;
						}
						if (document_LinesImpl.ExpnsCode_F1 > 0)
						{
							xml += "<row>";
							xml = xml + "<LineNumber>" + document_LinesImpl.LineNum + "</LineNumber>";
							xml += "<GroupCode>0</GroupCode>";
							xml = xml + "<ExpenseCode>" + document_LinesImpl.ExpnsCode_F1 + "</ExpenseCode>";
							if (!Function.ParseDecimal(document_LinesImpl.LineTotal_F1).Equals(0m))
							{
								xml = xml + "<LineTotal>" + Function.ParseDecimal(document_LinesImpl.LineTotal_F1) + "</LineTotal>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.VatGroup_F1))
							{
								xml = xml + "<VatGroup>" + Function.RawText(document_LinesImpl.VatGroup_F1) + "</VatGroup>";
							}
							if (!Function.ParseDecimal(document_LinesImpl.VatPrcnt_F1).Equals(0m))
							{
								xml = xml + "<TaxPercent>" + Function.ParseDecimal(document_LinesImpl.VatPrcnt_F1) + "</TaxPercent>";
							}
							if (!Function.ParseDecimal(document_LinesImpl.VatSum_F1).Equals(0m))
							{
								xml = xml + "<TaxSum>" + Function.ParseDecimal(document_LinesImpl.VatSum_F1) + "</TaxSum>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode_F1))
							{
								xml = xml + "<DistributionRule>" + Function.RawText(document_LinesImpl.OcrCode_F1) + "</DistributionRule>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode2_F1))
							{
								xml = xml + "<DistributionRule2>" + Function.RawText(document_LinesImpl.OcrCode2_F1) + "</DistributionRule2>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode3_F1))
							{
								xml = xml + "<DistributionRule3>" + Function.RawText(document_LinesImpl.OcrCode3_F1) + "</DistributionRule3>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode4_F1))
							{
								xml = xml + "<DistributionRule4>" + Function.RawText(document_LinesImpl.OcrCode4_F1) + "</DistributionRule4>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode5_F1))
							{
								xml = xml + "<DistributionRule5>" + Function.RawText(document_LinesImpl.OcrCode5_F1) + "</DistributionRule5>";
							}
							xml += "</row>";
						}
						if (document_LinesImpl.ExpnsCode_F2 > 0)
						{
							xml += "<row>";
							xml = xml + "<LineNumber>" + document_LinesImpl.LineNum + "</LineNumber>";
							xml += "<GroupCode>1</GroupCode>";
							xml = xml + "<ExpenseCode>" + document_LinesImpl.ExpnsCode_F2 + "</ExpenseCode>";
							if (!Function.ParseDecimal(document_LinesImpl.LineTotal_F2).Equals(0m))
							{
								xml = xml + "<LineTotal>" + Function.ParseDecimal(document_LinesImpl.LineTotal_F2) + "</LineTotal>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.VatGroup_F2))
							{
								xml = xml + "<VatGroup>" + Function.RawText(document_LinesImpl.VatGroup_F2) + "</VatGroup>";
							}
							if (!Function.ParseDecimal(document_LinesImpl.VatPrcnt_F2).Equals(0m))
							{
								xml = xml + "<TaxPercent>" + Function.ParseDecimal(document_LinesImpl.VatPrcnt_F2) + "</TaxPercent>";
							}
							if (!Function.ParseDecimal(document_LinesImpl.VatSum_F2).Equals(0m))
							{
								xml = xml + "<TaxSum>" + Function.ParseDecimal(document_LinesImpl.VatSum_F2) + "</TaxSum>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode_F2))
							{
								xml = xml + "<DistributionRule>" + Function.RawText(document_LinesImpl.OcrCode_F2) + "</DistributionRule>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode2_F2))
							{
								xml = xml + "<DistributionRule2>" + Function.RawText(document_LinesImpl.OcrCode2_F2) + "</DistributionRule2>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode3_F2))
							{
								xml = xml + "<DistributionRule3>" + Function.RawText(document_LinesImpl.OcrCode3_F2) + "</DistributionRule3>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode4_F2))
							{
								xml = xml + "<DistributionRule4>" + Function.RawText(document_LinesImpl.OcrCode4_F2) + "</DistributionRule4>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode5_F2))
							{
								xml = xml + "<DistributionRule5>" + Function.RawText(document_LinesImpl.OcrCode5_F2) + "</DistributionRule5>";
							}
							xml += "</row>";
						}
						if (document_LinesImpl.ExpnsCode_F3 > 0)
						{
							xml += "<row>";
							xml = xml + "<LineNumber>" + document_LinesImpl.LineNum + "</LineNumber>";
							xml += "<GroupCode>2</GroupCode>";
							xml = xml + "<ExpenseCode>" + document_LinesImpl.ExpnsCode_F3 + "</ExpenseCode>";
							if (!Function.ParseDecimal(document_LinesImpl.LineTotal_F3).Equals(0m))
							{
								xml = xml + "<LineTotal>" + Function.ParseDecimal(document_LinesImpl.LineTotal_F3) + "</LineTotal>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.VatGroup_F3))
							{
								xml = xml + "<VatGroup>" + Function.RawText(document_LinesImpl.VatGroup_F3) + "</VatGroup>";
							}
							if (!Function.ParseDecimal(document_LinesImpl.VatPrcnt_F3).Equals(0m))
							{
								xml = xml + "<TaxPercent>" + Function.ParseDecimal(document_LinesImpl.VatPrcnt_F3) + "</TaxPercent>";
							}
							if (!Function.ParseDecimal(document_LinesImpl.VatSum_F3).Equals(0m))
							{
								xml = xml + "<TaxSum>" + Function.ParseDecimal(document_LinesImpl.VatSum_F3) + "</TaxSum>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode_F3))
							{
								xml = xml + "<DistributionRule>" + Function.RawText(document_LinesImpl.OcrCode_F3) + "</DistributionRule>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode2_F3))
							{
								xml = xml + "<DistributionRule2>" + Function.RawText(document_LinesImpl.OcrCode2_F3) + "</DistributionRule2>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode3_F3))
							{
								xml = xml + "<DistributionRule3>" + Function.RawText(document_LinesImpl.OcrCode3_F3) + "</DistributionRule3>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode4_F3))
							{
								xml = xml + "<DistributionRule4>" + Function.RawText(document_LinesImpl.OcrCode4_F3) + "</DistributionRule4>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode5_F3))
							{
								xml = xml + "<DistributionRule5>" + Function.RawText(document_LinesImpl.OcrCode5_F3) + "</DistributionRule5>";
							}
							xml += "</row>";
						}
					}
					xml += "</Document_LinesAdditionalExpenses>";
				}
			}
			if (doc.Expenses != null && doc.Expenses.Count > 0)
			{
				xml += "<DocumentsAdditionalExpenses>";
				for (int num12 = 0; num12 < doc.Expenses.Count; num12++)
				{
					xml += "<row>";
					DocumentsAdditionalExpensesImpl documentsAdditionalExpensesImpl = doc.Expenses[num12];
					if (documentsAdditionalExpensesImpl != null && !string.IsNullOrEmpty(documentsAdditionalExpensesImpl.Status) && documentsAdditionalExpensesImpl.Status.ToUpper().Equals("O"))
					{
						if (!Function.ParseInt(documentsAdditionalExpensesImpl.ExpnsCode).Equals(0))
						{
							xml = xml + "<ExpenseCode>" + Function.ParseInt(documentsAdditionalExpensesImpl.ExpnsCode) + "</ExpenseCode>";
						}
						if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.LineTotal).Equals(0m))
						{
							xml = xml + "<LineTotal>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.LineTotal) + "</LineTotal>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.Comments))
						{
							xml = xml + "<Remarks>" + Function.RawText(documentsAdditionalExpensesImpl.Comments) + "</Remarks>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.DistrbMthd))
						{
							xml = xml + "<DistributionMethod>" + Function.RawText(documentsAdditionalExpensesImpl.DistrbMthd) + "</DistributionMethod>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.VatGroup))
						{
							xml = xml + "<VatGroup>" + Function.RawText(documentsAdditionalExpensesImpl.VatGroup) + "</VatGroup>";
						}
						if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.VatPrcnt).Equals(0m))
						{
							xml = xml + "<TaxPercent>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.VatPrcnt) + "</TaxPercent>";
						}
						if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSum).Equals(0m))
						{
							xml = xml + "<TaxSum>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSum) + "</TaxSum>";
						}
						if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.DedVatSum).Equals(0m))
						{
							xml = xml + "<DeductibleTaxSum>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.DedVatSum) + "</DeductibleTaxSum>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.TaxCode))
						{
							xml = xml + "<TaxCode>" + Function.RawText(documentsAdditionalExpensesImpl.TaxCode) + "</TaxCode>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.TaxType))
						{
							xml = xml + "<TaxType>" + Function.RawText(documentsAdditionalExpensesImpl.TaxType) + "</TaxType>";
						}
						if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatPer).Equals(0m))
						{
							xml = xml + "<EqualizationTaxPercent>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatPer) + "</EqualizationTaxPercent>";
						}
						if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatSum).Equals(0m))
						{
							xml = xml + "<EqualizationTaxSum>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatSum) + "</EqualizationTaxSum>";
						}
						if (!Function.ParseInt(documentsAdditionalExpensesImpl.BaseType).Equals(0) && !Function.ParseInt(documentsAdditionalExpensesImpl.BaseType).Equals(-1))
						{
							xml = xml + "<BaseDocType>" + Function.ParseInt(documentsAdditionalExpensesImpl.BaseType) + "</BaseDocType>";
							if (!Function.ParseInt(documentsAdditionalExpensesImpl.DocEntry).Equals(0))
							{
								xml = xml + "<BaseDocEntry>" + Function.ParseInt(documentsAdditionalExpensesImpl.DocEntry) + "</BaseDocEntry>";
							}
							if (!Function.ParseInt(documentsAdditionalExpensesImpl.BaseLnNum).Equals(0))
							{
								xml = xml + "<BaseDocLine>" + Function.ParseInt(documentsAdditionalExpensesImpl.BaseLnNum) + "</BaseDocLine>";
							}
						}
						xml += (Function.ParseBool(documentsAdditionalExpensesImpl.LstPchPrce) ? "<LastPurchasePrice>tYES</LastPurchasePrice>" : "<LastPurchasePrice>tNO</LastPurchasePrice>");
						xml += (Function.ParseBool(documentsAdditionalExpensesImpl.Stock) ? "<Stock>tYES</Stock>" : "<Stock>tNO</Stock>");
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode))
						{
							xml = xml + "<DistributionRule>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode) + "</DistributionRule>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode2))
						{
							xml = xml + "<DistributionRule2>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode2) + "</DistributionRule2>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode3))
						{
							xml = xml + "<DistributionRule3>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode3) + "</DistributionRule3>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode4))
						{
							xml = xml + "<DistributionRule4>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode4) + "</DistributionRule4>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode5))
						{
							xml = xml + "<DistributionRule5>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode5) + "</DistributionRule5>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.Project))
						{
							xml = xml + "<Project>" + Function.RawText(documentsAdditionalExpensesImpl.Project) + "</Project>";
						}
					}
					xml += "</row>";
				}
				xml += "</DocumentsAdditionalExpenses>";
			}
			xml += "</BO></BOM>";
			if (ds.Tables.Contains("DocumentLine"))
			{
				string text3 = XElement.Parse(ds.GetXml()).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Document"))
					.ToString();
				if (!string.IsNullOrEmpty(text3))
				{
					string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>" + GetServiceName(objType) + "</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"" + GetServiceName(objType) + "\">" + text3 + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
					Stopwatch stopwatch2 = new Stopwatch();
					stopwatch2.Start();
					XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
					stopwatch2.Stop();
					XElement xElement = XElement.Parse(xmlElement.OuterXml);
					if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
					{
						XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
						if (xElement2 != null)
						{
							throw new Exception(xElement2.Value);
						}
					}
					return new HttpResult();
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

	public void LoadXml(DataSet ds, XElement element)
	{
		LoadXmlColumn(ds, element);
		LoadXmlValue(ds, element, "", -1);
	}

	public void LoadXmlColumn(DataSet ds, XElement element, string fatherTableName = "", string fatherColumnName = "")
	{
		DataTable dataTable = new DataTable();
		if (ds.Tables.Contains(element.Name.LocalName))
		{
			dataTable = ds.Tables[element.Name.LocalName];
		}
		else
		{
			dataTable = new DataTable(element.Name.LocalName);
			ds.Tables.Add(dataTable);
		}
		string text = $"{element.Name.LocalName}_Id";
		DataColumn dataColumn = new DataColumn(text, typeof(int));
		dataColumn.ColumnMapping = MappingType.Hidden;
		if (!dataTable.IsExistsColumn(text))
		{
			dataTable.Columns.Add(dataColumn);
		}
		if (!string.IsNullOrEmpty(fatherTableName) && !string.IsNullOrEmpty(fatherColumnName))
		{
			DataColumn dataColumn2 = new DataColumn(fatherColumnName, typeof(int));
			dataColumn2.ColumnMapping = MappingType.Hidden;
			if (!dataTable.IsExistsColumn(fatherColumnName))
			{
				dataTable.Columns.Add(dataColumn2);
			}
			string text2 = $"{fatherTableName}_{element.Name.LocalName}";
			if (!ds.Relations.Contains(text2))
			{
				DataRelation dataRelation = new DataRelation(text2, ds.Tables[fatherTableName].Columns[fatherColumnName], dataColumn2);
				dataRelation.Nested = true;
				ds.Relations.Add(dataRelation);
			}
		}
		foreach (XElement item in element.Elements())
		{
			if (item == null)
			{
				continue;
			}
			if (item.HasElements)
			{
				LoadXmlColumn(ds, item, element.Name.LocalName, text);
			}
			else if (!item.IsEmpty && !dataTable.Columns.Contains(item.Name.LocalName))
			{
				DataColumn dataColumn3 = new DataColumn();
				dataColumn3.ColumnName = item.Name.LocalName;
				dataColumn3.DataType = typeof(string);
				if (!dataTable.IsExistsColumn(dataColumn3.ColumnName))
				{
					dataTable.Columns.Add(dataColumn3);
				}
			}
		}
	}

	public void LoadXmlValue(DataSet ds, XElement element, string fatherCol, int fatherid)
	{
		try
		{
			DataTable dataTable = ds.Tables[element.Name.LocalName];
			DataRow dataRow = dataTable.NewRow();
			string colName = $"{element.Name.LocalName}_Id";
			int num = 0;
			if (dataTable.Rows.Count > 0)
			{
				num = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t[colName])) + 1;
			}
			dataRow[colName] = num;
			if (!string.IsNullOrEmpty(fatherCol))
			{
				dataRow[fatherCol] = fatherid;
			}
			dataTable.Rows.Add(dataRow);
			foreach (XElement item in element.Elements())
			{
				if (item != null)
				{
					if (item.HasElements)
					{
						LoadXmlValue(ds, item, colName, num);
					}
					else if (!item.IsEmpty)
					{
						dataRow[item.Name.LocalName] = item.Value;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw ex;
		}
	}

	public virtual async Task<HttpResult> AddNew(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts)
	{
		try
		{
			string text = "";
			if (doc.Udos != null && doc.Udos.Count > 0)
			{
				foreach (DocumentUdo udo2 in doc.Udos)
				{
					if (udo2.dsChild.Tables.Count > 0 && udo2.UdoTable.IsNotNull())
					{
						int udoEntry = GetUdoEntry(doc, udo2, "OPMT", "@OPMT", "U_paymentEntry");
						DataTable udo = GetUdo("OPMT");
						if (udo != null && udo.Rows.Count > 0)
						{
							text = ((udoEntry != 0) ? (text + _unitOfWork.UDOs.MappingXml(udo, udo2.dtFather, udo2.dsChild, sessionId, "Update")) : (text + _unitOfWork.UDOs.MappingXml(udo, udo2.dtFather, udo2.dsChild, sessionId, "Add")));
						}
					}
				}
			}
			string text2 = Mapping(doc, EnumCommandType.Add, objType, docObjectCode);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text2 + "</dis:AddObject>" + text + "</env:Body></env:Envelopes>";
			Logging.Write("WATCH", $"Start add new\t{objType}");
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Add new document\t{objType}\t{num}");
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	protected DataTable GetUdo(string code)
	{
		try
		{
			return ServiceUnitOfWork.Commons.ExecuteDataTable("Select * from OUDO where \"Code\"=@code;", CommandType.Text, new SqlParameter("@code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected int GetUdoEntry(DocumentsImpl doc, DocumentUdo udo, string udoCode, string udoTableName, string udfFieldName)
	{
		if (Function.ToString(udo.UdoTable.Rows[0]["Code"]).Equals(udoCode))
		{
			int num = 0;
			UserFieldsImpl userFieldsImpl = doc.UserFields.FirstOrDefault((UserFieldsImpl t) => t.Name == udfFieldName);
			if (userFieldsImpl != null)
			{
				num = Function.ParseInt(userFieldsImpl.Value);
			}
			if (num == 0)
			{
				DataTable dataTable = ServiceUnitOfWork.Commons.ExecuteDataTable(GetUdoEntryQuery(udoTableName), CommandType.Text);
				if (dataTable.IsNotNull())
				{
					num = Function.ParseInt(dataTable.Rows[0][0]);
				}
				if (num != 0)
				{
					doc.UserFields.Remove(doc.UserFields.FirstOrDefault((UserFieldsImpl t) => t.Name == udfFieldName));
				}
				UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
				userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
				userFieldsImpl2.Name = udfFieldName;
				userFieldsImpl2.Value = num;
				doc.UserFields.Add(userFieldsImpl2);
				return 0;
			}
			return num;
		}
		return 0;
	}

	protected virtual string GetUdoEntryQuery(string udoTableName)
	{
		return "SELECT ISNULL((SELECT MAX(DocEntry) FROM [" + udoTableName + "]),0)+1 ID";
	}

	public string MappingService(DocumentsImpl doc, EnumCommandType commandType, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts)
	{
		try
		{
			string text = "<Document xmlns=\"http://www.sap.com/SBO/DIS\">";
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<DocEntry>" + Function.ParseInt(doc.DocEntry) + "</DocEntry>";
				text = text + "<DocNum>" + Function.ParseInt(doc.DocNum) + "</DocNum>";
			}
			text += "<DocType>dDocument_Items</DocType>\r\n                         <HandWritten>tNO</HandWritten>";
			if (!string.IsNullOrEmpty(doc.Printed))
			{
				text = text + "<Printed>" + ((doc.Printed == "N") ? "psNo" : "psYes") + "</Printed>";
			}
			if (objType == ApzObjectTypes.oDrafts)
			{
				text = text + "<DocObjectCode>" + Function.ToString((int)docObjectCode) + "</DocObjectCode>";
			}
			if (!doc.SlpCode.Equals(0))
			{
				text = text + "<SalesPersonCode>" + Function.ParseInt(doc.SlpCode) + "</SalesPersonCode>";
			}
			if (!string.IsNullOrEmpty(doc.Comments))
			{
				text = text + "<Comments>" + Function.RawText(doc.Comments) + "</Comments>";
			}
			text = text + "<ContactPersonCode>" + Function.ParseInt(doc.CntctCode) + "</ContactPersonCode>";
			text = ((doc.AtcEntry == 0 || doc.AttachmentFiles == null || doc.AttachmentFiles.Count <= 0) ? (text + "<AttachmentEntry>null</AttachmentEntry>") : (text + "<AttachmentEntry>" + Function.ParseInt(doc.AtcEntry) + "</AttachmentEntry>"));
			string text2 = Function.ToString(doc.DocStatus).ToUpper();
			if (((objType == ApzObjectTypes.oDrafts || objType == ApzObjectTypes.oOrders || objType == ApzObjectTypes.oQuotations || objType == ApzObjectTypes.oPurchaseOrders || objType == ApzObjectTypes.oPurchaseQuotations || objType == ApzObjectTypes.oPurchaseRequest) && (text2.Equals("O") || text2.Equals("OPEN"))) || commandType == EnumCommandType.Add)
			{
				if (!string.IsNullOrEmpty(doc.Address))
				{
					text = text + "<Address>" + Function.RawText(doc.Address) + "</Address>";
				}
				if ((!string.IsNullOrEmpty(doc.isIns) && new ApzObjectTypes[4]
				{
					ApzObjectTypes.oInvoices,
					ApzObjectTypes.oPurchaseInvoices,
					ApzObjectTypes.oReserveInvoices,
					ApzObjectTypes.oAPReserveInvoice
				}.Contains(objType)) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[4]
				{
					ApzObjectTypes.oInvoices,
					ApzObjectTypes.oPurchaseInvoices,
					ApzObjectTypes.oReserveInvoices,
					ApzObjectTypes.oAPReserveInvoice
				}.Contains(docObjectCode)))
				{
					text = text + "<ReserveInvoice>" + Function.RawText(doc.isIns) + "</ReserveInvoice>";
				}
				if (!string.IsNullOrEmpty(doc.Address2))
				{
					text = text + "<Address2>" + Function.RawText(doc.Address2) + "</Address2>";
				}
				if (!string.IsNullOrEmpty(doc.AgentCode))
				{
					text = text + "<AgentCode>" + Function.RawText(doc.AgentCode) + "</AgentCode>";
				}
				if (doc.AnnualInvoiceDeclarationReference != 0)
				{
					text = text + "<AnnualInvoiceDeclarationReference>" + Function.ParseInt(doc.AnnualInvoiceDeclarationReference) + "</AnnualInvoiceDeclarationReference>";
				}
				if (!string.IsNullOrEmpty(doc.BPChCode))
				{
					text = text + "<BPChannelCode>" + Function.RawText(doc.BPChCode) + "</BPChannelCode>";
				}
				if (doc.BPChCntc != 0)
				{
					text = text + "<BPChannelContact>" + Function.ParseInt(doc.BPChCntc) + "</BPChannelContact>";
				}
				if (doc.BPLId != 0 && doc.BPLId != -1)
				{
					text = text + "<BPL_IDAssignedToInvoice>" + Function.ParseInt(doc.BPLId) + "</BPL_IDAssignedToInvoice>";
				}
				if (doc.BlanketAgreementNumber != 0)
				{
					text = text + "<BlanketAgreementNumber>" + Function.ParseInt(doc.BlanketAgreementNumber) + "</BlanketAgreementNumber>";
				}
				if (!string.IsNullOrEmpty(doc.Box1099))
				{
					text = text + "<Box1099>" + Function.RawText(doc.Box1099) + "</Box1099>";
				}
				if (!string.IsNullOrEmpty(doc.CardCode))
				{
					text = text + "<CardCode>" + Function.RawText(doc.CardCode) + "</CardCode>";
				}
				if (!string.IsNullOrEmpty(doc.CardName))
				{
					text = text + "<CardName>" + Function.RawText(doc.CardName) + "</CardName>";
				}
				if (doc.CdcOffset != 0)
				{
					text = text + "<CashDiscountDateOffset>" + Function.ParseInt(doc.CdcOffset) + "</CashDiscountDateOffset>";
				}
				if (!string.IsNullOrEmpty(doc.CntrlBnk))
				{
					text = text + "<CentralBankIndicator>" + Function.RawText(doc.CntrlBnk) + "</CentralBankIndicator>";
				}
				if (!string.IsNullOrEmpty(doc.Footer))
				{
					text = text + "<ClosingRemarks>" + Function.RawText(doc.Footer) + "</ClosingRemarks>";
				}
				if (!string.IsNullOrEmpty(doc.CtlAccount))
				{
					text = text + "<ControlAccount>" + Function.RawText(doc.CtlAccount) + "</ControlAccount>";
				}
				if (!doc.DiscPrcnt.Equals(0.0))
				{
					text = text + "<DiscountPercent>" + Function.ParseDecimal(doc.DiscPrcnt) + "</DiscountPercent>";
				}
				if (!string.IsNullOrEmpty(doc.DocCur))
				{
					text = text + "<DocCurrency>" + Function.RawText(doc.DocCur) + "</DocCurrency>";
				}
				if (doc.DocDate > new DateTime(1900, 1, 1))
				{
					text = text + "<DocDate>" + Convert.ToDateTime(doc.DocDate).ToString("yyyy-MM-dd") + "</DocDate>";
				}
				if (doc.DocDueDate > new DateTime(1900, 1, 1))
				{
					text = text + "<DocDueDate>" + Convert.ToDateTime(doc.DocDueDate).ToString("yyyy-MM-dd") + "</DocDueDate>";
				}
				if (!doc.DocRate.Equals(0.0))
				{
					text = text + "<DocRate>" + Function.ParseDecimal(doc.DocRate) + "</DocRate>";
				}
				if (!string.IsNullOrEmpty(doc.DocType))
				{
					text = ((!(doc.DocType == "S")) ? (text + "<DocType>dDocument_Items</DocType>") : (text + "<DocType>dDocument_Service</DocType>"));
				}
				if (doc.OwnerCode > 0)
				{
					text = text + "<DocumentsOwner>" + Function.ParseInt(doc.OwnerCode) + "</DocumentsOwner>";
				}
				if (!doc.DownPayment.Equals(0.0))
				{
					text = text + "<DownPayment>" + Function.ParseDecimal(doc.DownPayment) + "</DownPayment>";
				}
				if (!doc.DpmAmnt.Equals(0.0))
				{
					text = text + "<DownPaymentAmount>" + Function.ParseDecimal(doc.DpmAmnt) + "</DownPaymentAmount>";
				}
				if (!doc.DpmAmntFC.Equals(0.0))
				{
					text = text + "<DownPaymentAmountFC>" + Function.ParseDecimal(doc.DpmAmntFC) + "</DownPaymentAmountFC>";
				}
				if (!doc.DpmAmntSC.Equals(0.0))
				{
					text = text + "<DownPaymentAmountSC>" + Function.ParseDecimal(doc.DpmAmntSC) + "</DownPaymentAmountSC>";
				}
				if (!doc.DpmPrcnt.Equals(0.0))
				{
					text = text + "<DownPaymentPercentage>" + Function.ParseDecimal(doc.DpmPrcnt) + "</DownPaymentPercentage>";
				}
				if (doc.TaxDate > new DateTime(1900, 1, 1))
				{
					text = text + "<TaxDate>" + Convert.ToDateTime(doc.TaxDate).ToString("yyyy-MM-dd") + "</TaxDate>";
				}
				if (!doc.TrnspCode.Equals(0))
				{
					text = text + "<TransportationCode>" + Function.ParseInt(doc.TrnspCode) + "</TransportationCode>";
				}
				if (doc.VatDate > new DateTime(1900, 1, 1))
				{
					text = text + "<VatDate>" + Convert.ToDateTime(doc.VatDate).ToString("yyyy-MM-dd") + "</VatDate>";
				}
				if (!doc.VatPercent.Equals(0.0))
				{
					text = text + "<VatPercent>" + Function.ParseDecimal(doc.VatPercent) + "</VatPercent>";
				}
				if (objType == ApzObjectTypes.oPurchaseRequest || docObjectCode == ApzObjectTypes.oPurchaseRequest)
				{
					if (!string.IsNullOrEmpty(doc.Requester))
					{
						text = text + "<Requester>" + Function.ToString(doc.Requester) + "</Requester>";
					}
					if (!string.IsNullOrEmpty(doc.Email))
					{
						text = text + "<RequesterEmail>" + Function.ToString(doc.Email) + "</RequesterEmail>";
					}
					if (!string.IsNullOrEmpty(doc.ReqName))
					{
						text = text + "<RequesterName>" + Function.ToString(doc.ReqName) + "</RequesterName>";
					}
					if (!Function.ParseInt(doc.Branch).Equals(0))
					{
						text = text + "<RequesterBranch>" + Function.ParseInt(doc.Branch) + "</RequesterBranch>";
					}
					if (!Function.ParseInt(doc.Department).Equals(0))
					{
						text = text + "<RequesterDepartment>" + Function.ParseInt(doc.Department) + "</RequesterDepartment>";
					}
					if (!Function.ParseInt(doc.ReqType).Equals(0))
					{
						text = text + "<ReqType>" + Function.ParseInt(doc.ReqType) + "</ReqType>";
					}
				}
				if (doc.ReqDate > new DateTime(1900, 1, 1))
				{
					text = text + "<RequriedDate>" + Convert.ToDateTime(doc.ReqDate).ToString("yyyy-MM-dd") + "</RequriedDate>";
				}
				if (!string.IsNullOrEmpty(doc.ShipToCode))
				{
					text = text + "<ShipToCode>" + Function.RawText(doc.ShipToCode) + "</ShipToCode>";
				}
				if (!string.IsNullOrEmpty(doc.PayToCode))
				{
					text = text + "<PayToCode>" + Function.RawText(doc.PayToCode) + "</PayToCode>";
				}
			}
			if (objType != ApzObjectTypes.oInventoryGenEntry && objType != ApzObjectTypes.oInventoryGenExit)
			{
				if (!string.IsNullOrEmpty(doc.JrnlMemo))
				{
					text = text + "<JournalMemo>" + Function.RawText(doc.JrnlMemo) + "</JournalMemo>";
				}
				if (!string.IsNullOrEmpty(doc.Project))
				{
					text = text + "<Project>" + Function.RawText(doc.Project) + "</Project>";
				}
				if (!string.IsNullOrEmpty(doc.Indicator))
				{
					text = text + "<Indicator>" + Function.RawText(doc.Indicator) + "</Indicator>";
				}
				if (!string.IsNullOrEmpty(doc.LicTradNum))
				{
					text = text + "<FederalTaxID>" + Function.RawText(doc.LicTradNum) + "</FederalTaxID>";
				}
				if (text2.Equals("O") || text2.Equals("OPEN"))
				{
					if (Function.ParseInt(doc.GroupNum) > 0)
					{
						text = text + "<PaymentGroupCode>" + Function.RawText(doc.GroupNum) + "</PaymentGroupCode>";
					}
					if (!string.IsNullOrEmpty(doc.PeyMethod))
					{
						text = text + "<PaymentMethod>" + Function.RawText(doc.PeyMethod) + "</PaymentMethod>";
					}
					if (commandType == EnumCommandType.Add || objType == ApzObjectTypes.oDrafts)
					{
						if (Function.ParseInt(doc.ExtraMonth) > 0)
						{
							text = text + "<ExtraMonth>" + Function.RawText(doc.ExtraMonth) + "</ExtraMonth>";
							BoPayTermDueTypes boPayTermDueTypes;
							try
							{
								boPayTermDueTypes = doc.StartFrom;
							}
							catch
							{
								boPayTermDueTypes = BoPayTermDueTypes.pdt_None;
							}
							text = text + "<StartFrom>" + Function.RawText(boPayTermDueTypes) + "</StartFrom>";
						}
						if (Function.ParseInt(doc.ExtraDays) > 0)
						{
							text = text + "<ExtraDays>" + Function.RawText(doc.ExtraDays) + "</ExtraDays>";
						}
					}
					if (Function.ParseInt(doc.CdcOffset) > 0)
					{
						text = text + "<CashDiscountDateOffset>" + Function.RawText(doc.CdcOffset) + "</CashDiscountDateOffset>";
					}
				}
			}
			text += DataService.MappingUdf(doc.UserFields, commandType);
			if (doc.Lines != null && doc.Lines.Count > 0)
			{
				text += "<DocumentLines>";
				for (int i = 0; i < doc.Lines.Count; i++)
				{
					Document_LinesImpl document_LinesImpl = doc.Lines[i];
					string text3 = Function.ToString(document_LinesImpl.LineStatus).ToUpper();
					text += "<DocumentLine>";
					if (((objType == ApzObjectTypes.oDrafts || objType == ApzObjectTypes.oOrders || objType == ApzObjectTypes.oQuotations || objType == ApzObjectTypes.oPurchaseOrders || objType == ApzObjectTypes.oPurchaseQuotations || objType == ApzObjectTypes.oPurchaseRequest) && (text2.Equals("O") || text2.Equals("OPEN")) && (text3.Equals("O") || text3.Equals("OPEN"))) || commandType == EnumCommandType.Add)
					{
						if (!string.IsNullOrEmpty(document_LinesImpl.AcctCode))
						{
							text = text + "<AccountCode>" + Function.RawText(document_LinesImpl.AcctCode) + "</AccountCode>";
						}
						if (!document_LinesImpl.ActBaseEnt.Equals(0))
						{
							text = text + "<ActualBaseEntry>" + Function.ParseInt(document_LinesImpl.ActBaseEnt) + "</ActualBaseEntry>";
						}
						if (document_LinesImpl.ActDelDate > new DateTime(1900, 1, 1))
						{
							text = text + "<ActualDeliveryDate>" + Convert.ToDateTime(document_LinesImpl.ActDelDate).ToString("yyyy-MM-dd") + "</ActualDeliveryDate>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Address))
						{
							text = text + "<Address>" + Function.RawText(document_LinesImpl.Address) + "</Address>";
						}
						if (!document_LinesImpl.AgrNo.Equals(0))
						{
							text = text + "<AgreementNo>" + Function.ParseInt(document_LinesImpl.AgrNo) + "</AgreementNo>";
						}
						if (!document_LinesImpl.AgrLnNum.Equals(0))
						{
							text = text + "<AgreementRowNumber>" + Function.ParseInt(document_LinesImpl.AgrLnNum) + "</AgreementRowNumber>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.CodeBars))
						{
							text = text + "<BarCode>" + Function.RawText(document_LinesImpl.CodeBars) + "</BarCode>";
						}
						if (!document_LinesImpl.BaseType.Equals(0) && !document_LinesImpl.BaseType.Equals(-1))
						{
							text = text + "<BaseType>" + Function.ParseInt(document_LinesImpl.BaseType) + "</BaseType>";
							if (!document_LinesImpl.BaseEntry.Equals(0))
							{
								text = text + "<BaseEntry>" + Function.ParseInt(document_LinesImpl.BaseEntry) + "</BaseEntry>";
							}
							if (!document_LinesImpl.BaseLine.Equals(-1))
							{
								text = text + "<BaseLine>" + Function.ParseInt(document_LinesImpl.BaseLine) + "</BaseLine>";
							}
						}
						if (!document_LinesImpl.Commission.Equals(0.0) || commandType == EnumCommandType.Update)
						{
							text = text + "<CommisionPercent>" + Function.ParseDecimal(document_LinesImpl.Commission) + "</CommisionPercent>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode) || commandType == EnumCommandType.Update)
						{
							text = text + "<CostingCode>" + Function.RawText(document_LinesImpl.OcrCode) + "</CostingCode>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode2) || commandType == EnumCommandType.Update)
						{
							text = text + "<CostingCode2>" + Function.RawText(document_LinesImpl.OcrCode2) + "</CostingCode2>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode3) || commandType == EnumCommandType.Update)
						{
							text = text + "<CostingCode3>" + Function.RawText(document_LinesImpl.OcrCode3) + "</CostingCode3>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode4) || commandType == EnumCommandType.Update)
						{
							text = text + "<CostingCode4>" + Function.RawText(document_LinesImpl.OcrCode4) + "</CostingCode4>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode5) || commandType == EnumCommandType.Update)
						{
							text = text + "<CostingCode5>" + Function.RawText(document_LinesImpl.OcrCode5) + "</CostingCode5>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Currency))
						{
							text = text + "<Currency>" + Function.RawText(document_LinesImpl.Currency) + "</Currency>";
						}
						if (!document_LinesImpl.DiscPrcnt.Equals(0.0) || commandType == EnumCommandType.Update)
						{
							text = text + "<DiscountPercent>" + Function.ParseDecimal(document_LinesImpl.DiscPrcnt) + "</DiscountPercent>";
						}
						if (!document_LinesImpl.ExciseAmt.Equals(0.0))
						{
							text = text + "<ExciseAmount>" + Function.ParseDecimal(document_LinesImpl.ExciseAmt) + "</ExciseAmount>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.ItemCode) || commandType == EnumCommandType.Update)
						{
							text = text + "<ItemCode>" + Function.RawText(document_LinesImpl.ItemCode) + "</ItemCode>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Dscription) || commandType == EnumCommandType.Update)
						{
							text = text + "<ItemDescription>" + Function.RawText(document_LinesImpl.Dscription) + "</ItemDescription>";
						}
						if (!document_LinesImpl.InvQty.Equals(0.0))
						{
							text = text + "<InventoryQuantity>" + Function.ParseDecimal(document_LinesImpl.InvQty) + "</InventoryQuantity>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Text) || commandType == EnumCommandType.Update)
						{
							text = text + "<ItemDetails>" + Function.RawText(document_LinesImpl.Text) + "</ItemDetails>";
						}
						if (document_LinesImpl.LineNum >= 0)
						{
							text = text + "<LineNum>" + Function.ParseInt(document_LinesImpl.LineNum) + "</LineNum>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.VendorNum) || commandType == EnumCommandType.Update)
						{
							text = text + "<VendorNum>" + Function.RawText(document_LinesImpl.VendorNum) + "</VendorNum>";
						}
						if (!document_LinesImpl.LocCode.Equals(0))
						{
							text = text + "<LocationCode>" + Function.ParseInt(document_LinesImpl.LocCode) + "</LocationCode>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.unitMsr) && document_LinesImpl.BaseType <= 0)
						{
							text = text + "<MeasureUnit>" + Function.RawText(document_LinesImpl.unitMsr) + "</MeasureUnit>";
						}
						if (!document_LinesImpl.PriceBefDi.Equals(0.0) || commandType == EnumCommandType.Update)
						{
							text = text + "<UnitPrice>" + Function.ParseDecimal(document_LinesImpl.PriceBefDi) + "</UnitPrice>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Project) || commandType == EnumCommandType.Update)
						{
							text = text + "<ProjectCode>" + Function.RawText(document_LinesImpl.Project) + "</ProjectCode>";
						}
						if (!document_LinesImpl.Quantity.Equals(0.0) || commandType == EnumCommandType.Update)
						{
							text = text + "<Quantity>" + Function.ParseDecimal(document_LinesImpl.Quantity) + "</Quantity>";
						}
						if (!document_LinesImpl.Rate.Equals(0.0) && (commandType == EnumCommandType.Add || (commandType == EnumCommandType.Update && (document_LinesImpl.TreeType == "N" || (document_LinesImpl.TreeType == "I" && document_LinesImpl.BOMTreeType != "0" && Function.ParseInt(document_LinesImpl.BOMTreeType) == 0)))))
						{
							text = text + "<Rate>" + Function.ParseDecimal(document_LinesImpl.Rate) + "</Rate>";
						}
						if (document_LinesImpl.PQTReqDate > new DateTime(1900, 1, 1))
						{
							text = text + "<RequiredDate>" + Convert.ToDateTime(document_LinesImpl.PQTReqDate).ToString("yyyy-MM-dd") + "</RequiredDate>";
						}
						if (!document_LinesImpl.PQTReqQty.Equals(0.0))
						{
							text = text + "<RequiredQuantity>" + Function.ParseDecimal(document_LinesImpl.PQTReqQty) + "</RequiredQuantity>";
						}
						if (!document_LinesImpl.PackQty.Equals(0.0) || commandType == EnumCommandType.Update)
						{
							text = text + "<PackageQuantity>" + Function.ParseDecimal(document_LinesImpl.PackQty) + "</PackageQuantity>";
						}
						if (!doc.SlpCode.Equals(0))
						{
							text = text + "<SalesPersonCode>" + Function.ParseInt(doc.SlpCode) + "</SalesPersonCode>";
						}
						if (!document_LinesImpl.SlpCode.Equals(0))
						{
							text = text + "<SalesPersonCode>" + Function.ParseInt(document_LinesImpl.SlpCode) + "</SalesPersonCode>";
						}
						_ = document_LinesImpl.ShipDate;
						if (document_LinesImpl.ShipDate > new DateTime(1900, 1, 1))
						{
							text = text + "<ShipDate>" + Convert.ToDateTime(document_LinesImpl.ShipDate).ToString("yyyy-MM-dd") + "</ShipDate>";
						}
						if (!document_LinesImpl.TrnsCode.Equals(0))
						{
							text = text + "<ShippingMethod>" + Function.ParseInt(document_LinesImpl.TrnsCode) + "</ShippingMethod>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.ShipToCode) || commandType == EnumCommandType.Update)
						{
							text = text + "<ShipToCode>" + Function.RawText(document_LinesImpl.ShipToCode) + "</ShipToCode>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.ShipToDesc) || commandType == EnumCommandType.Update)
						{
							text = text + "<ShipToDescription>" + Function.RawText(document_LinesImpl.ShipToDesc) + "</ShipToDescription>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.TaxCode))
						{
							text = text + "<TaxCode>" + Function.RawText(document_LinesImpl.TaxCode) + "</TaxCode>";
						}
						text = ((!(document_LinesImpl.NumPerMsr > 0.0)) ? (text + "<UnitsOfMeasurment>1</UnitsOfMeasurment>") : (text + "<UnitsOfMeasurment>" + document_LinesImpl.NumPerMsr + "</UnitsOfMeasurment>"));
						if (!string.IsNullOrEmpty(document_LinesImpl.UseBaseUn) && new string[4] { "N", "Y", "tYES", "tNO" }.Contains(document_LinesImpl.UseBaseUn))
						{
							text = text + "<UseBaseUnits>" + Function.RawText(document_LinesImpl.UseBaseUn) + "</UseBaseUnits>";
						}
						if (!document_LinesImpl.UomEntry.Equals(0))
						{
							text = text + "<UoMEntry>" + Function.ParseInt(document_LinesImpl.UomEntry) + "</UoMEntry>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.VatGroup))
						{
							text = text + "<VatGroup>" + Function.RawText(document_LinesImpl.VatGroup) + "</VatGroup>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.WhsCode))
						{
							text = text + "<WarehouseCode>" + Function.RawText(document_LinesImpl.WhsCode) + "</WarehouseCode>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.SubCatNum) || commandType == EnumCommandType.Update)
						{
							text = text + "<SupplierCatNum>" + Function.ToString(document_LinesImpl.SubCatNum) + "</SupplierCatNum>";
						}
						if (new ApzObjectTypes[2]
						{
							ApzObjectTypes.oCreditNotes,
							ApzObjectTypes.oPurchaseCreditNotes
						}.Contains(objType) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[2]
						{
							ApzObjectTypes.oCreditNotes,
							ApzObjectTypes.oPurchaseCreditNotes
						}.Contains(docObjectCode)))
						{
							text += ((document_LinesImpl.NoInvtryMv == "Y") ? "<WithoutInventoryMovement>tYES</WithoutInventoryMovement>" : "<WithoutInventoryMovement>tNO</WithoutInventoryMovement>");
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.TreeType) && !"0".Equals(document_LinesImpl.TreeType))
						{
							text = text + "<TreeType>" + Function.ToString(document_LinesImpl.TreeType) + "</TreeType>";
						}
					}
					if ((text2.Equals("O") || text2.Equals("OPEN") || text3.Equals("OPEN") || text3.Equals("O")) && !string.IsNullOrEmpty(document_LinesImpl.FreeTxt))
					{
						text = text + "<FreeText>" + Function.RawText(document_LinesImpl.FreeTxt) + "</FreeText>";
					}
					if (objType == ApzObjectTypes.oInventoryGenEntry && doc.Lines.FirstOrDefault((Document_LinesImpl t) => t.BaseType == 202) != null && !doc.Lines.Any((Document_LinesImpl t) => t.BaseLine >= 0))
					{
						text = text + "<TransactionType>" + ((document_LinesImpl.TranType == "R") ? "botrntReject" : "botrntComplete") + "</TransactionType>";
					}
					text += DataService.MappingUdf(document_LinesImpl.UserFields, commandType);
					text += "</DocumentLine>";
				}
				text += "</DocumentLines>";
				return text + "</Document>";
			}
			if (doc.Expenses != null && doc.Expenses.Count > 0)
			{
				text += "<DocumentsAdditionalExpenses>";
				for (int num = 0; num < doc.Expenses.Count; num++)
				{
					text += "<row>";
					DocumentsAdditionalExpensesImpl documentsAdditionalExpensesImpl = doc.Expenses[num];
					if (documentsAdditionalExpensesImpl != null && !string.IsNullOrEmpty(documentsAdditionalExpensesImpl.Status) && documentsAdditionalExpensesImpl.Status.ToUpper().Equals("O"))
					{
						if (!Function.ParseInt(documentsAdditionalExpensesImpl.ExpnsCode).Equals(0))
						{
							text = text + "<ExpenseCode>" + Function.ParseInt(documentsAdditionalExpensesImpl.ExpnsCode) + "</ExpenseCode>";
						}
						if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.LineTotal).Equals(0m))
						{
							text = text + "<LineTotal>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.LineTotal) + "</LineTotal>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.Comments))
						{
							text = text + "<Remarks>" + Function.RawText(documentsAdditionalExpensesImpl.Comments) + "</Remarks>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.DistrbMthd))
						{
							text = text + "<DistributionMethod>" + Function.RawText(documentsAdditionalExpensesImpl.DistrbMthd) + "</DistributionMethod>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.VatGroup))
						{
							text = text + "<VatGroup>" + Function.RawText(documentsAdditionalExpensesImpl.VatGroup) + "</VatGroup>";
						}
						if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.VatPrcnt).Equals(0m))
						{
							text = text + "<TaxPercent>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.VatPrcnt) + "</TaxPercent>";
						}
						if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSum).Equals(0m))
						{
							text = text + "<TaxSum>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSum) + "</TaxSum>";
						}
						if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.DedVatSum).Equals(0m))
						{
							text = text + "<DeductibleTaxSum>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.DedVatSum) + "</DeductibleTaxSum>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.TaxCode))
						{
							text = text + "<TaxCode>" + Function.RawText(documentsAdditionalExpensesImpl.TaxCode) + "</TaxCode>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.TaxType))
						{
							text = text + "<TaxType>" + Function.RawText(documentsAdditionalExpensesImpl.TaxType) + "</TaxType>";
						}
						if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatPer).Equals(0m))
						{
							text = text + "<EqualizationTaxPercent>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatPer) + "</EqualizationTaxPercent>";
						}
						if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatSum).Equals(0m))
						{
							text = text + "<EqualizationTaxSum>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatSum) + "</EqualizationTaxSum>";
						}
						if (!Function.ParseInt(documentsAdditionalExpensesImpl.BaseType).Equals(0) && !Function.ParseInt(documentsAdditionalExpensesImpl.BaseType).Equals(-1))
						{
							text = text + "<BaseDocType>" + Function.ParseInt(documentsAdditionalExpensesImpl.BaseType) + "</BaseDocType>";
							if (!Function.ParseInt(documentsAdditionalExpensesImpl.BaseAbsEnt).Equals(0))
							{
								text = text + "<BaseDocEntry>" + Function.ParseInt(documentsAdditionalExpensesImpl.BaseAbsEnt) + "</BaseDocEntry>";
							}
							text = text + "<BaseDocLine>" + Function.ParseInt(documentsAdditionalExpensesImpl.BaseLnNum) + "</BaseDocLine>";
						}
						text += (Function.ParseBool(documentsAdditionalExpensesImpl.LstPchPrce) ? "<LastPurchasePrice>tYES</LastPurchasePrice>" : "<LastPurchasePrice>tNO</LastPurchasePrice>");
						text += (Function.ParseBool(documentsAdditionalExpensesImpl.Stock) ? "<Stock>tYES</Stock>" : "<Stock>tNO</Stock>");
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode))
						{
							text = text + "<DistributionRule>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode) + "</DistributionRule>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode2))
						{
							text = text + "<DistributionRule2>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode2) + "</DistributionRule2>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode3))
						{
							text = text + "<DistributionRule3>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode3) + "</DistributionRule3>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode4))
						{
							text = text + "<DistributionRule4>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode4) + "</DistributionRule4>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode5))
						{
							text = text + "<DistributionRule5>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode5) + "</DistributionRule5>";
						}
						if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.Project))
						{
							text = text + "<Project>" + Function.RawText(documentsAdditionalExpensesImpl.Project) + "</Project>";
						}
					}
					text += "</row>";
				}
				text += "</DocumentsAdditionalExpenses>";
			}
			return text + "</Document>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public virtual string Mapping(DocumentsImpl doc, EnumCommandType commandType, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts, bool ignoreUpdateEmptyValue = false)
	{
		string text = "";
		try
		{
			if (!MainCurrency.ContainsKey(_connectionString))
			{
				lock (MainCurrency)
				{
					if (!MainCurrency.ContainsKey(_connectionString))
					{
						MainCurrency.Add(_connectionString, ServiceUnitOfWork.GeneralSetting.GetMainCurrency());
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		text = MainCurrency[_connectionString];
		string text2 = "<BOM>\r\n                          <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                          <AdmInfo>\r\n                            <Object>" + objType.ToString() + "</Object>\r\n                          </AdmInfo>";
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				text2 = text2 + "<QueryParams><DocEntry>" + Function.ParseInt(doc.DocEntry) + "</DocEntry></QueryParams>";
			}
			text2 += " <Documents>\r\n                            <row>";
			if (objType == ApzObjectTypes.oDrafts)
			{
				text2 = text2 + "<DocObjectCode>" + Function.ToString((int)docObjectCode) + "</DocObjectCode>";
				if (doc.DocNum > 0)
				{
					text2 = text2 + "<DocNum>" + doc.DocNum + "</DocNum>";
				}
			}
			if (new ApzObjectTypes[2]
			{
				ApzObjectTypes.oDownPayments,
				ApzObjectTypes.oPurchaseDownPayments
			}.Contains(objType) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[2]
			{
				ApzObjectTypes.oDownPayments,
				ApzObjectTypes.oPurchaseDownPayments
			}.Contains(docObjectCode)))
			{
				text2 = text2 + "<DownPaymentType>" + ((Enums.DownPaymentType)Function.ParseInt(doc.Posted)/*cast due to constrained. prefix*/).ToString() + "</DownPaymentType>";
			}
			if (!doc.SlpCode.Equals(0))
			{
				text2 = text2 + "<SalesPersonCode>" + Function.ParseInt(doc.SlpCode) + "</SalesPersonCode>";
			}
			text2 = text2 + "<Comments>" + Function.RawText(doc.Comments) + "</Comments>";
			text2 = text2 + "<OpeningRemarks>" + Function.RawText(doc.Header) + "</OpeningRemarks>";
			text2 = text2 + "<ClosingRemarks>" + Function.RawText(doc.Footer) + "</ClosingRemarks>";
			if (objType != ApzObjectTypes.oInventoryGenExit && objType != ApzObjectTypes.oInventoryGenEntry)
			{
				text2 += $"<ContactPersonCode>{doc.CntctCode}</ContactPersonCode>";
			}
			ApzObjectTypes[] source = new ApzObjectTypes[6]
			{
				ApzObjectTypes.oPurchaseRequest,
				ApzObjectTypes.oInventoryGenEntry,
				ApzObjectTypes.oInventoryGenExit,
				ApzObjectTypes.oStockTransfer,
				ApzObjectTypes.oStockTransferDraft,
				ApzObjectTypes.oInventoryTransferRequest
			};
			if (!source.Contains(objType) || (objType == ApzObjectTypes.oDrafts && !source.Contains(doc.DocObjectCode)))
			{
				text2 = text2 + "<NumAtCard>" + Function.RawText(doc.NumAtCard) + "</NumAtCard>";
			}
			if (doc.AtcEntry != 0 && doc.AttachmentFiles != null && doc.AttachmentFiles.Count > 0)
			{
				text2 = text2 + "<AttachmentEntry>" + Function.ParseInt(doc.AtcEntry) + "</AttachmentEntry>";
			}
			if (!string.IsNullOrEmpty(doc.Ref1))
			{
				text2 = text2 + "<Reference1>" + Function.RawText(doc.Ref1) + "</Reference1>";
			}
			if (!string.IsNullOrEmpty(doc.Ref2))
			{
				text2 = text2 + "<Reference2>" + Function.RawText(doc.Ref2) + "</Reference2>";
			}
			text2 = ((!(doc.PartSupply == "N")) ? (text2 + "<PartialSupply>tYES</PartialSupply>") : (text2 + "<PartialSupply>tNO</PartialSupply>"));
			if (!string.IsNullOrEmpty(doc.PriceMode))
			{
				text2 = text2 + "<PriceMode>" + ((doc.PriceMode == "N") ? "pmdNet" : "pmdGross") + "</PriceMode>";
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(doc.OpenForLaC)) && (objType == ApzObjectTypes.oPurchaseInvoices || objType == ApzObjectTypes.oPurchaseDeliveryNotes))
			{
				text2 = text2 + "<OpenForLandedCosts>" + ((doc.OpenForLaC == "N") ? Function.ToString(BoYesNoEnum.tNO) : Function.ToString(BoYesNoEnum.tYES)) + "</OpenForLandedCosts>";
			}
			string text3 = Function.ToString(doc.DocStatus).ToUpper();
			bool flag = ((objType == ApzObjectTypes.oDrafts || objType == ApzObjectTypes.oOrders || ((objType == ApzObjectTypes.oDownPayments || objType == ApzObjectTypes.oPurchaseDownPayments) && Function.ParseInt(doc.Posted) == 0) || objType == ApzObjectTypes.oQuotations || objType == ApzObjectTypes.oPurchaseOrders || objType == ApzObjectTypes.oPurchaseQuotations || objType == ApzObjectTypes.oPurchaseRequest) && (text3.Equals("O") || text3.Equals("OPEN"))) || commandType == EnumCommandType.Add;
			if (flag)
			{
				if (objType != ApzObjectTypes.oPurchaseRequest || (objType == ApzObjectTypes.oDrafts && docObjectCode != ApzObjectTypes.oPurchaseRequest))
				{
					text2 = text2 + "<Address>" + Function.RawText(doc.Address) + "</Address>";
				}
				if ((!string.IsNullOrEmpty(doc.isIns) && new ApzObjectTypes[4]
				{
					ApzObjectTypes.oInvoices,
					ApzObjectTypes.oPurchaseInvoices,
					ApzObjectTypes.oReserveInvoices,
					ApzObjectTypes.oAPReserveInvoice
				}.Contains(objType)) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[4]
				{
					ApzObjectTypes.oInvoices,
					ApzObjectTypes.oPurchaseInvoices,
					ApzObjectTypes.oReserveInvoices,
					ApzObjectTypes.oAPReserveInvoice
				}.Contains(docObjectCode)))
				{
					text2 = text2 + "<ReserveInvoice>" + Function.RawText(doc.isIns) + "</ReserveInvoice>";
				}
				text2 = text2 + "<Address2>" + Function.RawText(doc.Address2) + "</Address2>";
				if (objType != ApzObjectTypes.oPurchaseRequest || (objType == ApzObjectTypes.oDrafts && docObjectCode != ApzObjectTypes.oPurchaseRequest))
				{
					text2 = text2 + "<Address2>" + Function.RawText(doc.Address2) + "</Address2>";
				}
				if (!string.IsNullOrEmpty(doc.AgentCode))
				{
					text2 = text2 + "<AgentCode>" + Function.RawText(doc.AgentCode) + "</AgentCode>";
				}
				if (doc.AnnualInvoiceDeclarationReference != 0)
				{
					text2 = text2 + "<AnnualInvoiceDeclarationReference>" + Function.ParseInt(doc.AnnualInvoiceDeclarationReference) + "</AnnualInvoiceDeclarationReference>";
				}
				if (!string.IsNullOrEmpty(doc.BPChCode))
				{
					text2 = text2 + "<BPChannelCode>" + Function.RawText(doc.BPChCode) + "</BPChannelCode>";
				}
				if (doc.BPChCntc != 0)
				{
					text2 = text2 + "<BPChannelContact>" + Function.ParseInt(doc.BPChCntc) + "</BPChannelContact>";
				}
				if (doc.BPLId != 0 && doc.BPLId != -1)
				{
					text2 = text2 + "<BPL_IDAssignedToInvoice>" + Function.ParseInt(doc.BPLId) + "</BPL_IDAssignedToInvoice>";
				}
				if (doc.BlanketAgreementNumber != 0)
				{
					text2 = text2 + "<BlanketAgreementNumber>" + Function.ParseInt(doc.BlanketAgreementNumber) + "</BlanketAgreementNumber>";
				}
				if (!string.IsNullOrEmpty(doc.Box1099))
				{
					text2 = text2 + "<Box1099>" + Function.RawText(doc.Box1099) + "</Box1099>";
				}
				if (doc.Series > 0)
				{
					text2 = text2 + "<Series>" + doc.Series + "</Series>";
				}
				if (!string.IsNullOrEmpty(doc.CardCode))
				{
					text2 = text2 + "<CardCode>" + Function.RawText(doc.CardCode) + "</CardCode>";
				}
				if (!string.IsNullOrEmpty(doc.CardName))
				{
					text2 = text2 + "<CardName>" + Function.RawText(doc.CardName) + "</CardName>";
				}
				if (doc.CdcOffset != 0)
				{
					text2 = text2 + "<CashDiscountDateOffset>" + Function.ParseInt(doc.CdcOffset) + "</CashDiscountDateOffset>";
				}
				if (!string.IsNullOrEmpty(doc.CntrlBnk))
				{
					text2 = text2 + "<CentralBankIndicator>" + Function.RawText(doc.CntrlBnk) + "</CentralBankIndicator>";
				}
				if (!string.IsNullOrEmpty(doc.Footer))
				{
					text2 = text2 + "<ClosingRemarks>" + Function.RawText(doc.Footer) + "</ClosingRemarks>";
				}
				if (!string.IsNullOrEmpty(doc.CtlAccount))
				{
					text2 = text2 + "<ControlAccount>" + Function.RawText(doc.CtlAccount) + "</ControlAccount>";
				}
				if (!doc.DiscPrcnt.Equals(0.0) || commandType == EnumCommandType.Update)
				{
					text2 = text2 + "<DiscountPercent>" + Function.ParseDecimal(doc.DiscPrcnt) + "</DiscountPercent>";
				}
				if (!string.IsNullOrEmpty(doc.DocCur))
				{
					text2 = text2 + "<DocCurrency>" + Function.RawText(doc.DocCur) + "</DocCurrency>";
				}
				if (doc.DocDate > new DateTime(1900, 1, 1))
				{
					text2 = text2 + "<DocDate>" + Convert.ToDateTime(doc.DocDate).ToString("yyyyMMdd") + "</DocDate>";
				}
				if (((!string.IsNullOrEmpty(doc.isIns) && new ApzObjectTypes[4]
				{
					ApzObjectTypes.oInvoices,
					ApzObjectTypes.oPurchaseInvoices,
					ApzObjectTypes.oCreditNotes,
					ApzObjectTypes.oPurchaseCreditNotes
				}.Contains(objType)) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[4]
				{
					ApzObjectTypes.oInvoices,
					ApzObjectTypes.oPurchaseInvoices,
					ApzObjectTypes.oCreditNotes,
					ApzObjectTypes.oPurchaseCreditNotes
				}.Contains(docObjectCode))) && doc.AssetDate > new DateTime(1900, 1, 1))
				{
					text2 = text2 + "<AssetValueDate>" + Convert.ToDateTime(doc.AssetDate).ToString("yyyyMMdd") + "</AssetValueDate>";
				}
				if (!doc.DocNum.Equals(0) && objType == ApzObjectTypes.oDrafts)
				{
					text2 = text2 + "<DocNum>" + Function.ParseInt(doc.DocNum) + "</DocNum>";
				}
				if (!doc.DocRate.Equals(0.0))
				{
					text2 = text2 + "<DocRate>" + Function.ParseDecimal(doc.DocRate) + "</DocRate>";
				}
				if (!string.IsNullOrEmpty(doc.DocType))
				{
					text2 = ((!(doc.DocType == "S")) ? (text2 + "<DocType>dDocument_Items</DocType>") : (text2 + "<DocType>dDocument_Service</DocType>"));
				}
				if (doc.OwnerCode > 0)
				{
					text2 = text2 + "<DocumentsOwner>" + Function.ParseInt(doc.OwnerCode) + "</DocumentsOwner>";
				}
				if (!doc.DownPayment.Equals(0.0))
				{
					text2 = text2 + "<DownPayment>" + Function.ParseDecimal(doc.DownPayment) + "</DownPayment>";
				}
				text2 = ((!(doc.DocCur == text)) ? (text2 + "<DownPaymentAmountFC>" + Function.ParseDecimal(doc.DPMTotal) + "</DownPaymentAmountFC>") : (text2 + "<DownPaymentAmount>" + Function.ParseDecimal(doc.DPMTotal) + "</DownPaymentAmount>"));
				if (!doc.DpmPrcnt.Equals(0.0) || commandType == EnumCommandType.Update)
				{
					text2 = text2 + "<DownPaymentPercentage>" + Function.ParseDecimal(doc.DpmPrcnt) + "</DownPaymentPercentage>";
				}
				if (doc.TaxDate > new DateTime(1900, 1, 1))
				{
					text2 = text2 + "<TaxDate>" + Convert.ToDateTime(doc.TaxDate).ToString("yyyyMMdd") + "</TaxDate>";
				}
				if (!doc.TrnspCode.Equals(0))
				{
					text2 = text2 + "<TransportationCode>" + Function.ParseInt(doc.TrnspCode) + "</TransportationCode>";
				}
				if (doc.VatDate > new DateTime(1900, 1, 1))
				{
					text2 = text2 + "<VatDate>" + Convert.ToDateTime(doc.VatDate).ToString("yyyyMMdd") + "</VatDate>";
				}
				if (!doc.VatPercent.Equals(0.0))
				{
					text2 = text2 + "<VatPercent>" + Function.ParseDecimal(doc.VatPercent) + "</VatPercent>";
				}
				if (objType == ApzObjectTypes.oPurchaseRequest || docObjectCode == ApzObjectTypes.oPurchaseRequest)
				{
					if (!string.IsNullOrEmpty(doc.Requester))
					{
						text2 = text2 + "<Requester>" + Function.ToString(doc.Requester) + "</Requester>";
					}
					if (!string.IsNullOrEmpty(doc.Email))
					{
						text2 = text2 + "<RequesterEmail>" + Function.ToString(doc.Email) + "</RequesterEmail>";
					}
					if (!string.IsNullOrEmpty(doc.ReqName))
					{
						text2 = text2 + "<RequesterName>" + Function.ToString(doc.ReqName) + "</RequesterName>";
					}
					if (!Function.ParseInt(doc.Branch).Equals(0))
					{
						text2 = text2 + "<RequesterBranch>" + Function.ParseInt(doc.Branch) + "</RequesterBranch>";
					}
					if (!Function.ParseInt(doc.Department).Equals(0))
					{
						text2 = text2 + "<RequesterDepartment>" + Function.ParseInt(doc.Department) + "</RequesterDepartment>";
					}
					if (!Function.ParseInt(doc.ReqType).Equals(0))
					{
						text2 = text2 + "<ReqType>" + Function.ParseInt(doc.ReqType) + "</ReqType>";
					}
				}
				if (doc.ReqDate > new DateTime(1900, 1, 1) && objType != ApzObjectTypes.oInventoryGenEntry && objType != ApzObjectTypes.oInventoryGenExit)
				{
					text2 = text2 + "<RequriedDate>" + Convert.ToDateTime(doc.ReqDate).ToString("yyyyMMdd") + "</RequriedDate>";
				}
				if (((objType != ApzObjectTypes.oPurchaseOrders && objType != ApzObjectTypes.oPurchaseRequest && objType != ApzObjectTypes.oPurchaseQuotations && (objType != ApzObjectTypes.oPurchaseDownPayments || !(doc.Posted == "0")) && objType != ApzObjectTypes.oDrafts) || (docObjectCode != ApzObjectTypes.oPurchaseOrders && docObjectCode != ApzObjectTypes.oPurchaseRequest && docObjectCode != ApzObjectTypes.oPurchaseQuotations && (docObjectCode != ApzObjectTypes.oPurchaseDownPayments || !(doc.Posted == "0")) && objType == ApzObjectTypes.oDrafts)) && (!string.IsNullOrEmpty(doc.ShipToCode) || commandType == EnumCommandType.Update))
				{
					text2 = text2 + "<ShipToCode>" + Function.RawText(doc.ShipToCode) + "</ShipToCode>";
				}
				if ((objType == ApzObjectTypes.oPurchaseOrders || objType == ApzObjectTypes.oOrders) && !string.IsNullOrEmpty(doc.Confirmed))
				{
					text2 = text2 + "<Confirmed>" + (doc.Confirmed.Equals("Y") ? "tYES" : "tNO") + "</Confirmed>";
				}
				if (!string.IsNullOrEmpty(doc.PayToCode))
				{
					text2 = text2 + "<PayToCode>" + Function.RawText(doc.PayToCode) + "</PayToCode>";
				}
				text2 = text2 + "<PickRemark>" + Function.RawText(doc.PickRmrk) + "</PickRemark>";
			}
			if ((((text3.Equals("O") || text3.Equals("OPEN")) && objType != ApzObjectTypes.oInventoryGenEntry && objType != ApzObjectTypes.oInventoryGenExit) || commandType == EnumCommandType.Add) && doc.DocDueDate > new DateTime(1900, 1, 1))
			{
				text2 = text2 + "<DocDueDate>" + Convert.ToDateTime(doc.DocDueDate).ToString("yyyyMMdd") + "</DocDueDate>";
			}
			if (!string.IsNullOrEmpty(doc.JrnlMemo))
			{
				text2 = text2 + "<JournalMemo>" + Function.RawText(doc.JrnlMemo) + "</JournalMemo>";
			}
			if (objType != ApzObjectTypes.oInventoryGenEntry && objType != ApzObjectTypes.oInventoryGenExit)
			{
				if (!string.IsNullOrEmpty(doc.Project) || commandType == EnumCommandType.Add)
				{
					text2 = text2 + "<Project>" + Function.RawText(doc.Project) + "</Project>";
				}
				if (!string.IsNullOrEmpty(doc.Indicator))
				{
					text2 = text2 + "<Indicator>" + Function.RawText(doc.Indicator) + "</Indicator>";
				}
				if (!string.IsNullOrEmpty(doc.LicTradNum))
				{
					text2 = text2 + "<FederalTaxID>" + Function.RawText(doc.LicTradNum) + "</FederalTaxID>";
				}
				if (text3.Equals("O") || text3.Equals("OPEN"))
				{
					if (Function.ParseInt(doc.GroupNum) > 0)
					{
						text2 = text2 + "<PaymentGroupCode>" + Function.RawText(doc.GroupNum) + "</PaymentGroupCode>";
					}
					if (((objType != ApzObjectTypes.oPurchaseRequest && objType != ApzObjectTypes.oDrafts) || (objType == ApzObjectTypes.oDrafts && docObjectCode != ApzObjectTypes.oPurchaseRequest)) && !string.IsNullOrEmpty(doc.PeyMethod) && doc.Lines.FirstOrDefault((Document_LinesImpl t) => t.LineStatus == "C") == null)
					{
						text2 = text2 + "<PaymentMethod>" + Function.RawText(doc.PeyMethod) + "</PaymentMethod>";
					}
					if (commandType == EnumCommandType.Add || objType == ApzObjectTypes.oDrafts)
					{
						if (Function.ParseInt(doc.ExtraMonth) > 0)
						{
							text2 = text2 + "<ExtraMonth>" + Function.RawText(doc.ExtraMonth) + "</ExtraMonth>";
							BoPayTermDueTypes boPayTermDueTypes;
							try
							{
								boPayTermDueTypes = doc.StartFrom;
							}
							catch
							{
								boPayTermDueTypes = BoPayTermDueTypes.pdt_None;
							}
							text2 = text2 + "<StartFrom>" + Function.RawText(boPayTermDueTypes) + "</StartFrom>";
						}
						if (Function.ParseInt(doc.ExtraDays) > 0)
						{
							text2 = text2 + "<ExtraDays>" + Function.RawText(doc.ExtraDays) + "</ExtraDays>";
						}
					}
				}
			}
			text2 += DataService.MappingUdf(doc.UserFields, commandType, ignoreUpdateEmptyValue);
			bool flag2 = false;
			if (new ApzObjectTypes[9]
			{
				ApzObjectTypes.oPurchaseRequest,
				ApzObjectTypes.oPurchaseOrders,
				ApzObjectTypes.oPurchaseQuotations,
				ApzObjectTypes.oPurchaseDeliveryNotes,
				ApzObjectTypes.oPurchaseInvoices,
				ApzObjectTypes.oAPReserveInvoice,
				ApzObjectTypes.oPurchaseReturns,
				ApzObjectTypes.oGoodsReturnRequest,
				ApzObjectTypes.oPurchaseCreditNotes
			}.Contains(objType) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[9]
			{
				ApzObjectTypes.oPurchaseRequest,
				ApzObjectTypes.oPurchaseOrders,
				ApzObjectTypes.oPurchaseQuotations,
				ApzObjectTypes.oPurchaseDeliveryNotes,
				ApzObjectTypes.oPurchaseInvoices,
				ApzObjectTypes.oAPReserveInvoice,
				ApzObjectTypes.oPurchaseReturns,
				ApzObjectTypes.oGoodsReturnRequest,
				ApzObjectTypes.oPurchaseCreditNotes
			}.Contains(docObjectCode)))
			{
				flag2 = true;
			}
			text2 += "</row>\r\n                        </Documents>";
			if (doc.Lines != null && doc.Lines.Count > 0 && (flag || (objType != ApzObjectTypes.oInventoryGenEntry && objType != ApzObjectTypes.oInventoryGenExit)))
			{
				text2 += "<Document_Lines>";
				for (int num = 0; num < doc.Lines.Count; num++)
				{
					Document_LinesImpl document_LinesImpl = doc.Lines[num];
					string text4 = Function.ToString(document_LinesImpl.LineStatus).ToUpper();
					text2 += "<row>";
					if (document_LinesImpl.LineNum >= 0)
					{
						text2 = text2 + "<LineNum>" + Function.ParseInt(document_LinesImpl.LineNum) + "</LineNum>";
					}
					if (flag)
					{
						if (!flag2 && !string.IsNullOrEmpty(document_LinesImpl.AcctCode))
						{
							text2 = text2 + "<AccountCode>" + Function.RawText(document_LinesImpl.AcctCode) + "</AccountCode>";
						}
						if (!document_LinesImpl.ActBaseEnt.Equals(0))
						{
							text2 = text2 + "<ActualBaseEntry>" + Function.ParseInt(document_LinesImpl.ActBaseEnt) + "</ActualBaseEntry>";
						}
						if (document_LinesImpl.ActDelDate > new DateTime(1900, 1, 1))
						{
							text2 = text2 + "<ActualDeliveryDate>" + Convert.ToDateTime(document_LinesImpl.ActDelDate).ToString("yyyyMMdd") + "</ActualDeliveryDate>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Address))
						{
							text2 = text2 + "<Address>" + Function.RawText(document_LinesImpl.Address) + "</Address>";
						}
						if (!document_LinesImpl.AgrNo.Equals(0))
						{
							text2 = text2 + "<AgreementNo>" + Function.ParseInt(document_LinesImpl.AgrNo) + "</AgreementNo>";
						}
						if (!document_LinesImpl.AgrLnNum.Equals(0))
						{
							text2 = text2 + "<AgreementRowNumber>" + Function.ParseInt(document_LinesImpl.AgrLnNum) + "</AgreementRowNumber>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.CodeBars))
						{
							text2 = text2 + "<BarCode>" + Function.RawText(document_LinesImpl.CodeBars) + "</BarCode>";
						}
						if (!document_LinesImpl.BaseType.Equals(0) && !document_LinesImpl.BaseType.Equals(-1) && objType != ApzObjectTypes.oQuotations)
						{
							text2 = text2 + "<BaseType>" + Function.ParseInt(document_LinesImpl.BaseType) + "</BaseType>";
							if (!document_LinesImpl.BaseEntry.Equals(0))
							{
								text2 = text2 + "<BaseEntry>" + Function.ParseInt(document_LinesImpl.BaseEntry) + "</BaseEntry>";
							}
							if (!document_LinesImpl.BaseLine.Equals(-1))
							{
								text2 = text2 + "<BaseLine>" + Function.ParseInt(document_LinesImpl.BaseLine) + "</BaseLine>";
							}
						}
						if (!document_LinesImpl.Commission.Equals(0.0) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							text2 = text2 + "<CommisionPercent>" + Function.ParseDecimal(document_LinesImpl.Commission) + "</CommisionPercent>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							text2 = text2 + "<CostingCode>" + Function.RawText(document_LinesImpl.OcrCode) + "</CostingCode>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							text2 = text2 + "<CostingCode2>" + Function.RawText(document_LinesImpl.OcrCode2) + "</CostingCode2>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							text2 = text2 + "<CostingCode3>" + Function.RawText(document_LinesImpl.OcrCode3) + "</CostingCode3>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode4) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							text2 = text2 + "<CostingCode4>" + Function.RawText(document_LinesImpl.OcrCode4) + "</CostingCode4>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode5) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							text2 = text2 + "<CostingCode5>" + Function.RawText(document_LinesImpl.OcrCode5) + "</CostingCode5>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Currency))
						{
							text2 = text2 + "<Currency>" + Function.RawText(document_LinesImpl.Currency) + "</Currency>";
						}
						if (!document_LinesImpl.ExciseAmt.Equals(0.0))
						{
							text2 = text2 + "<ExciseAmount>" + Function.ParseDecimal(document_LinesImpl.ExciseAmt) + "</ExciseAmount>";
						}
						if (document_LinesImpl.BaseType != 202 && (!string.IsNullOrEmpty(document_LinesImpl.ItemCode) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update)))
						{
							text2 = text2 + "<ItemCode>" + Function.RawText(document_LinesImpl.ItemCode) + "</ItemCode>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Dscription) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							text2 = text2 + "<ItemDescription>" + Function.RawText(document_LinesImpl.Dscription) + "</ItemDescription>";
						}
						if (!document_LinesImpl.InvQty.Equals(0.0))
						{
							text2 = text2 + "<InventoryQuantity>" + Function.ParseDecimal(document_LinesImpl.InvQty) + "</InventoryQuantity>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Text) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							text2 = text2 + "<ItemDetails>" + Function.RawText(document_LinesImpl.Text) + "</ItemDetails>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.VendorNum) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							text2 = text2 + "<VendorNum>" + Function.RawText(document_LinesImpl.VendorNum) + "</VendorNum>";
						}
						if (!document_LinesImpl.LocCode.Equals(0))
						{
							text2 = text2 + "<LocationCode>" + Function.ParseInt(document_LinesImpl.LocCode) + "</LocationCode>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.unitMsr) && document_LinesImpl.BaseType <= 0)
						{
							text2 = text2 + "<MeasureUnit>" + Function.RawText(document_LinesImpl.unitMsr) + "</MeasureUnit>";
						}
						if (!document_LinesImpl.SpecifiedPriceBefDi)
						{
							if (document_LinesImpl.BaseType != 202)
							{
								if ((Math.Abs(document_LinesImpl.PriceBefDi) > 1E-07 || commandType == EnumCommandType.Update) && !document_LinesImpl.SpecifiedPriceAfterVat)
								{
									text2 = ((!(doc.PriceMode != "G")) ? (text2 + "<GrossPrice>" + document_LinesImpl.PriceAfVAT + "</GrossPrice>") : (text2 + "<UnitPrice>" + document_LinesImpl.PriceBefDi + "</UnitPrice>"));
								}
								else if (Math.Abs(document_LinesImpl.PriceAfVAT) > 1E-07)
								{
									text2 = text2 + "<PriceAfterVAT>" + document_LinesImpl.PriceAfVAT + "</PriceAfterVAT>";
								}
							}
							if (commandType == EnumCommandType.Add || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update) || (ignoreUpdateEmptyValue && Math.Abs(document_LinesImpl.DiscPrcnt) > 1E-07))
							{
								text2 = text2 + "<DiscountPercent>" + document_LinesImpl.DiscPrcnt + "</DiscountPercent>";
							}
						}
						else
						{
							string text5 = "";
							double num2 = 0.0;
							if (document_LinesImpl.BaseType != 202)
							{
								if ((!string.IsNullOrEmpty(doc.DocCur) && doc.DocCur == text) || string.IsNullOrEmpty(doc.DocCur))
								{
									text5 = ((!(doc.PriceMode != "G")) ? ("<GrossTotal>" + document_LinesImpl.GTotal + "</GrossTotal>") : ("<LineTotal>" + document_LinesImpl.LineTotal + "</LineTotal>"));
									num2 = document_LinesImpl.LineTotal;
								}
								else
								{
									text5 = ((!(doc.PriceMode != "G")) ? ("<GrossTotalFC>" + document_LinesImpl.GTotalFC + "</GrossTotalFC>") : ("<RowTotalFC>" + document_LinesImpl.TotalFrgn + "</RowTotalFC>"));
									num2 = Function.ParseDouble(document_LinesImpl.TotalFrgn);
								}
							}
							if (Math.Abs(num2) < 1E-07 && Math.Abs(document_LinesImpl.PriceBefDi) < 1E-07 && Math.Abs(document_LinesImpl.PriceAfVAT) < 1E-07)
							{
								if (document_LinesImpl.BaseType != 202)
								{
									text2 += "<UnitPrice>0</UnitPrice>";
									text2 += text5;
								}
							}
							else
							{
								if (document_LinesImpl.BaseType != 202)
								{
									text2 += text5;
								}
								if (document_LinesImpl.DiscPrcnt > 0.0)
								{
									if (document_LinesImpl.BaseType != 202)
									{
										if ((Math.Abs(document_LinesImpl.PriceBefDi) > 1E-07 || commandType == EnumCommandType.Update) && !document_LinesImpl.SpecifiedPriceAfterVat)
										{
											text2 = ((!(doc.PriceMode != "G")) ? (text2 + "<GrossPrice>" + document_LinesImpl.PriceAfVAT + "</GrossPrice>") : (text2 + "<UnitPrice>" + document_LinesImpl.PriceBefDi + "</UnitPrice>"));
										}
										else if (Math.Abs(document_LinesImpl.PriceAfVAT) > 1E-07)
										{
											text2 = text2 + "<PriceAfterVAT>" + document_LinesImpl.PriceAfVAT + "</PriceAfterVAT>";
										}
										if (commandType == EnumCommandType.Update)
										{
											text2 = text2 + "<DiscountPercent>" + document_LinesImpl.DiscPrcnt + "</DiscountPercent>";
										}
									}
								}
								else
								{
									text2 += "<DiscountPercent>0</DiscountPercent>";
									if (document_LinesImpl.BaseType != 202)
									{
										if (commandType != EnumCommandType.Update)
										{
											if (!doc.SaveFromForm && !(Math.Abs(document_LinesImpl.Quantity * document_LinesImpl.PriceBefDi * (1.0 - document_LinesImpl.DiscPrcnt / 100.0) - num2) < 1E-07))
											{
												text2 = (((!(Math.Abs(document_LinesImpl.PriceAfVAT) > 1E-07) || !(Math.Abs(document_LinesImpl.PriceBefDi) < 1E-07)) && !document_LinesImpl.SpecifiedPriceAfterVat) ? (text2 + "<UnitPrice>0</UnitPrice>") : (text2 + "<PriceAfterVAT>" + document_LinesImpl.PriceAfVAT + "</PriceAfterVAT>"));
											}
											else
											{
												text2 = ((!(doc.PriceMode != "G")) ? (text2 + "<GrossPrice>" + document_LinesImpl.PriceAfVAT + "</GrossPrice>") : (text2 + "<UnitPrice>" + document_LinesImpl.PriceBefDi + "</UnitPrice>"));
											}
										}
										else if ((Math.Abs(document_LinesImpl.PriceAfVAT) > 1E-07 && Math.Abs(document_LinesImpl.PriceBefDi) < 1E-07) || document_LinesImpl.SpecifiedPriceAfterVat)
										{
											text2 = text2 + "<PriceAfterVAT>" + document_LinesImpl.PriceAfVAT + "</PriceAfterVAT>";
										}
										else if (Math.Abs(document_LinesImpl.Quantity * document_LinesImpl.PriceBefDi * (1.0 - document_LinesImpl.DiscPrcnt / 100.0) - num2) < 1E-07)
										{
											text2 = ((!(doc.PriceMode != "G")) ? (text2 + "<GrossPrice>" + document_LinesImpl.PriceAfVAT + "</GrossPrice>") : (text2 + "<UnitPrice>" + document_LinesImpl.PriceBefDi + "</UnitPrice>"));
										}
									}
								}
							}
						}
						if (!new int[8] { 15, 13, 14, 16, 18, 19, 20, 21 }.Contains(document_LinesImpl.BaseType))
						{
							if (!string.IsNullOrEmpty(document_LinesImpl.CogsAcct))
							{
								text2 = text2 + "<COGSAccountCode>" + Function.RawText(document_LinesImpl.CogsAcct) + "</COGSAccountCode>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCod))
							{
								text2 = text2 + "<COGSCostingCode>" + Function.RawText(document_LinesImpl.CogsOcrCod) + "</COGSCostingCode>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCo2))
							{
								text2 = text2 + "<COGSCostingCode2>" + Function.RawText(document_LinesImpl.CogsOcrCo2) + "</COGSCostingCode2>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCo3))
							{
								text2 = text2 + "<COGSCostingCode3>" + Function.RawText(document_LinesImpl.CogsOcrCo3) + "</COGSCostingCode3>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCo4))
							{
								text2 = text2 + "<COGSCostingCode4>" + Function.RawText(document_LinesImpl.CogsOcrCo4) + "</COGSCostingCode4>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCo5))
							{
								text2 = text2 + "<COGSCostingCode5>" + Function.RawText(document_LinesImpl.CogsOcrCo5) + "</COGSCostingCode5>";
							}
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Project) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update) || commandType == EnumCommandType.Add)
						{
							text2 = text2 + "<ProjectCode>" + Function.RawText(document_LinesImpl.Project) + "</ProjectCode>";
						}
						if ((objType == ApzObjectTypes.oDeliveryNotes && (document_LinesImpl.TreeType.Equals("S") || document_LinesImpl.TreeType.Equals("I"))) || !document_LinesImpl.Quantity.Equals(0.0) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							text2 = text2 + "<Quantity>" + Function.ParseDecimal(document_LinesImpl.Quantity) + "</Quantity>";
						}
						if (document_LinesImpl.LineStatus != "C" && document_LinesImpl.TargetType <= 0 && !document_LinesImpl.Rate.Equals(0.0) && (commandType == EnumCommandType.Add || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update && (document_LinesImpl.TreeType == "N" || (document_LinesImpl.TreeType == "I" && document_LinesImpl.BOMTreeType != "0" && Function.ParseInt(document_LinesImpl.BOMTreeType) == 0)))))
						{
							if ((!string.IsNullOrEmpty(document_LinesImpl.Currency) && document_LinesImpl.Currency == text) || string.IsNullOrEmpty(document_LinesImpl.Currency))
							{
								text2 += "<Rate>0</Rate>";
							}
							else if (document_LinesImpl.Currency == doc.DocCur)
							{
								text2 = text2 + "<Rate>" + Function.ParseDecimal(doc.DocCur) + "</Rate>";
							}
							else if (document_LinesImpl.Currency != text)
							{
								text2 = text2 + "<Rate>" + Function.ParseDecimal(document_LinesImpl.Rate) + "</Rate>";
							}
						}
						if (document_LinesImpl.PQTReqDate > new DateTime(1900, 1, 1) && objType != ApzObjectTypes.oInventoryGenEntry && objType != ApzObjectTypes.oInventoryGenExit)
						{
							text2 = text2 + "<RequiredDate>" + Convert.ToDateTime(document_LinesImpl.PQTReqDate).ToString("yyyyMMdd") + "</RequiredDate>";
						}
						if (!document_LinesImpl.PQTReqQty.Equals(0.0))
						{
							text2 = text2 + "<RequiredQuantity>" + Function.ParseDecimal(document_LinesImpl.PQTReqQty) + "</RequiredQuantity>";
						}
						if (!document_LinesImpl.PackQty.Equals(0.0) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							text2 = text2 + "<PackageQuantity>" + Function.ParseDecimal(document_LinesImpl.PackQty) + "</PackageQuantity>";
						}
						if ((!document_LinesImpl.SlpCode.Equals(0) && commandType == EnumCommandType.Add) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update) || (ignoreUpdateEmptyValue && document_LinesImpl.SlpCode > 0))
						{
							text2 = text2 + "<SalesPersonCode>" + Function.ParseInt(document_LinesImpl.SlpCode) + "</SalesPersonCode>";
						}
						else if ((!doc.SlpCode.Equals(0) && commandType == EnumCommandType.Add) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update) || (ignoreUpdateEmptyValue && doc.SlpCode > 0))
						{
							text2 = text2 + "<SalesPersonCode>" + Function.ParseInt(doc.SlpCode) + "</SalesPersonCode>";
						}
						_ = document_LinesImpl.ShipDate;
						if (document_LinesImpl.ShipDate > new DateTime(1900, 1, 1))
						{
							text2 = text2 + "<ShipDate>" + Convert.ToDateTime(document_LinesImpl.ShipDate).ToString("yyyyMMdd") + "</ShipDate>";
						}
						if ((!document_LinesImpl.TrnsCode.Equals(0) && commandType == EnumCommandType.Add) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update) || (ignoreUpdateEmptyValue && document_LinesImpl.TrnsCode > 0))
						{
							text2 = text2 + "<ShippingMethod>" + Function.ParseInt(document_LinesImpl.TrnsCode) + "</ShippingMethod>";
						}
						if ((objType != ApzObjectTypes.oPurchaseOrders && objType != ApzObjectTypes.oPurchaseRequest && objType != ApzObjectTypes.oPurchaseQuotations && (objType != ApzObjectTypes.oPurchaseDownPayments || !(doc.Posted == "0")) && objType != ApzObjectTypes.oDrafts) || (docObjectCode != ApzObjectTypes.oPurchaseOrders && docObjectCode != ApzObjectTypes.oPurchaseRequest && docObjectCode != ApzObjectTypes.oPurchaseQuotations && (docObjectCode != ApzObjectTypes.oPurchaseDownPayments || !(doc.Posted == "0")) && objType == ApzObjectTypes.oDrafts))
						{
							if (commandType == EnumCommandType.Update)
							{
								text2 = text2 + "<ShipToCode>" + Function.RawText(document_LinesImpl.ShipToCode) + "</ShipToCode>";
							}
							if (commandType == EnumCommandType.Update)
							{
								text2 = text2 + "<ShipToDescription>" + Function.RawText(document_LinesImpl.ShipToDesc) + "</ShipToDescription>";
							}
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.TaxCode))
						{
							text2 = text2 + "<TaxCode>" + Function.RawText(document_LinesImpl.TaxCode) + "</TaxCode>";
						}
						if (document_LinesImpl.NumPerMsr > 0.0)
						{
							text2 = text2 + "<UnitsOfMeasurment>" + document_LinesImpl.NumPerMsr + "</UnitsOfMeasurment>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.UseBaseUn) && new string[4] { "N", "Y", "tYES", "tNO" }.Contains(document_LinesImpl.UseBaseUn))
						{
							text2 = text2 + "<UseBaseUnits>" + Function.RawText(document_LinesImpl.UseBaseUn) + "</UseBaseUnits>";
						}
						if (!document_LinesImpl.UomEntry.Equals(0))
						{
							text2 = text2 + "<UoMEntry>" + Function.ParseInt(document_LinesImpl.UomEntry) + "</UoMEntry>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.VatGroup))
						{
							text2 = text2 + "<VatGroup>" + Function.RawText(document_LinesImpl.VatGroup) + "</VatGroup>";
						}
						text2 = ((!(doc.DocCur == text)) ? (text2 + "<TaxTotal>" + Function.ParseDecimal(document_LinesImpl.VatSumFrgn) + "</TaxTotal>") : (text2 + "<TaxTotal>" + Function.ParseDecimal(document_LinesImpl.LineVat) + "</TaxTotal>"));
						if (!string.IsNullOrEmpty(document_LinesImpl.WhsCode))
						{
							text2 = text2 + "<WarehouseCode>" + Function.RawText(document_LinesImpl.WhsCode) + "</WarehouseCode>";
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.SubCatNum) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							text2 = text2 + "<SupplierCatNum>" + Function.ToString(document_LinesImpl.SubCatNum) + "</SupplierCatNum>";
						}
						if (objType == ApzObjectTypes.oPurchaseRequest || (objType == ApzObjectTypes.oDrafts && docObjectCode == ApzObjectTypes.oPurchaseRequest))
						{
							text2 = text2 + "<LineVendor>" + Function.ToString(document_LinesImpl.LineVendor) + "</LineVendor>";
						}
						if (new ApzObjectTypes[2]
						{
							ApzObjectTypes.oCreditNotes,
							ApzObjectTypes.oPurchaseCreditNotes
						}.Contains(objType) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[2]
						{
							ApzObjectTypes.oCreditNotes,
							ApzObjectTypes.oPurchaseCreditNotes
						}.Contains(docObjectCode)))
						{
							text2 += ((document_LinesImpl.NoInvtryMv == "Y") ? "<WithoutInventoryMovement>tYES</WithoutInventoryMovement>" : "<WithoutInventoryMovement>tNO</WithoutInventoryMovement>");
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.TreeType) && !"0".Equals(document_LinesImpl.TreeType))
						{
							text2 = text2 + "<TreeType>" + Function.ToString(document_LinesImpl.TreeType) + "</TreeType>";
						}
						if (objType == ApzObjectTypes.oReturns)
						{
							if (!string.IsNullOrEmpty(document_LinesImpl.EnSetCost) && document_LinesImpl.EnSetCost.Equals("Y"))
							{
								text2 += "<EnableReturnCost>tYES</EnableReturnCost>";
								text2 = text2 + "<ReturnCost>" + Function.ParseDecimal(document_LinesImpl.RetCost) + "</ReturnCost>";
							}
							else
							{
								text2 += "<EnableReturnCost>tNO</EnableReturnCost>";
								text2 += "<ReturnCost/>";
							}
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.TaxOnly))
						{
							text2 = text2 + "<TaxOnly>" + Function.ToString(document_LinesImpl.TaxOnly) + "</TaxOnly>";
						}
					}
					if ((text3.Equals("O") || text3.Equals("OPEN") || text4.Equals("OPEN") || text4.Equals("O")) && !string.IsNullOrEmpty(document_LinesImpl.FreeTxt))
					{
						text2 = text2 + "<FreeText>" + Function.RawText(document_LinesImpl.FreeTxt) + "</FreeText>";
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.LineStatus) && commandType != EnumCommandType.Add && document_LinesImpl.LineStatus == "C" && (new ApzObjectTypes[5]
					{
						ApzObjectTypes.oQuotations,
						ApzObjectTypes.oOrders,
						ApzObjectTypes.oPurchaseRequest,
						ApzObjectTypes.oPurchaseOrders,
						ApzObjectTypes.oPurchaseQuotations
					}.Contains(objType) || (new ApzObjectTypes[5]
					{
						ApzObjectTypes.oQuotations,
						ApzObjectTypes.oOrders,
						ApzObjectTypes.oPurchaseRequest,
						ApzObjectTypes.oPurchaseOrders,
						ApzObjectTypes.oPurchaseQuotations
					}.Contains(docObjectCode) && objType == ApzObjectTypes.oDrafts) || ((objType == ApzObjectTypes.oDownPayments || objType == ApzObjectTypes.oPurchaseDownPayments) && Function.ParseInt(doc.Posted) == 0)))
					{
						text2 += "<LineStatus>bost_Close</LineStatus>";
					}
					if (objType == ApzObjectTypes.oInventoryGenEntry && doc.Lines.FirstOrDefault((Document_LinesImpl t) => t.BaseType == 202) != null && !doc.Lines.Any((Document_LinesImpl t) => t.BaseLine >= 0))
					{
						text2 = text2 + "<TransactionType>" + ((document_LinesImpl.TranType == "R") ? "botrntReject" : "botrntComplete") + "</TransactionType>";
					}
					text2 += DataService.MappingUdf(document_LinesImpl.UserFields, commandType, ignoreUpdateEmptyValue);
					text2 += "</row>";
				}
				text2 += "</Document_Lines>";
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					text2 += "<BatchNumbers>";
					for (int num3 = 0; num3 < doc.Lines.Count; num3++)
					{
						if (doc.Lines[num3].BatchNumbers == null || doc.Lines[num3].BatchNumbers.Count <= 0)
						{
							continue;
						}
						for (int num4 = 0; num4 < doc.Lines[num3].BatchNumbers.Count; num4++)
						{
							text2 += "<row>";
							BatchNumbersImpl batchNumbersImpl = doc.Lines[num3].BatchNumbers[num4];
							if (!string.IsNullOrEmpty(batchNumbersImpl.BatchNumber))
							{
								text2 = text2 + "<BatchNumber>" + Function.ToString(batchNumbersImpl.BatchNumber) + "</BatchNumber>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.ManufacturerSerialNumber))
							{
								text2 = text2 + "<ManufacturerSerialNumber>" + Function.RawText(batchNumbersImpl.ManufacturerSerialNumber) + "</ManufacturerSerialNumber>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.InternalSerialNumber))
							{
								text2 = text2 + "<InternalSerialNumber>" + Function.RawText(batchNumbersImpl.InternalSerialNumber) + "</InternalSerialNumber>";
							}
							if (batchNumbersImpl.ExpiryDate.HasValue && batchNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
							{
								text2 = text2 + "<ExpiryDate>" + Convert.ToDateTime(batchNumbersImpl.ExpiryDate).ToString("yyyyMMdd") + "</ExpiryDate>";
							}
							if (batchNumbersImpl.ManufacturingDate.HasValue && batchNumbersImpl.ManufacturingDate > new DateTime(1900, 1, 1))
							{
								text2 = text2 + "<ManufacturingDate>" + Convert.ToDateTime(batchNumbersImpl.ManufacturingDate).ToString("yyyyMMdd") + "</ManufacturingDate>";
							}
							if (batchNumbersImpl.AddmisionDate.HasValue && batchNumbersImpl.AddmisionDate > new DateTime(1900, 1, 1))
							{
								text2 = text2 + "<AddmisionDate>" + Convert.ToDateTime(batchNumbersImpl.AddmisionDate).ToString("yyyyMMdd") + "</AddmisionDate>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Location))
							{
								text2 = text2 + "<Location>" + Function.RawText(batchNumbersImpl.Location) + "</Location>";
							}
							if (commandType == EnumCommandType.Update || batchNumbersImpl.Quantity > 0.0)
							{
								text2 = text2 + "<Quantity>" + Function.ParseDecimal(batchNumbersImpl.Quantity) + "</Quantity>";
							}
							if (batchNumbersImpl.BaseLineNumber >= 0)
							{
								text2 = text2 + "<BaseLineNumber>" + Function.ParseInt(batchNumbersImpl.BaseLineNumber) + "</BaseLineNumber>";
							}
							if (0 < batchNumbersImpl.TrackingNote)
							{
								text2 = text2 + "<TrackingNote>" + Function.RawText(batchNumbersImpl.TrackingNote) + "</TrackingNote>";
							}
							if (0 < batchNumbersImpl.TrackingNoteLine)
							{
								text2 = text2 + "<TrackingNoteLine>" + Function.RawText(batchNumbersImpl.TrackingNoteLine) + "</TrackingNoteLine>";
							}
							text2 += DataService.MappingUdf(batchNumbersImpl.UserFields, EnumCommandType.Add, ignoreUpdateEmptyValue);
							text2 += "</row>";
						}
					}
					text2 += "</BatchNumbers>";
				}
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					text2 += "<SerialNumbers>";
					for (int num5 = 0; num5 < doc.Lines.Count; num5++)
					{
						if (doc.Lines[num5].SerialNumbers == null || doc.Lines[num5].SerialNumbers.Count <= 0)
						{
							continue;
						}
						for (int num6 = 0; num6 < doc.Lines[num5].SerialNumbers.Count; num6++)
						{
							text2 += "<row>";
							SerialNumbersImpl serialNumbersImpl = doc.Lines[num5].SerialNumbers[num6];
							if (!string.IsNullOrEmpty(serialNumbersImpl.BatchID))
							{
								text2 = text2 + "<BatchID>" + Function.ToString(serialNumbersImpl.BatchID) + "</BatchID>";
							}
							if (serialNumbersImpl.SystemSerialNumber > 0)
							{
								text2 = text2 + "<SystemSerialNumber>" + serialNumbersImpl.SystemSerialNumber + "</SystemSerialNumber>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.ManufacturerSerialNumber))
							{
								text2 = text2 + "<ManufacturerSerialNumber>" + Function.RawText(serialNumbersImpl.ManufacturerSerialNumber) + "</ManufacturerSerialNumber>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.InternalSerialNumber))
							{
								text2 = text2 + "<InternalSerialNumber>" + Function.RawText(serialNumbersImpl.InternalSerialNumber) + "</InternalSerialNumber>";
							}
							if (!new ApzObjectTypes[4]
							{
								ApzObjectTypes.oDeliveryNotes,
								ApzObjectTypes.oInvoices,
								ApzObjectTypes.oReserveInvoices,
								ApzObjectTypes.oInventoryGenExit
							}.Contains(objType))
							{
								if (serialNumbersImpl.ExpiryDate.HasValue && serialNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
								{
									text2 = text2 + "<ExpiryDate>" + Convert.ToDateTime(serialNumbersImpl.ExpiryDate).ToString("yyyyMMdd") + "</ExpiryDate>";
								}
								if (serialNumbersImpl.ManufactureDate.HasValue && serialNumbersImpl.ManufactureDate > new DateTime(1900, 1, 1))
								{
									text2 = text2 + "<ManufacturingDate>" + Convert.ToDateTime(serialNumbersImpl.ManufactureDate).ToString("yyyyMMdd") + "</ManufacturingDate>";
								}
								if (serialNumbersImpl.ReceptionDate.HasValue && serialNumbersImpl.ReceptionDate > new DateTime(1900, 1, 1))
								{
									text2 = text2 + "<ReceptionDate>" + Convert.ToDateTime(serialNumbersImpl.ReceptionDate).ToString("yyyyMMdd") + "</ReceptionDate>";
								}
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.Location))
							{
								text2 = text2 + "<Location>" + Function.RawText(serialNumbersImpl.Location) + "</Location>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.Notes))
							{
								text2 = text2 + "<Notes>" + Function.RawText(serialNumbersImpl.Notes) + "</Notes>";
							}
							if (commandType == EnumCommandType.Update || serialNumbersImpl.Quantity > 0.0)
							{
								text2 = text2 + "<Quantity>" + Function.ParseDecimal(serialNumbersImpl.Quantity) + "</Quantity>";
							}
							if (serialNumbersImpl.BaseLineNumber >= 0)
							{
								text2 = text2 + "<BaseLineNumber>" + Function.ParseInt(serialNumbersImpl.BaseLineNumber) + "</BaseLineNumber>";
							}
							if (0 < serialNumbersImpl.TrackingNote)
							{
								text2 = text2 + "<TrackingNote>" + Function.RawText(serialNumbersImpl.TrackingNote) + "</TrackingNote>";
							}
							if (0 < serialNumbersImpl.TrackingNoteLine)
							{
								text2 = text2 + "<TrackingNoteLine>" + Function.RawText(serialNumbersImpl.TrackingNoteLine) + "</TrackingNoteLine>";
							}
							text2 += DataService.MappingUdf(serialNumbersImpl.UserFields, EnumCommandType.Add, ignoreUpdateEmptyValue);
							text2 += "</row>";
						}
					}
					text2 += "</SerialNumbers>";
				}
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					text2 += "<DocumentLinesBinAllocations>";
					for (int num7 = 0; num7 < doc.Lines.Count; num7++)
					{
						if (doc.Lines[num7].BinAllocations == null || doc.Lines[num7].BinAllocations.Count <= 0)
						{
							continue;
						}
						for (int num8 = 0; num8 < doc.Lines[num7].BinAllocations.Count; num8++)
						{
							text2 += "<row>";
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = doc.Lines[num7].BinAllocations[num8];
							if (!documentLinesBinAllocationsImpl.BinAbsEntry.Equals(0))
							{
								text2 = text2 + "<BinAbsEntry>" + Function.ParseInt(documentLinesBinAllocationsImpl.BinAbsEntry) + "</BinAbsEntry>";
							}
							if (!documentLinesBinAllocationsImpl.Quantity.Equals(0.0))
							{
								text2 = text2 + "<Quantity>" + Function.ParseDecimal(documentLinesBinAllocationsImpl.Quantity) + "</Quantity>";
							}
							if (documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine.HasValue && documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine >= 0)
							{
								text2 = text2 + "<SerialAndBatchNumbersBaseLine>" + Function.ParseInt(documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine) + "</SerialAndBatchNumbersBaseLine>";
							}
							if (documentLinesBinAllocationsImpl.BaseLineNumber >= 0)
							{
								text2 = text2 + "<BaseLineNumber>" + Function.ParseInt(documentLinesBinAllocationsImpl.BaseLineNumber) + "</BaseLineNumber>";
							}
							text2 += "</row>";
						}
					}
					text2 += "</DocumentLinesBinAllocations>";
				}
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					text2 += "<Document_LinesAdditionalExpenses>";
					for (int num9 = 0; num9 < doc.Lines.Count; num9++)
					{
						Document_LinesImpl document_LinesImpl2 = doc.Lines[num9];
						if (document_LinesImpl2 == null)
						{
							continue;
						}
						if (document_LinesImpl2.ExpnsCode_F1 > 0)
						{
							text2 += "<row>";
							text2 = ((commandType == EnumCommandType.Add) ? (text2 + "<LineNumber>" + num9 + "</LineNumber>") : (text2 + "<LineNumber>" + document_LinesImpl2.LineNum + "</LineNumber>"));
							text2 += "<GroupCode>0</GroupCode>";
							text2 = text2 + "<ExpenseCode>" + document_LinesImpl2.ExpnsCode_F1 + "</ExpenseCode>";
							if (!Function.ParseDecimal(document_LinesImpl2.TotalFrgn_F1).Equals(0m))
							{
								text2 = text2 + "<LineTotal>" + Function.ParseDecimal(document_LinesImpl2.TotalFrgn_F1) + "</LineTotal>";
							}
							else if (!Function.ParseDecimal(document_LinesImpl2.LineTotal_F1).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<LineTotal>" + Function.ParseDecimal(document_LinesImpl2.LineTotal_F1) + "</LineTotal>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.VatGroup_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<VatGroup>" + Function.RawText(document_LinesImpl2.VatGroup_F1) + "</VatGroup>";
							}
							if (!Function.ParseDecimal(document_LinesImpl2.VatPrcnt_F1).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<TaxPercent>" + Function.ParseDecimal(document_LinesImpl2.VatPrcnt_F1) + "</TaxPercent>";
							}
							if (!Function.ParseDecimal(document_LinesImpl2.VatSum_F1).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<TaxSum>" + Function.ParseDecimal(document_LinesImpl2.VatSum_F1) + "</TaxSum>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule>" + Function.RawText(document_LinesImpl2.OcrCode_F1) + "</DistributionRule>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode2_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule2>" + Function.RawText(document_LinesImpl2.OcrCode2_F1) + "</DistributionRule2>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode3_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule3>" + Function.RawText(document_LinesImpl2.OcrCode3_F1) + "</DistributionRule3>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode4_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule4>" + Function.RawText(document_LinesImpl2.OcrCode4_F1) + "</DistributionRule4>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode5_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule5>" + Function.RawText(document_LinesImpl2.OcrCode5_F1) + "</DistributionRule5>";
							}
							text2 += "</row>";
						}
						if (document_LinesImpl2.ExpnsCode_F2 > 0)
						{
							text2 += "<row>";
							text2 = ((commandType == EnumCommandType.Add) ? (text2 + "<LineNumber>" + num9 + "</LineNumber>") : (text2 + "<LineNumber>" + document_LinesImpl2.LineNum + "</LineNumber>"));
							text2 += "<GroupCode>1</GroupCode>";
							text2 = text2 + "<ExpenseCode>" + document_LinesImpl2.ExpnsCode_F2 + "</ExpenseCode>";
							if (!Function.ParseDecimal(document_LinesImpl2.TotalFrgn_F2).Equals(0m))
							{
								text2 = text2 + "<LineTotal>" + Function.ParseDecimal(document_LinesImpl2.TotalFrgn_F2) + "</LineTotal>";
							}
							else if (!Function.ParseDecimal(document_LinesImpl2.LineTotal_F2).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<LineTotal>" + Function.ParseDecimal(document_LinesImpl2.LineTotal_F2) + "</LineTotal>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.VatGroup_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<VatGroup>" + Function.RawText(document_LinesImpl2.VatGroup_F2) + "</VatGroup>";
							}
							if (!Function.ParseDecimal(document_LinesImpl2.VatPrcnt_F2).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<TaxPercent>" + Function.ParseDecimal(document_LinesImpl2.VatPrcnt_F2) + "</TaxPercent>";
							}
							if (!Function.ParseDecimal(document_LinesImpl2.VatSum_F2).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<TaxSum>" + Function.ParseDecimal(document_LinesImpl2.VatSum_F2) + "</TaxSum>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule>" + Function.RawText(document_LinesImpl2.OcrCode_F2) + "</DistributionRule>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode2_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule2>" + Function.RawText(document_LinesImpl2.OcrCode2_F2) + "</DistributionRule2>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode3_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule3>" + Function.RawText(document_LinesImpl2.OcrCode3_F2) + "</DistributionRule3>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode4_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule4>" + Function.RawText(document_LinesImpl2.OcrCode4_F2) + "</DistributionRule4>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode5_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule5>" + Function.RawText(document_LinesImpl2.OcrCode5_F2) + "</DistributionRule5>";
							}
							text2 += "</row>";
						}
						if (document_LinesImpl2.ExpnsCode_F3 > 0)
						{
							text2 += "<row>";
							text2 = ((commandType == EnumCommandType.Add) ? (text2 + "<LineNumber>" + num9 + "</LineNumber>") : (text2 + "<LineNumber>" + document_LinesImpl2.LineNum + "</LineNumber>"));
							text2 += "<GroupCode>2</GroupCode>";
							text2 = text2 + "<ExpenseCode>" + document_LinesImpl2.ExpnsCode_F3 + "</ExpenseCode>";
							if (!Function.ParseDecimal(document_LinesImpl2.TotalFrgn_F3).Equals(0m))
							{
								text2 = text2 + "<LineTotal>" + Function.ParseDecimal(document_LinesImpl2.TotalFrgn_F3) + "</LineTotal>";
							}
							else if (!Function.ParseDecimal(document_LinesImpl2.LineTotal_F3).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<LineTotal>" + Function.ParseDecimal(document_LinesImpl2.LineTotal_F3) + "</LineTotal>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.VatGroup_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<VatGroup>" + Function.RawText(document_LinesImpl2.VatGroup_F3) + "</VatGroup>";
							}
							if (!Function.ParseDecimal(document_LinesImpl2.VatPrcnt_F3).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<TaxPercent>" + Function.ParseDecimal(document_LinesImpl2.VatPrcnt_F3) + "</TaxPercent>";
							}
							if (!Function.ParseDecimal(document_LinesImpl2.VatSum_F3).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<TaxSum>" + Function.ParseDecimal(document_LinesImpl2.VatSum_F3) + "</TaxSum>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule>" + Function.RawText(document_LinesImpl2.OcrCode_F3) + "</DistributionRule>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode2_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule2>" + Function.RawText(document_LinesImpl2.OcrCode2_F3) + "</DistributionRule2>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode3_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule3>" + Function.RawText(document_LinesImpl2.OcrCode3_F3) + "</DistributionRule3>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode4_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule4>" + Function.RawText(document_LinesImpl2.OcrCode4_F3) + "</DistributionRule4>";
							}
							if (!string.IsNullOrEmpty(document_LinesImpl2.OcrCode5_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								text2 = text2 + "<DistributionRule5>" + Function.RawText(document_LinesImpl2.OcrCode5_F3) + "</DistributionRule5>";
							}
							text2 += "</row>";
						}
					}
					text2 += "</Document_LinesAdditionalExpenses>";
				}
			}
			if ((new ApzObjectTypes[4]
			{
				ApzObjectTypes.oInvoices,
				ApzObjectTypes.oReserveInvoices,
				ApzObjectTypes.oAPReserveInvoice,
				ApzObjectTypes.oPurchaseInvoices
			}.Contains(objType) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[4]
			{
				ApzObjectTypes.oInvoices,
				ApzObjectTypes.oReserveInvoices,
				ApzObjectTypes.oAPReserveInvoice,
				ApzObjectTypes.oPurchaseInvoices
			}.Contains(docObjectCode))) && commandType == EnumCommandType.Add && doc.DownPaymentsToDraw != null && doc.DownPaymentsToDraw.Any())
			{
				throw new Exception("DI service does not support 'Down Payments To Draw'");
			}
			if (doc.SpecialLines != null && doc.SpecialLines.Count > 0)
			{
				text2 += "<Document_SpecialLines>";
				for (int num10 = 0; num10 < doc.SpecialLines.Count; num10++)
				{
					text2 += "<row>";
					text2 = ((doc.SpecialLines[num10].LineType != BoDocSpecialLineType.dslt_Subtotal) ? (text2 + "<LineType>dslt_Text</LineType>") : (text2 + "<LineType>dslt_Subtotal</LineType>"));
					text2 = text2 + "<AfterLineNumber>" + doc.SpecialLines[num10].AfterLineNumber + "</AfterLineNumber>";
					if (!string.IsNullOrEmpty(doc.SpecialLines[num10].LineText))
					{
						text2 = text2 + "<LineText>" + Function.RawText(doc.SpecialLines[num10].LineText) + "</LineText>";
					}
					text2 += " </row>";
				}
				text2 += "</Document_SpecialLines>";
			}
			if (doc.Expenses != null && doc.Expenses.Count > 0)
			{
				text2 += "<DocumentsAdditionalExpenses>";
				for (int num11 = 0; num11 < doc.Expenses.Count; num11++)
				{
					DocumentsAdditionalExpensesImpl documentsAdditionalExpensesImpl = doc.Expenses[num11];
					if (documentsAdditionalExpensesImpl == null || string.IsNullOrEmpty(documentsAdditionalExpensesImpl.Status) || !documentsAdditionalExpensesImpl.Status.ToUpper().Equals("O") || !(documentsAdditionalExpensesImpl.LineTotal > 0.0))
					{
						continue;
					}
					text2 += "<row>";
					if (!Function.ParseInt(documentsAdditionalExpensesImpl.ExpnsCode).Equals(0))
					{
						text2 = text2 + "<ExpenseCode>" + Function.ParseInt(documentsAdditionalExpensesImpl.ExpnsCode) + "</ExpenseCode>";
					}
					if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.TotalFrgn).Equals(0m))
					{
						text2 = text2 + "<LineTotal>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.TotalFrgn) + "</LineTotal>";
					}
					else if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.LineTotal).Equals(0m))
					{
						text2 = text2 + "<LineTotal>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.LineTotal) + "</LineTotal>";
					}
					if (commandType == EnumCommandType.Update && ((Function.ParseDecimal(documentsAdditionalExpensesImpl.TotalFrgn).Equals(0m) && doc.DocCur != text) || (Function.ParseDecimal(documentsAdditionalExpensesImpl.LineTotal).Equals(0m) && doc.DocCur == text)) && !ignoreUpdateEmptyValue)
					{
						text2 += "<LineTotal>0</LineTotal>";
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.Comments))
					{
						text2 = text2 + "<Remarks>" + Function.RawText(documentsAdditionalExpensesImpl.Comments) + "</Remarks>";
					}
					string text6 = "";
					text6 = Function.ToString(documentsAdditionalExpensesImpl.DistrbMthd) switch
					{
						"Q" => "aedm_Quantity", 
						"V" => "aedm_Volume", 
						"W" => "aedm_Weight", 
						"E" => "aedm_Equally", 
						"T" => "aedm_RowTotal", 
						_ => "aedm_None", 
					};
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.DistrbMthd))
					{
						text2 = text2 + "<DistributionMethod>" + text6 + "</DistributionMethod>";
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.VatGroup))
					{
						text2 = text2 + "<VatGroup>" + Function.RawText(documentsAdditionalExpensesImpl.VatGroup) + "</VatGroup>";
					}
					if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.VatPrcnt).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
					{
						text2 = text2 + "<TaxPercent>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.VatPrcnt) + "</TaxPercent>";
					}
					if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSumFrgn).Equals(0m))
					{
						text2 = text2 + "<TaxSum>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSumFrgn) + "</TaxSum>";
					}
					else if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSum).Equals(0m))
					{
						text2 = text2 + "<TaxSum>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSum) + "</TaxSum>";
					}
					if (commandType == EnumCommandType.Update && ((Function.ParseDecimal(documentsAdditionalExpensesImpl.TotalFrgn).Equals(0m) && doc.DocCur != text) || (Function.ParseDecimal(documentsAdditionalExpensesImpl.LineTotal).Equals(0m) && doc.DocCur == text)) && !ignoreUpdateEmptyValue)
					{
						text2 += "<TaxSum>0</TaxSum>";
					}
					if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.DedVatSum).Equals(0m))
					{
						text2 = text2 + "<DeductibleTaxSum>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.DedVatSum) + "</DeductibleTaxSum>";
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.TaxCode))
					{
						text2 = text2 + "<TaxCode>" + Function.RawText(documentsAdditionalExpensesImpl.TaxCode) + "</TaxCode>";
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.TaxType))
					{
						text2 = text2 + "<TaxType>" + Function.RawText(documentsAdditionalExpensesImpl.TaxType) + "</TaxType>";
					}
					if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatPer).Equals(0m))
					{
						text2 = text2 + "<EqualizationTaxPercent>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatPer) + "</EqualizationTaxPercent>";
					}
					if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatSum).Equals(0m))
					{
						text2 = text2 + "<EqualizationTaxSum>" + Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatSum) + "</EqualizationTaxSum>";
					}
					if (!Function.ParseInt(documentsAdditionalExpensesImpl.BaseType).Equals(0) && !Function.ParseInt(documentsAdditionalExpensesImpl.BaseType).Equals(-1) && !Function.ParseInt(documentsAdditionalExpensesImpl.BaseType).Equals(540000006))
					{
						text2 = text2 + "<BaseDocType>" + Function.ParseInt(documentsAdditionalExpensesImpl.BaseType) + "</BaseDocType>";
						if (!Function.ParseInt(documentsAdditionalExpensesImpl.BaseAbsEnt).Equals(0))
						{
							text2 = text2 + "<BaseDocEntry>" + Function.ParseInt(documentsAdditionalExpensesImpl.BaseAbsEnt) + "</BaseDocEntry>";
						}
						if (Function.ParseInt(documentsAdditionalExpensesImpl.BaseLnNum) >= 0)
						{
							text2 = text2 + "<BaseDocLine>" + Function.ParseInt(documentsAdditionalExpensesImpl.BaseLnNum) + "</BaseDocLine>";
						}
					}
					if (documentsAdditionalExpensesImpl.LstPchPrce == "Y")
					{
						text2 += "<LastPurchasePrice>tYES</LastPurchasePrice>";
					}
					else if (documentsAdditionalExpensesImpl.LstPchPrce == "N")
					{
						text2 += "<LastPurchasePrice>tNO</LastPurchasePrice>";
					}
					if (documentsAdditionalExpensesImpl.Stock == "Y")
					{
						text2 += "<Stock>tYES</Stock>";
					}
					else if (documentsAdditionalExpensesImpl.Stock == "N")
					{
						text2 += "<Stock>tNO</Stock>";
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode))
					{
						text2 = text2 + "<DistributionRule>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode) + "</DistributionRule>";
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode2))
					{
						text2 = text2 + "<DistributionRule2>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode2) + "</DistributionRule2>";
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode3))
					{
						text2 = text2 + "<DistributionRule3>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode3) + "</DistributionRule3>";
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode4))
					{
						text2 = text2 + "<DistributionRule4>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode4) + "</DistributionRule4>";
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode5))
					{
						text2 = text2 + "<DistributionRule5>" + Function.RawText(documentsAdditionalExpensesImpl.OcrCode5) + "</DistributionRule5>";
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.Project))
					{
						text2 = text2 + "<Project>" + Function.RawText(documentsAdditionalExpensesImpl.Project) + "</Project>";
					}
					text2 += "</row>";
				}
				text2 += "</DocumentsAdditionalExpenses>";
			}
			text2 += "</BO></BOM>";
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
		}
		return text2;
	}

	public void MappingUdfTable(ref DataSet ds, string tablename, List<UserFieldsImpl> lst, EnumCommandType commandType = EnumCommandType.Add)
	{
		if (lst == null || lst.Count <= 0)
		{
			return;
		}
		foreach (UserFieldsImpl item in lst)
		{
			if (item == null || string.IsNullOrEmpty(item.Name))
			{
				continue;
			}
			if (!item.Name.StartsWith("U_"))
			{
				item.Name = "U_" + item.Name;
			}
			switch (item.Type)
			{
			case BoFieldTypes.db_Alpha:
			case BoFieldTypes.db_Memo:
				if (!string.IsNullOrEmpty(Function.ToString(item.Value)))
				{
					if (!ds.Tables[tablename].IsExistsColumn(item.Name))
					{
						ds.Tables[tablename].Columns.Add(item.Name, typeof(string));
					}
					ds.Tables[tablename].Rows[0][item.Name] = Function.RawText(item.Value);
				}
				break;
			case BoFieldTypes.db_Date:
				if (Function.ParseDateTime(item.Value).HasValue && Convert.ToDateTime(item.Value) > new DateTime(1900, 1, 1))
				{
					if (!ds.Tables[tablename].IsExistsColumn(item.Name))
					{
						ds.Tables[tablename].Columns.Add(item.Name, typeof(DateTime));
					}
					ds.Tables[tablename].Rows[0][item.Name] = Convert.ToDateTime(item.Value).ToString("yyyy-MM-dd");
				}
				break;
			case BoFieldTypes.db_Numeric:
			case BoFieldTypes.db_Float:
				if (Math.Abs(Function.ParseDecimal(item.Value)) > 0m || commandType == EnumCommandType.Update)
				{
					if (!ds.Tables[tablename].IsExistsColumn(item.Name))
					{
						ds.Tables[tablename].Columns.Add(item.Name, typeof(decimal));
					}
					ds.Tables[tablename].Rows[0][item.Name] = Function.ParseDecimal(item.Value);
				}
				break;
			}
		}
	}

	protected virtual string DeleteLines(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, List<int> lstRemoved)
	{
		try
		{
			string byParams = GetByParams(sessionId, objType, doc);
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[DI-SERVER] No matching record found.");
			}
			XElement xElement = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Document"));
			DataSet dataSet = new DataSet();
			using (StringReader reader = new StringReader(Function.ToString(xElement)))
			{
				dataSet.ReadXml(reader);
			}
			if (dataSet.Tables.Contains("DocumentLine"))
			{
				bool flag = false;
				DataTable dataTable = dataSet.Tables["DocumentLine"];
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
				if (dataSet.Tables.Contains("DocumentLineAdditionalExpense"))
				{
					DataTable dataTable2 = dataSet.Tables["DocumentLineAdditionalExpense"];
					List<string> source = (from t in dataTable2.AsEnumerable()
						select Function.ToString(t["LineNumber"]) + "-" + Function.ToString(t["GroupCode"])).ToList();
					List<string> docexp = new List<string>();
					for (int num = 0; num < doc.Lines.Count; num++)
					{
						if (doc.Lines[num].ExpnsCode_F1 > 0)
						{
							docexp.Add(Function.ToString(doc.Lines[num].LineNum) + "-0");
						}
						if (doc.Lines[num].ExpnsCode_F2 > 0)
						{
							docexp.Add(Function.ToString(doc.Lines[num].LineNum) + "-1");
						}
						if (doc.Lines[num].ExpnsCode_F3 > 0)
						{
							docexp.Add(Function.ToString(doc.Lines[num].LineNum) + "-2");
						}
					}
					List<string> list = source.Where((string line) => !docexp.Contains(line)).ToList();
					if (list.Count > 0)
					{
						flag = true;
						foreach (string i2 in list)
						{
							DataRow dataRow2 = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => (Function.ToString(t["LineNumber"]) + "-" + Function.ToString(t["GroupCode"])).Equals(i2));
							if (dataRow2 != null)
							{
								dataTable2.Rows.Remove(dataRow2);
							}
						}
					}
				}
				if (dataSet.Tables.Contains("DocumentSpecialLine"))
				{
					DataTable dataTable3 = dataSet.Tables["DocumentSpecialLine"];
					if (dataTable3.IsNotNull())
					{
						if (doc.SpecialLines == null || (doc.SpecialLines != null && doc.SpecialLines.Count == 0))
						{
							flag = true;
							dataTable3.Rows.Clear();
						}
						else if (dataTable3.Rows.Count > doc.SpecialLines.Count)
						{
							flag = true;
							for (int num2 = dataTable3.Rows.Count - 1; num2 >= doc.SpecialLines.Count; num2--)
							{
								dataTable3.Rows.RemoveAt(num2);
							}
						}
						dataTable3.AcceptChanges();
					}
				}
				if (!flag)
				{
					return string.Empty;
				}
				string xml = dataSet.GetXml();
				if (!string.IsNullOrEmpty(xml))
				{
					string xmlObject = "<?xml version=\"1.0\" ?>\r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>" + GetServiceName(objType) + "</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"" + GetServiceName(objType) + "\">" + xml + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
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

	public void Dispose()
	{
		Dispose();
	}

	public virtual async Task<HttpResult> SaveDraftToDocumentApproval(DocumentsImpl doc, string sessionId, string userName = "")
	{
		try
		{
			Task<DataTable> task = Task.Run(() => _serviceUnitOfWork.Documents.Search(new DocumentsImpl
			{
				DocEntry = doc.draftKey,
				UserName = userName
			}, ApzObjectTypes.oDrafts));
			Task<DataTable> t2 = Task.Run(() => _serviceUnitOfWork.Documents.TableLine(doc.draftKey, ApzObjectTypes.oDrafts));
			Task<DataTable> t3 = Task.Run(() => _serviceUnitOfWork.DocumentDraft.GetApprovalInfo(doc.draftKey));
			DocumentsImpl saveDoc = (await task).Rows[0].ToDocument();
			saveDoc.draftKey = saveDoc.DocEntry;
			saveDoc.DocEntry = 0;
			saveDoc.DocObjectCode = (ApzObjectTypes)saveDoc.ObjType;
			DataTable dataTable = await t2;
			List<Document_LinesImpl> source = dataTable.ToDocumentLine();
			source = source.Where((Document_LinesImpl l) => !string.IsNullOrEmpty(l.ItemCode) && (string.IsNullOrEmpty(l.LineType) || l.LineType == "R")).ToList();
			saveDoc.Lines = source;
			List<DataRow> list = (from dataRow in dataTable.AsEnumerable()
				where new string[2] { "T", "S" }.Contains(Function.ToString(dataRow["LineType"]))
				select dataRow).ToList();
			if (list != null && list.Count > 0)
			{
				saveDoc.SpecialLines = new List<Document_SpecialLinesImpl>();
				for (int num = 0; num < list.Count; num++)
				{
					Document_SpecialLinesImpl document_SpecialLinesImpl = new Document_SpecialLinesImpl();
					document_SpecialLinesImpl.AfterLineNumber = Function.ParseInt(list[num]["LineNum"]);
					string text = Function.ToString(list[num]["LineType"]);
					document_SpecialLinesImpl.LineNum = num;
					if (text == "T")
					{
						document_SpecialLinesImpl.LineType = BoDocSpecialLineType.dslt_Text;
						document_SpecialLinesImpl.LineText = Function.ToString(list[num]["ItemCode"]);
					}
					else if (text == "S")
					{
						document_SpecialLinesImpl.LineType = BoDocSpecialLineType.dslt_Subtotal;
						document_SpecialLinesImpl.Subtotal = Function.ParseDouble(list[num]["LineTotal"]);
						document_SpecialLinesImpl.SubtotalFC = Function.ParseDouble(list[num]["TotalFrgn"]);
					}
					saveDoc.SpecialLines.Add(document_SpecialLinesImpl);
				}
			}
			DataTable dataTable2 = await t3;
			if (dataTable2 != null && dataTable2.Rows.Count == 1 && Function.ParseInt(dataTable2.Rows[0]["DocEntry"]) > 0)
			{
				saveDoc.DocEntry = Function.ParseInt(dataTable2.Rows[0]["DocEntry"]);
			}
			return SaveDraftToDocument(saveDoc, sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual HttpResult SaveDraftToDocument(DocumentsImpl doc, string sessionId)
	{
		try
		{
			if (new ApzObjectTypes[2]
			{
				ApzObjectTypes.oInventoryTransferRequest,
				ApzObjectTypes.oStockTransfer
			}.Contains(doc.DocObjectCode))
			{
				return _unitOfWork.StockTransfer.SaveDraftToDocument(doc, sessionId);
			}
			string text = ((doc.DocEntry > 0) ? "UpdateObject" : "AddObject");
			new DataTable();
			string text2 = "";
			string text3 = string.Empty;
			List<int> list = new List<int>();
			if (text == "UpdateObject")
			{
				string tableByApzObjectType = Function.GetTableByApzObjectType(doc.DocObjectCode);
				Logging.Write("WATCH", $"Remove document from approval process- docEntry {doc.DocEntry} - object type {doc.DocObjectCode}");
				text2 = _unitOfWork.UDOs.GetXmlExecuteSQL("UPDATE " + tableByApzObjectType + " SET WddStatus='-' WHERE DocEntry =" + doc.DocEntry, "", isGetBody: true);
				if (new ApzObjectTypes[5]
				{
					ApzObjectTypes.oQuotations,
					ApzObjectTypes.oOrders,
					ApzObjectTypes.oPurchaseRequest,
					ApzObjectTypes.oPurchaseQuotations,
					ApzObjectTypes.oPurchaseOrders
				}.Contains(doc.DocObjectCode) || doc.DocObjectCode == ApzObjectTypes.oDrafts || (new ApzObjectTypes[2]
				{
					ApzObjectTypes.oDownPayments,
					ApzObjectTypes.oPurchaseDownPayments
				}.Contains(doc.DocObjectCode) && doc.Posted == "0"))
				{
					List<string> tableName = Function.GetTableName(doc.DocObjectCode);
					IEnumerable<int> source = from t in doc.Lines
						select new { t.LineNum, t.VisualOrder } into t
						orderby t.VisualOrder
						select t.LineNum;
					string sql = "UPDATE T0\r\n                                            SET T0.VisOrder=T1.Id-1\r\n                                         FROM " + tableName[1] + " T0 INNER JOIN (SELECT * FROM dbo.Split('" + string.Join(",", source.ToArray()) + "',',')) T1 ON T0.LineNum=T1.Data\r\n                                         WHERE T0.DocEntry=" + doc.DocEntry;
					text3 = _unitOfWork.UDOs.GetXmlExecuteSQL(sql, "", isGetBody: true);
					string text4 = string.Join(",", doc.Lines.Select((Document_LinesImpl t) => t.LineNum));
					DataTable dt = ExecuteDataTable("SELECT * FROM " + tableName[1] + " WHERE DocEntry=@DocEntry AND LineNum NOT IN (" + text4 + ")", CommandType.Text, new SqlParameter("@DocEntry", doc.DocEntry));
					if (dt.IsNotNull())
					{
						List<Document_LinesImpl> list2 = dt.ToDocumentLine();
						if (list2 != null && list2.Count > 0)
						{
							list.AddRange(list2.Select((Document_LinesImpl t) => t.LineNum));
							doc.Lines.AddRange(list2);
						}
					}
					doc.Lines = (from t in doc.Lines
						orderby t.VisualOrder, t.LineNum
						select t).ToList();
				}
			}
			string text5 = ((doc.DocEntry > 0) ? Mapping(doc, EnumCommandType.Update, doc.DocObjectCode) : Mapping(doc, EnumCommandType.Add, doc.DocObjectCode));
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text2 + "<dis:" + text + " xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text5 + "</dis:" + text + ">" + text3 + "</env:Body></env:Envelopes>";
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
					Logging.Write("WATCH", string.Format(text + " document - {0} - DocEntry - {1}", doc.DocObjectCode, num));
					try
					{
						if (!ServiceUnitOfWork.Documents.SaveDraftToDocument(doc.DocObjectCode, num, doc.draftKey))
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "", "");
						}
						if (new ApzObjectTypes[7]
						{
							ApzObjectTypes.oQuotations,
							ApzObjectTypes.oOrders,
							ApzObjectTypes.oPurchaseRequest,
							ApzObjectTypes.oPurchaseQuotations,
							ApzObjectTypes.oPurchaseOrders,
							ApzObjectTypes.oDownPayments,
							ApzObjectTypes.oPurchaseDownPayments
						}.Contains(doc.DocObjectCode) || doc.DocObjectCode == ApzObjectTypes.oDrafts)
						{
							DeleteLines(doc, sessionId, doc.DocObjectCode, list);
						}
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					}
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
		return new HttpResult();
	}

	public string ApproveDraft(string sessionId, string user, DataTable dt, string serverPath = "")
	{
		try
		{
			int userId = GetUserId(user);
			return _unitOfWork.ApprovalRequests.ApproveDraft(sessionId, user, userId, dt, serverPath);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public int GetUserId(string userCode)
	{
		int result = 0;
		try
		{
			DataTable dataTable = ExecuteDataTable("select top 1 UserID from OUSR where \"USER_CODE\" = @UserCode;", CommandType.Text, new SqlParameter("@UserCode", userCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ParseInt(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public virtual string RemoveAndCloseDocumentDraft(DataTable dt, string sessionId)
	{
		try
		{
			string text = "<?xml version=\"1.0\" encoding=\"utf-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">                          \r\n                <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">";
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				text = ((!(dt.Rows[i]["UpdateType"].ToString() == "C")) ? (text + "\r\n                                  <dis:RemoveObject  xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                  <BOM>\r\n                                   <BO>\r\n                                    <AdmInfo>\r\n                                     <Object>oDrafts</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams>\r\n                                     <DocEntry>" + Function.ToString(dt.Rows[i]["Id"]) + "</DocEntry>\r\n                                    </QueryParams>\r\n                                   </BO>\r\n                                  </BOM>\r\n                                  </dis:RemoveObject >") : (text + "\r\n                                  <dis:CloseObject  xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                  <BOM>\r\n                                   <BO>\r\n                                    <AdmInfo>\r\n                                     <Object>oDrafts</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams>\r\n                                     <DocEntry>" + Function.ToString(dt.Rows[i]["Id"]) + "</DocEntry>\r\n                                    </QueryParams>\r\n                                   </BO>\r\n                                  </BOM>\r\n                                  </dis:CloseObject >"));
			}
			text += "</env:Body> </env:Envelopes>";
			XmlElement xmlElement = DataService.Process(text).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", $"Close document - {ApzObjectTypes.oDrafts} - {xElement2.Value}");
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Close document - {ApzObjectTypes.oDrafts} - DocEntry - {num}");
					return "";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "Error";
	}

	public async Task<string> SaveTestScriptAsync(string ss)
	{
		try
		{
			return await SaveTestScript(ss);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public async Task<string> SaveTestScript(string ss)
	{
		string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + ss + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\"><BOM>\r\n                          <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                          <AdmInfo>\r\n                            <Object>oOrders</Object>\r\n                          </AdmInfo> <Documents>\r\n                            <row><SalesPersonCode>6</SalesPersonCode><ContactPersonCode>6</ContactPersonCode><BPL_IDAssignedToInvoice>1</BPL_IDAssignedToInvoice><CardCode>C0000006</CardCode><CardName>Công Ty Cổ Phần Sản Xuất In An Bình</CardName><DocCurrency>VND</DocCurrency><DocDate>20160525</DocDate><DocDueDate>20160525</DocDueDate><DocRate>1</DocRate><TaxDate>20160525</TaxDate><PaymentMethod>-1</PaymentMethod><StartFrom>pdt_None</StartFrom><U_invTaxCode>0312297229</U_invTaxCode></row>\r\n                        </Documents><Document_Lines><row><Currency>VND</Currency><ItemCode>BLWK-A0014</ItemCode><ItemDescription>Niêm phong dòn, rất dễ vỡ, đế trắng dày BL-A</ItemDescription><InventoryQuantity>1</InventoryQuantity><Quantity>1</Quantity><Rate>1.0</Rate><UnitsOfMeasurment>1</UnitsOfMeasurment><UseBaseUnits>N</UseBaseUnits><MeasureUnit>m2</MeasureUnit><VatGroup>A</VatGroup><WithoutInventoryMovement>tNO</WithoutInventoryMovement></row><row><Currency>VND</Currency><ItemCode>BLWK-B1016</ItemCode><ItemDescription>Kiểm tra lại mã hàng</ItemDescription><InventoryQuantity>1</InventoryQuantity><Quantity>1</Quantity><Rate>1.0</Rate><UnitsOfMeasurment>1</UnitsOfMeasurment><UseBaseUnits>N</UseBaseUnits><MeasureUnit>m2</MeasureUnit><VatGroup>A</VatGroup><WithoutInventoryMovement>tNO</WithoutInventoryMovement></row></Document_Lines><BatchNumbers></BatchNumbers><DocumentLinesBinAllocations></DocumentLinesBinAllocations><Document_LinesAdditionalExpenses></Document_LinesAdditionalExpenses></BO></BOM></dis:AddObject></env:Body></env:Envelopes>";
		XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
		XElement xElement = XElement.Parse(xmlElement.OuterXml);
		if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
		{
			XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
			if (xElement2 != null)
			{
				Logging.Write("ERROR", $" document - {ApzObjectTypes.oQuotations} - {xElement2.Value}");
				return xElement2.Value;
			}
		}
		else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
		{
			XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
			if (xElement3 != null)
			{
				int num = Function.ParseInt(xElement3.Value);
				Logging.Write("WATCH", $" document - {ApzObjectTypes.oQuotations} - DocEntry - {num}");
				return "";
			}
		}
		return string.Empty;
	}

	public virtual string RemoveDoc(int idDocEntry, string sessionId, ApzObjectTypes apzObjectTypes)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n                <env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                    <SessionID>" + sessionId + "</SessionID>\r\n                </env:Header>\r\n                    <env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                  <dis:RemoveObject  xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                  <BOM>\r\n                                   <BO>\r\n                                    <AdmInfo>\r\n                                     <Object>" + apzObjectTypes.ToString() + "</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams>\r\n                                     <DocEntry>" + idDocEntry + "</DocEntry>\r\n                                    </QueryParams>\r\n                                   </BO>\r\n                                  </BOM>\r\n                                  </dis:RemoveObject >\r\n                    </env:Body>\r\n                </env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", $"Remove document - {apzObjectTypes} - {xElement2.Value}");
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Remove document - {apzObjectTypes} - DocEntry - {num}");
					return "";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "Unknown error";
	}

	public string GetByParams(string sessionId, ApzObjectTypes objType, DocumentsImpl doc)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>" + GetServiceName(objType) + "</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetByParams xmlns=\"" + GetServiceName(objType) + "\">\r\n                                     <Document>   <DocEntry>" + doc.DocEntry + "</DocEntry></Document>\r\n                                    </GetByParams>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
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

	public string CreateCancellationDocument(string sessionId, ApzObjectTypes objType, DocumentsImpl doc)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>" + GetServiceName(objType) + "</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <CreateCancellationDocument xmlns=\"" + GetServiceName(objType) + "\">\r\n                                     <Document>   <DocEntry>6</DocEntry></Document>\r\n                                    </CreateCancellationDocument>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
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

	public string GetServiceName(ApzObjectTypes objType)
	{
		try
		{
			switch (objType)
			{
			case ApzObjectTypes.oOrders:
				return "OrdersService";
			case ApzObjectTypes.oQuotations:
				return "QuotationsService";
			case ApzObjectTypes.oDeliveryNotes:
				return "DeliveryNotesService";
			case ApzObjectTypes.oReturns:
				return "ReturnsService";
			case ApzObjectTypes.oCreditNotes:
				return "CreditNotesService";
			case ApzObjectTypes.oInvoices:
				return "InvoicesService";
			case ApzObjectTypes.oDrafts:
				return "DraftsService";
			case ApzObjectTypes.oPurchaseRequest:
				return "PurchaseRequestService";
			case ApzObjectTypes.oPurchaseQuotations:
				return "PurchaseQuotationsService";
			case ApzObjectTypes.oPurchaseOrders:
				return "PurchaseOrdersService";
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				return "PurchaseDeliveryNotesService";
			case ApzObjectTypes.oPurchaseReturns:
				return "PurchaseReturnsService";
			case ApzObjectTypes.oPurchaseCreditNotes:
				return "PurchaseCreditNotesService";
			case ApzObjectTypes.oPurchaseInvoices:
				return "PurchaseInvoicesService";
			case ApzObjectTypes.oInventoryGenEntry:
				return "InventoryGenEntryService";
			case ApzObjectTypes.oInventoryGenExit:
				return "InventoryGenExitService";
			case ApzObjectTypes.oDownPayments:
				return "DownPaymentsService";
			case ApzObjectTypes.oPurchaseDownPayments:
				return "PurchaseDownPaymentsService";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public string GetTableNameByObjectType(ApzObjectTypes objType)
	{
		try
		{
			switch (objType)
			{
			case ApzObjectTypes.oOrders:
				return "ORDR";
			case ApzObjectTypes.oQuotations:
				return "OQUT";
			case ApzObjectTypes.oDeliveryNotes:
				return "ODLN";
			case ApzObjectTypes.oReturns:
				return "ORDN";
			case ApzObjectTypes.oCreditNotes:
				return "ORIN";
			case ApzObjectTypes.oInvoices:
				return "OINV";
			case ApzObjectTypes.oDrafts:
				return "ODRF";
			case ApzObjectTypes.oPurchaseRequest:
				return "OPRQ";
			case ApzObjectTypes.oPurchaseQuotations:
				return "OPQT";
			case ApzObjectTypes.oPurchaseOrders:
				return "OPOR";
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				return "OPDN";
			case ApzObjectTypes.oPurchaseReturns:
				return "ORPD";
			case ApzObjectTypes.oPurchaseCreditNotes:
				return "ORPC";
			case ApzObjectTypes.oPurchaseInvoices:
				return "OPCH";
			case ApzObjectTypes.oInventoryGenEntry:
				return "OIGN";
			case ApzObjectTypes.oInventoryGenExit:
				return "OIGN";
			case ApzObjectTypes.oDownPayments:
				return "ODPI";
			case ApzObjectTypes.oPurchaseDownPayments:
				return "ODPO";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public virtual HttpResult AddListDocument(List<DocumentsImpl> docs, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			string text = string.Empty;
			if (docs != null && docs.Count > 0)
			{
				for (int i = 0; i < docs.Count; i++)
				{
					text = text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(docs[i], EnumCommandType.Add, objType) + "</dis:AddObject>";
				}
				string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
				XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						Logging.Write("ERROR", "Add list " + objType.ToString() + " error: " + xElement2.Value);
						return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
					}
				}
				else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
				{
					List<XElement> list = (from t in xElement.Descendants()
						where t.Name.LocalName.Equals("RetKey")
						select t).ToList();
					if (list.Count > 0)
					{
						List<string> list2 = new List<string>();
						list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
						if (list2.Count > 0)
						{
							Logging.Write("WATCH", "Add list " + objType.ToString() + " Success: " + string.Join(",", list2.ToArray()));
							return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
						}
					}
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

	public virtual HttpResult UpdateListDocument(List<DocumentsImpl> docs, string sessionId)
	{
		try
		{
			string text = string.Empty;
			if (docs != null && docs.Count > 0)
			{
				for (int i = 0; i < docs.Count; i++)
				{
					ApzObjectTypes objType = (ApzObjectTypes)docs[i].ObjType;
					text = ((docs[i].DocEntry > 0) ? (text + "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(docs[i], EnumCommandType.Update, objType) + "</dis:UpdateObject>") : (text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(docs[i], EnumCommandType.Add, objType) + "</dis:AddObject>"));
				}
				string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
				XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						Logging.Write("ERROR", "Update list doc error: " + xElement2.Value);
						return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
					}
				}
				else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
				{
					List<XElement> list = (from t in xElement.Descendants()
						where t.Name.LocalName.Equals("RetKey")
						select t).ToList();
					if (list.Count > 0)
					{
						List<string> list2 = new List<string>();
						list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
						if (list2.Count > 0)
						{
							Logging.Write("WATCH", "Update list doc Success: " + string.Join(",", list2.ToArray()));
							return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
						}
					}
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

	public virtual HttpResult SaveData(DocumentsImpl doc, string sessionId)
	{
		try
		{
			string empty = string.Empty;
			ApzObjectTypes objType = (ApzObjectTypes)doc.ObjType;
			_ = string.Empty;
			List<int> list = new List<int>();
			if (doc.DocEntry <= 0)
			{
				empty = empty + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(doc, EnumCommandType.Add, objType) + "</dis:AddObject>";
			}
			else
			{
				if (new ApzObjectTypes[5]
				{
					ApzObjectTypes.oQuotations,
					ApzObjectTypes.oOrders,
					ApzObjectTypes.oPurchaseRequest,
					ApzObjectTypes.oPurchaseQuotations,
					ApzObjectTypes.oPurchaseOrders
				}.Contains(objType) || objType == ApzObjectTypes.oDrafts)
				{
					IEnumerable<int> source = from t in doc.Lines
						select new { t.LineNum, t.VisualOrder } into t
						orderby t.VisualOrder
						select t.LineNum;
					string sql = "UPDATE T0\r\n                                            SET T0.VisOrder=T1.Id-1\r\n                                         FROM RDR1 T0 INNER JOIN (SELECT * FROM dbo.Split('" + string.Join(",", source.ToArray()) + "',',')) T1 ON T0.LineNum=T1.Data\r\n                                         WHERE T0.DocEntry=" + doc.DocEntry;
					_unitOfWork.UDOs.GetXmlExecuteSQL(sql, "", isGetBody: true);
					string text = string.Join(",", doc.Lines.Select((Document_LinesImpl t) => t.LineNum));
					List<string> tableName = Function.GetTableName(objType);
					DataTable dt = ExecuteDataTable("SELECT * FROM " + tableName[1] + " WHERE DocEntry=@DocEntry AND LineNum NOT IN (" + text + ")", CommandType.Text, new SqlParameter("@DocEntry", doc.DocEntry));
					if (dt.IsNotNull())
					{
						List<Document_LinesImpl> list2 = dt.ToDocumentLine();
						if (list2 != null && list2.Count > 0)
						{
							list.AddRange(list2.Select((Document_LinesImpl t) => t.LineNum));
							doc.Lines.AddRange(list2);
						}
						doc.Lines = doc.Lines.OrderBy((Document_LinesImpl t) => t.LineNum).ToList();
					}
				}
				empty = empty + "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(doc, EnumCommandType.Update, objType) + "</dis:UpdateObject>";
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + empty + "</env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Update list doc error: " + xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				List<XElement> list3 = (from t in xElement.Descendants()
					where t.Name.LocalName.Equals("RetKey")
					select t).ToList();
				if (list3.Count > 0)
				{
					List<string> list4 = new List<string>();
					list4.AddRange(list3.Select((XElement t) => Function.ToString(t.Value)));
					if (list4.Count > 0)
					{
						Logging.Write("WATCH", "Update list doc Success: " + string.Join(",", list4.ToArray()));
						if ((new ApzObjectTypes[7]
						{
							ApzObjectTypes.oQuotations,
							ApzObjectTypes.oOrders,
							ApzObjectTypes.oPurchaseRequest,
							ApzObjectTypes.oPurchaseQuotations,
							ApzObjectTypes.oPurchaseOrders,
							ApzObjectTypes.oDownPayments,
							ApzObjectTypes.oPurchaseDownPayments
						}.Contains(objType) || objType == ApzObjectTypes.oDrafts) && list.Count > 0)
						{
							DeleteLines(doc, sessionId, objType, list);
						}
						return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list4.ToArray()));
					}
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

	public virtual HttpResult AddListDocument(List<DocumentsImpl> docs, string sessionId)
	{
		try
		{
			string text = string.Empty;
			if (docs != null && docs.Count > 0)
			{
				for (int i = 0; i < docs.Count; i++)
				{
					ApzObjectTypes objType = (ApzObjectTypes)docs[i].ObjType;
					text = text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(docs[i], EnumCommandType.Add, objType) + "</dis:AddObject>";
				}
				string text2 = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
				XmlElement xmlElement = DataService.Process(text2).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						Logging.Write("ERROR", "Add list error: " + xElement2.Value);
						StringBuilder stringBuilder = new StringBuilder("/**********BEGIN Log for debug [Send XML: ");
						stringBuilder.Append(text2);
						stringBuilder.Append("]" + Environment.NewLine);
						stringBuilder.Append("[Return XML: ");
						stringBuilder.Append(xmlElement.OuterXml);
						stringBuilder.Append("]END Log for debug**********/" + Environment.NewLine);
						Logging.Write("ERROR", stringBuilder.ToString());
						return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
					}
				}
				else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
				{
					List<XElement> list = (from t in xElement.Descendants()
						where t.Name.LocalName.Equals("RetKey")
						select t).ToList();
					if (list.Count > 0)
					{
						List<string> list2 = new List<string>();
						list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
						if (list2.Count > 0)
						{
							Logging.Write("WATCH", "Add list Success: " + string.Join(",", list2.ToArray()));
							return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
						}
					}
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

	public virtual HttpResult AddDocWithUDO(DocumentsImpl doc, DataSet dsUdo, string sessionId, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts)
	{
		try
		{
			string text = "";
			string text2 = "";
			if (dsUdo.IsNotNull())
			{
				DataTable dataTable = dsUdo.Tables[0];
				DataTable dataTable2 = dsUdo.Tables[1];
				dsUdo.Tables.RemoveAt(0);
				dsUdo.Tables.RemoveAt(0);
				text2 = Function.ToString(dataTable.Rows[0]["Code"]);
				int num = ((dataTable2.Rows[0]["DocEntry"] != null) ? Function.ParseInt(dataTable2.Rows[0]["DocEntry"]) : 0);
				text = _unitOfWork.UDOs.MappingXml(dataTable, dataTable2, dsUdo, sessionId, (num > 0) ? "Update" : "Add");
			}
			string text3 = Mapping(doc, EnumCommandType.Add, objType, docObjectCode);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text3 + "</dis:AddObject>" + text + "</env:Body></env:Envelopes>";
			Logging.Write("WATCH", $"Start add new\t{objType}", text2);
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				List<string> list = new List<string>();
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					string text4 = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Add new document\t{objType}\t{text4}");
					list.Add(text4);
				}
				if (xmlElement.OuterXml.Contains("DocEntry"))
				{
					XElement xElement4 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("DocEntry"));
					if (xElement4 != null)
					{
						string text5 = Function.ToString(xElement4.Value);
						Logging.Write("WATCH", "Execute UDO Success: " + text2, text5);
						list.Add(text5);
					}
				}
				return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list.ToArray()));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public string MappingAllocateBatch(DocumentsImpl doc, EnumCommandType commandType, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts)
	{
		string text = "<BOM>\r\n                          <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                          <AdmInfo>\r\n                            <Object>" + objType.ToString() + "</Object>\r\n                          </AdmInfo>";
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><DocEntry>" + Function.ParseInt(doc.DocEntry) + "</DocEntry></QueryParams>";
			}
			text += " <Documents>\r\n                            <row>";
			if (objType == ApzObjectTypes.oDrafts)
			{
				text = text + "<DocObjectCode>" + Function.ToString((int)docObjectCode) + "</DocObjectCode>";
			}
			string text2 = Function.ToString(doc.DocStatus).ToUpper();
			text += DataService.MappingUdf(doc.UserFields, commandType);
			text += "</row>\r\n                        </Documents>";
			if (doc.Lines != null && doc.Lines.Count > 0)
			{
				text += "<Document_Lines>";
				for (int i = 0; i < doc.Lines.Count; i++)
				{
					Document_LinesImpl document_LinesImpl = doc.Lines[i];
					string text3 = Function.ToString(document_LinesImpl.LineStatus).ToUpper();
					text += "<row>";
					if (((objType == ApzObjectTypes.oDrafts || objType == ApzObjectTypes.oOrders || objType == ApzObjectTypes.oQuotations || objType == ApzObjectTypes.oPurchaseOrders || objType == ApzObjectTypes.oPurchaseQuotations || objType == ApzObjectTypes.oPurchaseRequest) && (text2.Equals("O") || text2.Equals("OPEN")) && (text3.Equals("O") || text3.Equals("OPEN"))) || commandType == EnumCommandType.Add)
					{
						if (document_LinesImpl.LineNum >= 0)
						{
							text = text + "<LineNum>" + Function.ParseInt(document_LinesImpl.LineNum) + "</LineNum>";
						}
						text += DataService.MappingUdf(document_LinesImpl.UserFields, commandType);
						text += "</row>";
					}
				}
				text += "</Document_Lines>";
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					text += "<BatchNumbers>";
					for (int j = 0; j < doc.Lines.Count; j++)
					{
						if (doc.Lines[j].BatchNumbers == null || doc.Lines[j].BatchNumbers.Count <= 0)
						{
							continue;
						}
						for (int k = 0; k < doc.Lines[j].BatchNumbers.Count; k++)
						{
							text += "<row>";
							BatchNumbersImpl batchNumbersImpl = doc.Lines[j].BatchNumbers[k];
							if (!string.IsNullOrEmpty(batchNumbersImpl.BatchNumber))
							{
								text = text + "<BatchNumber>" + Function.ToString(batchNumbersImpl.BatchNumber) + "</BatchNumber>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.ManufacturerSerialNumber))
							{
								text = text + "<ManufacturerSerialNumber>" + Function.RawText(batchNumbersImpl.ManufacturerSerialNumber) + "</ManufacturerSerialNumber>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.InternalSerialNumber))
							{
								text = text + "<InternalSerialNumber>" + Function.RawText(batchNumbersImpl.InternalSerialNumber) + "</InternalSerialNumber>";
							}
							if (batchNumbersImpl.ExpiryDate.HasValue && batchNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ExpiryDate>" + Convert.ToDateTime(batchNumbersImpl.ExpiryDate).ToString("yyyyMMdd") + "</ExpiryDate>";
							}
							if (batchNumbersImpl.ManufacturingDate.HasValue && batchNumbersImpl.ManufacturingDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ManufacturingDate>" + Convert.ToDateTime(batchNumbersImpl.ManufacturingDate).ToString("yyyyMMdd") + "</ManufacturingDate>";
							}
							if (batchNumbersImpl.AddmisionDate.HasValue && batchNumbersImpl.AddmisionDate > new DateTime(1900, 1, 1))
							{
								text = text + "<AddmisionDate>" + Convert.ToDateTime(batchNumbersImpl.AddmisionDate).ToString("yyyyMMdd") + "</AddmisionDate>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Location))
							{
								text = text + "<Location>" + Function.RawText(batchNumbersImpl.Location) + "</Location>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Notes))
							{
								text = text + "<Notes>" + Function.RawText(batchNumbersImpl.Notes) + "</Notes>";
							}
							if (commandType == EnumCommandType.Update || batchNumbersImpl.Quantity > 0.0)
							{
								text = text + "<Quantity>" + Function.ParseDecimal(batchNumbersImpl.Quantity) + "</Quantity>";
							}
							if (batchNumbersImpl.BaseLineNumber >= 0)
							{
								text = text + "<BaseLineNumber>" + Function.ParseInt(batchNumbersImpl.BaseLineNumber) + "</BaseLineNumber>";
							}
							if (0 < batchNumbersImpl.TrackingNote)
							{
								text = text + "<TrackingNote>" + Function.RawText(batchNumbersImpl.TrackingNote) + "</TrackingNote>";
							}
							if (0 < batchNumbersImpl.TrackingNoteLine)
							{
								text = text + "<TrackingNoteLine>" + Function.RawText(batchNumbersImpl.TrackingNoteLine) + "</TrackingNoteLine>";
							}
							text += DataService.MappingUdf(batchNumbersImpl.UserFields);
							text += "</row>";
						}
					}
					text += "</BatchNumbers>";
				}
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					text += "<SerialNumbers>";
					for (int l = 0; l < doc.Lines.Count; l++)
					{
						if (doc.Lines[l].SerialNumbers == null || doc.Lines[l].SerialNumbers.Count <= 0)
						{
							continue;
						}
						for (int m = 0; m < doc.Lines[l].SerialNumbers.Count; m++)
						{
							text += "<row>";
							SerialNumbersImpl serialNumbersImpl = doc.Lines[l].SerialNumbers[m];
							if (!string.IsNullOrEmpty(serialNumbersImpl.BatchID))
							{
								text = text + "<BatchID>" + Function.ToString(serialNumbersImpl.BatchID) + "</BatchID>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.ManufacturerSerialNumber))
							{
								text = text + "<ManufacturerSerialNumber>" + Function.RawText(serialNumbersImpl.ManufacturerSerialNumber) + "</ManufacturerSerialNumber>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.InternalSerialNumber))
							{
								text = text + "<InternalSerialNumber>" + Function.RawText(serialNumbersImpl.InternalSerialNumber) + "</InternalSerialNumber>";
							}
							if (serialNumbersImpl.ExpiryDate.HasValue && serialNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ExpiryDate>" + Convert.ToDateTime(serialNumbersImpl.ExpiryDate).ToString("yyyyMMdd") + "</ExpiryDate>";
							}
							if (serialNumbersImpl.ManufactureDate.HasValue && serialNumbersImpl.ManufactureDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ManufacturingDate>" + Convert.ToDateTime(serialNumbersImpl.ManufactureDate).ToString("yyyyMMdd") + "</ManufacturingDate>";
							}
							if (serialNumbersImpl.ReceptionDate.HasValue && serialNumbersImpl.ReceptionDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ReceptionDate>" + Convert.ToDateTime(serialNumbersImpl.ReceptionDate).ToString("yyyyMMdd") + "</ReceptionDate>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.Location))
							{
								text = text + "<Location>" + Function.RawText(serialNumbersImpl.Location) + "</Location>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.Notes))
							{
								text = text + "<Notes>" + Function.RawText(serialNumbersImpl.Notes) + "</Notes>";
							}
							if (commandType == EnumCommandType.Update || serialNumbersImpl.Quantity > 0.0)
							{
								text = text + "<Quantity>" + Function.ParseDecimal(serialNumbersImpl.Quantity) + "</Quantity>";
							}
							if (serialNumbersImpl.BaseLineNumber >= 0)
							{
								text = text + "<BaseLineNumber>" + Function.ParseInt(serialNumbersImpl.BaseLineNumber) + "</BaseLineNumber>";
							}
							if (0 < serialNumbersImpl.TrackingNote)
							{
								text = text + "<TrackingNote>" + Function.RawText(serialNumbersImpl.TrackingNote) + "</TrackingNote>";
							}
							if (0 < serialNumbersImpl.TrackingNoteLine)
							{
								text = text + "<TrackingNoteLine>" + Function.RawText(serialNumbersImpl.TrackingNoteLine) + "</TrackingNoteLine>";
							}
							text += DataService.MappingUdf(serialNumbersImpl.UserFields);
							text += "</row>";
						}
					}
					text += "</SerialNumbers>";
				}
			}
			text += "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public virtual string SaveFacebookOrder(DocumentsImpl doc, BPImpls dtBp, string sessionId)
	{
		try
		{
			string text = "";
			if (dtBp != null && !string.IsNullOrEmpty(dtBp.CardCode))
			{
				string cardCode = (doc.CardCode = (dtBp.CardCode = string.Format("F{0}", DateTime.Now.ToString("yyyyMMddHHmmss"))));
				if (dtBp.Addresses != null)
				{
					for (int i = 0; i < dtBp.Addresses.Count; i++)
					{
						dtBp.Addresses[i].CardCode = cardCode;
					}
				}
				text = _unitOfWork.BusinessPartners.MappingDis(dtBp, EnumCommandType.Add, sessionId);
			}
			text = text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(doc, EnumCommandType.Add, ApzObjectTypes.oOrders) + "</dis:AddObject>";
			string text4 = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(text4).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Add list error: " + xElement2.Value);
					StringBuilder stringBuilder = new StringBuilder("/**********BEGIN Log for debug [Send XML: ");
					stringBuilder.Append(text4);
					stringBuilder.Append("]" + Environment.NewLine);
					stringBuilder.Append("[Return XML: ");
					stringBuilder.Append(xmlElement.OuterXml);
					stringBuilder.Append("]END Log for debug**********/" + Environment.NewLine);
					Logging.Write("ERROR", stringBuilder.ToString());
					return Function.ToString(xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				List<XElement> list = (from t in xElement.Descendants()
					where t.Name.LocalName.Equals("RetKey")
					select t).ToList();
				if (list.Count > 0)
				{
					List<string> list2 = new List<string>();
					list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
					if (list2.Count > 0)
					{
						Logging.Write("WATCH", "Add list Success: " + string.Join(",", list2.ToArray()));
						return "";
					}
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

	public List<ApzObjectTypes> ListObjectUpdateLine()
	{
		return new List<ApzObjectTypes>
		{
			ApzObjectTypes.oOrders,
			ApzObjectTypes.oQuotations,
			ApzObjectTypes.oReturnRequest,
			ApzObjectTypes.oPurchaseOrders,
			ApzObjectTypes.oPurchaseQuotations,
			ApzObjectTypes.oPurchaseRequest,
			ApzObjectTypes.oGoodsReturnRequest,
			ApzObjectTypes.oInventoryTransferRequest,
			ApzObjectTypes.oDrafts,
			ApzObjectTypes.oStockTransferDraft
		};
	}
}
