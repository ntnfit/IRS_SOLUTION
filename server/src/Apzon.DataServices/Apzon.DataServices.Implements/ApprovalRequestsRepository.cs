using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Commons.Kit;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;
using Apzon.Services;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class ApprovalRequestsRepository : Repository<DataTable>, IApprovalRequests
{
	protected readonly IUnitOfWork DataServiceWork;

	public ApprovalRequestsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public ApprovalRequestsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork dataServiceWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		DataServiceWork = dataServiceWork;
	}

	public string ApproveDraft(string sessionId, string user, int userId, DataTable dt, string serverPath = "")
	{
		int num = Function.ParseInt(dt.Rows[0]["WddCode"]);
		string password = Function.ToString(dt.Rows[0]["Password"]);
		string text = Function.ToString(dt.Rows[0]["status"]);
		string text2 = Function.ToString(dt.Rows[0]["Remarks"]);
		DataSet approvalRequest = GetApprovalRequest(num, sessionId);
		DataTable dataTable = approvalRequest.Tables["ApprovalRequestLine"];
		DataTable dataTable2 = approvalRequest.Tables["ApprovalRequest"];
		bool flag = false;
		if (dataTable.IsNotNull() && dataTable2.IsNotNull())
		{
			int num2 = Function.ParseInt(dataTable2.Rows[0]["CurrentStage"]);
			foreach (DataRow row2 in dataTable.Rows)
			{
				int num3 = Function.ParseInt(row2["UserID"]);
				string text3 = Function.ToString(row2["Status"]);
				int num4 = Function.ParseInt(row2["StageCode"]);
				if (userId == num3 && text3.Equals("ardPending") && num4 == num2)
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			return "Current approval stage is belong to other User";
		}
		string text4 = "";
		ProcessApprovalDraft(decision: (text == "Y") ? "ardApproved" : ((!(text == "N")) ? "ardPending" : "ardNotApproved"), sessionId: sessionId, wddCode: num, userName: user, password: password, remarks: text2);
		DataTable dataTable3 = ExecuteDataTable("SELECT T0.* FROM OWDD T0,APZ_OADM T1 WHERE T0.\"Status\"='Y' AND T1.\"AutoSaveDraftApproval\"='Y' AND T0.\"WddCode\"=@WddCode;", CommandType.Text, new SqlParameter("@WddCode", num));
		if (dataTable3.IsNotNull())
		{
			int draftKey = Function.ParseInt(dataTable3.Rows[0]["DraftEntry"]);
			int num5 = Function.ParseInt(dataTable3.Rows[0]["ObjType"]);
			List<Document_LinesImpl> list = new List<Document_LinesImpl>();
			DocumentsImpl documentsImpl;
			if (new int[2] { 1250000001, 67 }.Contains(num5))
			{
				documentsImpl = new DocumentsImpl
				{
					draftKey = draftKey,
					DocObjectCode = (ApzObjectTypes)num5
				};
			}
			else
			{
				DataTable dataTable4 = ServiceUnitOfWork.Documents.CopyFrom("112", draftKey.ToString(), num5.ToString(), -1, "");
				DataTable dataTable5 = ServiceUnitOfWork.Documents.CopyFromLine("112", num5.ToString(), draftKey.ToString());
				DataTable dataTable6 = ServiceUnitOfWork.Commons.CreateFreightGridDatasource("I");
				foreach (DataRow row in ServiceUnitOfWork.Commons.GetFreight("DRF3", draftKey.ToString()).Rows)
				{
					DataRow dataRow2 = dataTable6.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ExpnsCode"]) == Function.ParseInt(row["ExpnsCode"]));
					if (dataRow2 != null)
					{
						dataRow2["LineTotal"] = Function.ParseDouble(row["LineTotal"]);
						dataRow2["TotalFrgn"] = Function.ParseDouble(row["TotalFrgn"]);
						dataRow2["Comments"] = Function.ToString(row["Comments"]);
						dataRow2["VatGroup"] = Function.ToString(row["VatGroup"]);
						dataRow2["VatPrcnt"] = Function.ParseDouble(row["VatPrcnt"]);
						dataRow2["VatSum"] = Function.ParseDouble(row["VatSum"]);
						dataRow2["DistrbMthd"] = Function.ToString(row["DistrbMthd"]);
						dataRow2["GrsAmount"] = Function.ParseDouble(row["GrsAmount"]);
						dataRow2["OcrCode"] = Function.ToString(row["OcrCode"]);
						dataRow2["OcrCode2"] = Function.ToString(row["OcrCode2"]);
						dataRow2["OcrCode3"] = Function.ToString(row["OcrCode3"]);
						dataRow2["OcrCode4"] = Function.ToString(row["OcrCode4"]);
						dataRow2["OcrCode5"] = Function.ToString(row["OcrCode5"]);
						dataRow2["Project"] = Function.ToString(row["Project"]);
						dataRow2["BaseType"] = Function.ParseInt(row["BaseType"]);
						dataRow2["BaseAbsEnt"] = Function.ParseInt(row["BaseAbsEnt"]);
						dataRow2["BaseLnNum"] = Function.ParseInt(row["BaseLnNum"]);
					}
				}
				dataTable6.AcceptChanges();
				if (dataTable4 != null && dataTable4.Rows.Count > 0)
				{
					documentsImpl = dataTable4.Rows[0].ToDocument();
					DataRow[] array = (from t in dataTable6.Copy().AsEnumerable()
						where !string.IsNullOrEmpty(Function.ToString(t["ExpnsCode"])) && (Function.ParseDouble(t["LineTotal"]) != 0.0 || Function.ParseDouble(t["TotalFrgn"]) != 0.0)
						select t).ToArray();
					if (array.Length != 0)
					{
						documentsImpl.Expenses = array.CopyToDataTable().ToDocumentsAdditional();
					}
				}
				else
				{
					documentsImpl = null;
				}
				if (dataTable5 != null && dataTable5.Rows.Count > 0)
				{
					list = dataTable5.ToDocumentLine(priceSpecified: true);
					list = list.Where((Document_LinesImpl x) => !string.IsNullOrEmpty(x.ItemCode)).ToList();
					if (list.Count > 0)
					{
						DataTable dataTable7 = ServiceUnitOfWork.Documents.CopyBatchSerial(draftKey.ToString(), "112");
						if (dataTable7.IsNotNull())
						{
							DataTable dataTable8 = DocumentsHelper.BatchSerialNumbersStruct.Copy();
							string[] columnNames = (from DataColumn t in dataTable8.Columns
								select t.ColumnName).ToArray();
							dataTable7 = dataTable7.DefaultView.ToTable("Source", distinct: false, columnNames);
							DataRow dataRow3 = dataTable8.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["DistNumber"]).Equals(""));
							if (dataRow3 != null)
							{
								dataTable8.Rows.Remove(dataRow3);
								dataTable8.AcceptChanges();
							}
							foreach (DataRow row3 in dataTable7.Rows)
							{
								DataRow dataRow5 = dataTable8.NewRow();
								dataRow5.ItemArray = row3.ItemArray;
								dataTable8.Rows.Add(dataRow5);
							}
						}
						foreach (Document_LinesImpl line in list)
						{
							DataRow[] array2 = (from t in dataTable7.AsEnumerable()
								where Function.ParseInt(t["BaseLineNumber"]) == line.LineNum
								select t).ToArray();
							if (array2.Length != 0)
							{
								line.LoadDataBatch(array2.CopyToDataTable(), (ApzObjectTypes)num5);
							}
						}
					}
				}
			}
			if (documentsImpl != null)
			{
				documentsImpl.Lines = list;
				documentsImpl.DocObjectCode = (ApzObjectTypes)num5;
				documentsImpl.ObjType = num5;
				DataTable dt2 = ServiceUnitOfWork.IssueForProduction.LoadAttachFiles(documentsImpl.AtcEntry);
				if (dt2.IsNotNull())
				{
					documentsImpl.AttachmentFiles = EntitiesHelper<AttachmentFile_Impl>.ToEntities(dt2);
				}
				HttpResult httpResult = DataServiceWork.Documents.SaveDraftToDocument(documentsImpl, sessionId);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					Logging.Write("ERROR", httpResult.Message + " " + num + " " + draftKey);
				}
				else
				{
					Logging.Write("WATCH", " Add draft to document " + httpResult.Content);
				}
			}
		}
		try
		{
			bool flag2 = false;
			MailHelper mailHelper = new MailHelper();
			DataTable dataTable9 = ExecuteDataTable("SELECT T0.\"WddCode\",t0.\"WtmCode\",t0.\"CurrStep\",t2.\"UserID\",T0.\"DraftEntry\",T0.\"ObjType\",t0.\"OwnerID\",T3.\"USER_CODE\"\r\n                                                        FROM owdd T0\r\n                                                        INNER JOIN OWST T1  ON t0.\"CurrStep\" = T1.\"WstCode\"\r\n                                                        INNER JOIN WST1 T2 ON t1.\"WstCode\" = t2.\"WstCode\"\r\n                                                        INNER JOIN OUSR T3 ON T2.\"UserID\"=T3.\"USERID\"\r\n                                                        WHERE \"WddCode\" = @WddCode AND t0.\"Status\" = 'W';", CommandType.Text, new SqlParameter("@WddCode", num));
			Logging.Write("WATCH", "1.get next stage");
			if (dataTable9.IsNotNull())
			{
				int num6 = Function.ParseInt(dataTable9.Rows[0]["WtmCode"]);
				int num7 = Function.ParseInt(dataTable9.Rows[0]["DraftEntry"]);
				int num8 = Function.ParseInt(dataTable9.Rows[0]["ObjType"]);
				int num9 = Function.ParseInt(dataTable9.Rows[0]["OwnerID"]);
				DataSet data = ServiceUnitOfWork.ContextAction.GetData("H$ApprovalDecision", num6.ToString());
				if (data.IsNotNull() && data.Tables[0].IsNotNull())
				{
					flag2 = true;
					mailHelper.EmailConfig = ServiceUnitOfWork.MailSetting.GetDoc();
					mailHelper.UnlockedUsers = ServiceUnitOfWork.SubActivities.GetUnlockedUsers();
					Logging.Write("WATCH", "2.get email config");
				}
				bool flag3 = false;
				foreach (DataRow row4 in dataTable9.Rows)
				{
					try
					{
						string text5 = Function.ToString(row4["UserID"]);
						string text6 = Function.ToString(row4["USER_CODE"]);
						Logging.Write("WATCH", "3.receipt user " + text6);
						if (flag2)
						{
							DataTable dataTable10 = new DataTable();
							string query = "apz_approval_emailcontent";
							Logging.Write("WATCH", "4.get email content wddcode " + num + " wtmcode " + num6 + " draftkey " + num7 + " objtype " + num8 + " remarks " + text2 + " reqid " + num9 + " receipt " + text5);
							dataTable10 = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@WddCode", num), new SqlParameter("@WtmCode", num6), new SqlParameter("@DraftKey", num7), new SqlParameter("@ObjType", num8), new SqlParameter("@Remarks", text2), new SqlParameter("@ReqId", num9), new SqlParameter("@ReceiptId", text5));
							if (dataTable10.IsNotNull())
							{
								ActionHandle actionHandle = new ActionHandle();
								MailImpl mailContent = actionHandle.GetMailContent(data, dataTable10);
								if (mailContent != null)
								{
									string text7 = Function.ToString(dataTable10.Rows[0]["DbName"]);
									string encryptedText = Function.ToString(dataTable10.Rows[0]["Pwd"]);
									string api = Function.ToString(dataTable10.Rows[0]["ApiAddress"]);
									Guid guid = Guid.NewGuid();
									string attPath = serverPath + "\\" + text7 + "\\Action";
									mailContent.BodyText = GetMailBodyText(text7, text6, Function.Decrypt(RSA.PrivateKey, encryptedText), num, api, guid.ToString());
									string commandText = "";
									string fileName = "";
									ReportDocument crystalReport = actionHandle.GetCrystalReport(data, dataTable10, attPath, out commandText, out fileName);
									if (!string.IsNullOrEmpty(commandText))
									{
										DataTable dataSource = ExecuteDataTable(commandText, CommandType.Text);
										crystalReport.SetDataSource(dataSource);
										crystalReport.Refresh();
										mailContent.AttachStream = new Dictionary<string, Stream>();
										mailContent.AttachStream[fileName + ".pdf"] = crystalReport.ExportToStream(ExportFormatType.PortableDocFormat);
										ExecuteNonQuery(GetUpdateExUrlQuery(), CommandType.Text, new SqlParameter("@Guid", guid));
										Logging.Write("WATCH", "5.send mail " + mailContent.AddressTo.FirstOrDefault());
										string text8 = mailHelper.SendEmail(num9, mailContent.AddressTo, mailContent.Subject, mailContent.BodyText, mailContent.AddressFrom, mailContent.AddressCc, mailContent.AddressBcc, mailContent.AttachStream);
										Logging.Write("WATCH", "6.sent");
										Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text8);
									}
									else
									{
										Logging.Write("WATCH", "could not get cmd text report");
									}
								}
								else
								{
									Logging.Write("WATCH", "could not get email content handle.getEmailContent");
								}
							}
							else
							{
								Logging.Write("WATCH", "could not get email content");
							}
						}
						else
						{
							Logging.Write("WATCH", "not setup auto email");
						}
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", ex);
					}
				}
				if (!flag3)
				{
					Logging.Write("Could not send alert");
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
		}
		return string.Empty;
	}

	protected virtual string GetUpdateExUrlQuery()
	{
		return "insert into APZ_EXRL(DocEntry,Token,Status) SELECT isnull((SELECT Max(DocEntry) FROM APZ_EXRL),0)+1,@Guid,'N'";
	}

	protected virtual void ProcessApprovalDraft(string sessionId, int wddCode, string userName, string password, string decision, string remarks)
	{
		string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ApprovalRequestsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <UpdateRequest xmlns=\"ApprovalRequestsService\">\r\n                                     <ApprovalRequest>\r\n                                            <Code>" + wddCode + "</Code>\r\n                                            <ApprovalRequestDecisions>\r\n                                            <ApprovalRequestDecision>\r\n                                                <ApproverUserName>" + userName + "</ApproverUserName>\r\n                                                <ApproverPassword>" + EncodePassword.DecryptString(password, RSA.PasswordPhrase) + "</ApproverPassword>\r\n                                                <Status>" + decision + "</Status>\r\n                                                <Remarks>" + remarks + "</Remarks>\r\n                                             </ApprovalRequestDecision>\r\n                                             </ApprovalRequestDecisions>\r\n                                    </ApprovalRequest>\r\n                                    </UpdateRequest>\r\n                                </soap:Body>\r\n                            </soap:Envelope>";
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
	}

	private string GetMailBodyText(string dbname, string username, string password, int wddcode, string api, string guid)
	{
		string value = Function.Encrypt(RSA.PublicKey, dbname + ";" + username + ";" + password + ";" + wddcode);
		value = WebUtility.UrlEncode(value);
		guid = WebUtility.UrlEncode(guid);
		return "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">  <html xmlns= \"http://www.w3.org/1999/xhtml\">\r\n                        <head><meta http-equiv=\"Content -Type\" content=\"text/html; charset=utf-8\"/><title></title>\r\n                        </head>   \r\n                       <body>  \r\n                        <span>\r\n                        Yêu cầu phê duyệt được gửi từ hệ thống.\r\n                        </span>\r\n                        <br/>\r\n                        Vui lòng nhấn vào đường dẫn dưới đây để thực hiện phê duyệt\r\n                        <br/>\r\n                        <a href=\"" + api + "/api/ApprovalDecision?token=" + guid + "&key=" + value + "\">" + api + "/api/ApprovalDecision?token=" + guid + "&key=" + value + "</a>\r\n                        </body>\r\n                </html> ";
	}

	public virtual DataSet GetApprovalRequest(int wddCode, string sessionId)
	{
		string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ApprovalRequestsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetApprovalRequest xmlns=\"ApprovalRequestsService\">\r\n                                     <ApprovalRequest>\r\n                                            <Code>" + wddCode + "</Code>\r\n                                    </ApprovalRequest>\r\n                                    </GetApprovalRequest>\r\n                                </soap:Body>\r\n                            </soap:Envelope>";
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
		XElement xElement3 = XElement.Parse(xmlElement.OuterXml).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("ApprovalRequest"));
		DataSet dataSet = new DataSet();
		using (StringReader reader = new StringReader(Function.ToString(xElement3)))
		{
			dataSet.ReadXml(reader);
		}
		if (!dataSet.Tables.Contains("ApprovalRequestLine") || !dataSet.Tables.Contains("ApprovalRequest"))
		{
			throw new Exception("Could not get approval request");
		}
		return dataSet;
	}

	public virtual string CancelApprovalRequest(int wddCode, string sessionId)
	{
		string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ApprovalRequestsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <UpdateRequest xmlns=\"ApprovalRequestsService\">\r\n                                     <ApprovalRequest>\r\n                                            <Code>" + wddCode + "</Code>\r\n                                            <Status>" + Function.ToString(BoApprovalRequestStatusEnum.arsCancelled) + "</Status>\r\n                                    </ApprovalRequest>\r\n                                    </UpdateRequest>\r\n                                </soap:Body>\r\n                            </soap:Envelope>";
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
}
