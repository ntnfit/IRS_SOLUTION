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
using Apzon.Entities.Models.Production;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class ServiceCallRepository : Repository<DataTable>, IServiceCall, IRepository<DataTable>, IDisposable
{
	public ServiceCallRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public void Dispose()
	{
		Dispose();
	}

	public string Save(DataRow doc, DataTable line, DataTable activiti, DataTable expenses, DataTable solution, List<AttachmentFile_Impl> attachment, string sessionId)
	{
		try
		{
			if (doc != null)
			{
				if (attachment != null)
				{
					string text = SaveAttachmentFiles(attachment, attachment[0].AbsEntry);
					if (!string.IsNullOrWhiteSpace(text))
					{
						return text;
					}
				}
				if (!Exist(Function.ParseInt(doc["callID"])))
				{
					return AddNew(doc, line, activiti, expenses, solution, attachment, sessionId, ApzObjectTypes.oServiceCalls).Message;
				}
				return UpdateData(doc, line, activiti, expenses, solution, attachment, sessionId, ApzObjectTypes.oServiceCalls);
			}
			return "Loi du lieu!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected virtual string UpdateData(DataRow doc, DataTable line, DataTable activiti, DataTable expenses, DataTable solution, List<AttachmentFile_Impl> attachment, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			DataService.GetBusinessObjectTemplate(sessionId, objType.ToString());
			string text = Mapping(doc, line, activiti, expenses, solution, attachment, sessionId, EnumCommandType.Update, objType);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:UpdateObject></env:Body></env:Envelopes>";
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
					Logging.Write("WATCH", $"update document - {objType} - DocEntry - {num}");
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

	protected virtual HttpResult AddNew(DataRow doc, DataTable line, DataTable activiti, DataTable expenses, DataTable solution, List<AttachmentFile_Impl> attachment, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			string text = Mapping(doc, line, activiti, expenses, solution, attachment, sessionId, EnumCommandType.Add, objType);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:AddObject></env:Body></env:Envelopes>";
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
					Logging.Write("WATCH", $"Add new document - {objType} - DocEntry - {num}");
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

	private string Mapping(DataRow doc, DataTable line, DataTable activiti, DataTable expenses, DataTable solution, List<AttachmentFile_Impl> attachment, string sessionId, EnumCommandType commandType, ApzObjectTypes objType)
	{
		string text = "<BOM>\r\n                          <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                          <AdmInfo>\r\n                            <Object>" + objType.ToString() + "</Object>\r\n                          </AdmInfo>";
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><ServiceCallID>" + Function.ParseInt(doc["callID"]) + "</ServiceCallID></QueryParams>";
			}
			text += " <ServiceCalls>\r\n                            <row>";
			if (!string.IsNullOrEmpty(Function.ToString(doc["subject"])))
			{
				text = text + "<Subject>" + Function.RawText(doc["subject"]) + "</Subject>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["customer"])))
			{
				text = text + "<CustomerCode>" + Function.RawText(doc["customer"]) + "</CustomerCode>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["custmrName"])))
			{
				text = text + "<CustomerName>" + Function.RawText(doc["custmrName"]) + "</CustomerName>";
			}
			if (Function.ParseInt(doc["contctCode"]) != 0)
			{
				text = text + "<ContactCode>" + Function.ParseInt(doc["contctCode"]) + "</ContactCode>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["manufSN"])))
			{
				text = text + "<ManufacturerSerialNum>" + Function.RawText(doc["manufSN"]) + "</ManufacturerSerialNum>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["internalSN"])))
			{
				text = text + "<InternalSerialNum>" + Function.RawText(doc["internalSN"]) + "</InternalSerialNum>";
			}
			if (Function.ParseInt(doc["contractID"]) == 0)
			{
				text = text + "<ContractID>" + Function.ParseInt(doc["contractID"]) + "</ContractID>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["resolDate"])) && Function.ParseDateTimes(line.Rows[0]["resolDate"]) > new DateTime(1900, 1, 1))
			{
				text = text + "<ResolutionDate>" + Function.ParseDateTimes(line.Rows[0]["resolDate"]).ToString("yyyyMMdd") + "</ResolutionDate>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["resolTime"])))
			{
				text = text + "<ResolutionTime>" + Function.ToString(line.Rows[0]["resolTime"]) + "</ResolutionTime>";
			}
			if (Function.ParseInt(line.Rows[0]["origin"]) != 0)
			{
				text = text + "<Origin>" + Function.ParseInt(line.Rows[0]["origin"]) + "</Origin>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["itemCode"])))
			{
				text = text + "<ItemCode>" + Function.RawText(doc["itemCode"]) + "</ItemCode>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["itemName"])))
			{
				text = text + "<ItemDescription>" + Function.RawText(doc["itemName"]) + "</ItemDescription>";
			}
			if (Function.ParseInt(doc["status"]) != 0)
			{
				text = text + "<Status>" + Function.ParseInt(doc["status"]) + "</Status>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["priority"])))
			{
				text = text + "<Priority>" + Function.ParseChar(doc["priority"]) + "</Priority>";
			}
			if (Function.ParseInt(line.Rows[0]["callType"]) != 0)
			{
				text = text + "<CallType>" + Function.ParseInt(line.Rows[0]["callType"]) + "</CallType>";
			}
			if (Function.ParseInt(line.Rows[0]["problemTyp"]) != 0)
			{
				text = text + "<ProblemType>" + Function.ParseInt(line.Rows[0]["problemTyp"]) + "</ProblemType>";
			}
			if (Function.ParseInt(line.Rows[0]["assignee"]) != 0)
			{
				text = text + "<AssigneeCode>" + Function.ParseInt(line.Rows[0]["assignee"]) + "</AssigneeCode>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["descrption"])))
			{
				text = text + "<Description>" + Function.RawText(line.Rows[0]["descrption"]) + "</Description>";
			}
			if (Function.ParseInt(line.Rows[0]["technician"]) != 0)
			{
				text = text + "<TechnicianCode>" + Function.ParseInt(line.Rows[0]["technician"]) + "</TechnicianCode>";
			}
			text = text + "<Resolution>" + Function.ToString(line.Rows[0]["resolution"]) + "</Resolution>";
			if (!string.IsNullOrEmpty(Function.ToString(doc["createDate"])) && Function.ParseDateTimes(doc["createDate"]) > new DateTime(1900, 1, 1))
			{
				text = text + "<CreationDate >" + Function.ParseDateTimes(doc["createDate"]).ToString("yyyyMMdd") + "</CreationDate >";
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["createTime"])))
			{
				text = text + "<CreationTime >" + Function.ToString(doc["createTime"]) + "</CreationTime >";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["isQueue"])))
			{
				text = text + "<BelongsToAQueue>" + Function.ParseChar(line.Rows[0]["isQueue"]) + "</BelongsToAQueue>";
			}
			if (Function.ParseInt(doc["status"]) == -1)
			{
				text = (string.IsNullOrEmpty(Function.ToString(doc["closeTime"])) ? (text + "<ClosingTime>" + DateTime.Now.TimeOfDay.ToString() + "</ClosingTime >") : (text + "<ClosingTime>" + Function.ToString(doc["closeTime"]) + "</ClosingTime >"));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["Queue"])))
			{
				text = text + "<Queue>" + Function.ParseChar(line.Rows[0]["Queue"]) + "</Queue>";
			}
			if (Function.ParseInt(doc["status"]) == -1)
			{
				text = ((string.IsNullOrEmpty(Function.ToString(doc["closeDate"])) || !(Function.ParseDateTimes(doc["closeDate"]) > new DateTime(1900, 1, 1))) ? (text + "<ClosingDate>" + DateTime.Now.ToString("yyyyMMdd") + "</ClosingDate >") : (text + "<ClosingDate>" + Function.ParseDateTimes(doc["closeDate"]).ToString("yyyyMMdd") + "</ClosingDate >"));
			}
			if (Function.ParseInt(doc["Series"]) != 0)
			{
				text = text + "<Series>" + Function.ParseInt(doc["Series"]) + "</Series>";
			}
			if (Function.ParseInt(doc["DocNum"]) != 0)
			{
				text = text + "<DocNum>" + Function.ParseInt(doc["DocNum"]) + "</DocNum>";
			}
			if (Function.ParseInt(doc["Series"]) == -1)
			{
				text += "<HandWritten>Y</HandWritten>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["StartDate"])) && Function.ParseDateTimes(line.Rows[0]["StartDate"]) > new DateTime(1900, 1, 1))
			{
				text = text + "<StartDate>" + Function.ParseDateTimes(line.Rows[0]["StartDate"]).ToString("yyyyMMdd") + "</StartDate>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["StartTime"])))
			{
				text = text + "<StartTime>" + Function.ToString(line.Rows[0]["StartTime"]) + "</StartTime >";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["EndDate"])) && Function.ParseDateTimes(line.Rows[0]["EndDate"]) > new DateTime(1900, 1, 1))
			{
				text = text + "<EndDueDate>" + Function.ParseDateTimes(line.Rows[0]["EndDate"]).ToString("yyyyMMdd") + "</EndDueDate>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["EndTime"])))
			{
				text = text + "<EndTime>" + Function.ToString(line.Rows[0]["EndTime"]) + "</EndTime >";
			}
			if (Function.ParseInt(line.Rows[0]["Duration"]) != 0)
			{
				text = text + "<Duration>" + Function.ParseInt(line.Rows[0]["Duration"]) + "</Duration>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["DurType"])))
			{
				text = text + "<DurationType>" + Function.ParseChar(line.Rows[0]["DurType"]) + "</DurationType>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["Reminder"])))
			{
				text = text + "<Reminder>" + Function.ParseChar(line.Rows[0]["Reminder"]) + "</Reminder>";
			}
			if (Function.ParseInt(line.Rows[0]["RemQty"]) != 0)
			{
				text = text + "<ReminderPeriod>" + Function.ParseInt(line.Rows[0]["RemQty"]) + "</ReminderPeriod>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["RemType"])))
			{
				text = text + "<ReminderType>" + Function.ParseChar(line.Rows[0]["RemType"]) + "</ReminderType>";
			}
			if (Function.ParseInt(line.Rows[0]["Location"]) != 0)
			{
				text = text + "<Location>" + Function.ParseInt(line.Rows[0]["Location"]) + "</Location>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["AddrName"])))
			{
				text = text + "<AddressName>" + Function.RawText(line.Rows[0]["AddrName"]) + "</AddressName>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["AddrType"])))
			{
				text = text + "<AddressType>" + Function.ParseChar(line.Rows[0]["AddrType"]) + "</AddressType>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["Street"])))
			{
				text = text + "<Street>" + Function.RawText(line.Rows[0]["Street"]) + "</Street>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["City"])))
			{
				text = text + "<City>" + Function.RawText(line.Rows[0]["City"]) + "</City>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["Room"])))
			{
				text = text + "<Room>" + Function.RawText(line.Rows[0]["Room"]) + "</Room>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["State"])))
			{
				text = text + "<State>" + Function.RawText(line.Rows[0]["State"]) + "</State>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["Country"])))
			{
				text = text + "<Country>" + Function.RawText(line.Rows[0]["Country"]) + "</Country>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["DisplInCal"])))
			{
				text = text + "<DisplayInCalendar>" + Function.ParseChar(line.Rows[0]["DisplInCal"]) + "</DisplayInCalendar>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["NumAtCard"])))
			{
				text = text + "<CustomerRefNo>" + Function.ToString(doc["NumAtCard"]) + "</CustomerRefNo>";
			}
			if (Function.ParseInt(line.Rows[0]["ProSubType"]) != 0)
			{
				text = text + "<ProblemSubType>" + Function.ParseInt(line.Rows[0]["ProSubType"]) + "</ProblemSubType>";
			}
			text += "</row>\r\n                        </ServiceCalls>";
			text += "<ServiceCallActivities>";
			if (activiti != null && activiti.Rows.Count > 0)
			{
				for (int i = 0; i < activiti.Rows.Count; i++)
				{
					text += "<row>";
					if (Function.ParseInt(activiti.Rows[i]["ClgCode"]) != 0)
					{
						text = text + "<ActivityCode>" + Function.ParseInt(activiti.Rows[i]["ClgCode"]) + "</ActivityCode>";
					}
					text += "</row>";
				}
			}
			text += "</ServiceCallActivities>";
			text += "<ServiceCallInventoryExpenses>";
			if (expenses != null && expenses.Rows.Count > 0)
			{
				for (int j = 0; j < expenses.Rows.Count; j++)
				{
					text += "<row>";
					if (!string.IsNullOrEmpty(Function.ToString(expenses.Rows[j]["Object"])))
					{
						text = text + "<DocumentType>" + Function.ToString(expenses.Rows[j]["Object"]) + "</DocumentType>";
					}
					if (Function.ParseInt(expenses.Rows[j]["DocNumber"]) != 0)
					{
						text = text + "<DocumentNumber>" + Function.ParseInt(expenses.Rows[j]["DocNumber"]) + "</DocumentNumber>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(expenses.Rows[j]["StckTrnDir"])))
					{
						text = text + "<StockTransferDirection>" + Function.ParseChar(expenses.Rows[j]["StckTrnDir"]) + "</StockTransferDirection>";
					}
					text += "</row>";
				}
			}
			else
			{
				text += "</ServiceCallInventoryExpenses>";
			}
			text += "<ServiceCallSolutions>";
			if (solution != null && solution.Rows.Count > 0)
			{
				for (int k = 0; k < solution.Rows.Count; k++)
				{
					text += "<row>";
					if (Function.ParseInt(solution.Rows[k]["SltCode"]) != 0)
					{
						text = text + "<SolutionID>" + Function.ParseInt(solution.Rows[k]["SltCode"]) + "</SolutionID>";
					}
					text += "</row>";
				}
			}
			else
			{
				text = text + "<SolutionID>" + -1 + "</SolutionID>";
			}
			text += "</ServiceCallSolutions>";
			text += "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	private bool Exist(int callID)
	{
		try
		{
			if (Function.ParseInt(ExecuteScarlar("select top 1 1 from OSCL where \"callID\" = @callID;", CommandType.Text, new SqlParameter("@callID", callID))) < 1)
			{
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	private string SaveAttachmentFiles(List<AttachmentFile_Impl> lstFiles, int absEntry)
	{
		try
		{
			ITransaction transaction = new Transaction();
			if (lstFiles != null && lstFiles.Count > 0)
			{
				if (!CheckFile(absEntry))
				{
					string text = SaveFile(absEntry);
					if (!string.IsNullOrEmpty(text))
					{
						transaction.Rollback();
						return text;
					}
					DeleteFile(absEntry);
					foreach (AttachmentFile_Impl lstFile in lstFiles)
					{
						string text2 = SaveLineFile(lstFile);
						if (!string.IsNullOrEmpty(text2))
						{
							transaction.Rollback();
							return text2;
						}
					}
				}
				else
				{
					DeleteFile(absEntry);
					foreach (AttachmentFile_Impl lstFile2 in lstFiles)
					{
						string text3 = SaveLineFile(lstFile2);
						if (!string.IsNullOrEmpty(text3))
						{
							transaction.Rollback();
							return text3;
						}
					}
				}
			}
			else
			{
				string text4 = DeleteFile(absEntry);
				if (!string.IsNullOrEmpty(text4))
				{
					transaction.Rollback();
					return text4;
				}
			}
			transaction.Commit();
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private bool CheckFile(int absEntry)
	{
		try
		{
			string query = "SELECT AbsEntry FROM oatc  WHERE\r\n                        AbsEntry = " + absEntry;
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	public string SaveFile(int absEntry)
	{
		try
		{
			string query = "insert into oatc (absEntry) values (" + absEntry + ")";
			if (ExecuteNonQuery(query, CommandType.Text) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC file oatc table FAILED ", ex.Message);
			return ex.Message;
		}
	}

	private string DeleteFile(int absEntry)
	{
		try
		{
			string query = "delete from ATC1 WHERE absEntry = " + absEntry;
			if (ExecuteNonQuery(query, CommandType.Text) >= 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public string SaveLineFile(AttachmentFile_Impl line)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string text;
			if (!CheckLineFile(line.AbsEntry, line.Line))
			{
				text = "INSERT INTO [dbo].[ATC1]\r\n                               ([AbsEntry]\r\n                               ,[Line]\r\n                               ,[srcPath]\r\n                               ,[trgtPath]\r\n                               ,[FileName]\r\n                               ,[FileExt]\r\n                               ,[Date]\r\n                               ,[UsrID]\r\n                               ,[Copied]\r\n                               ,[Override]\r\n                               ,[subPath])\r\n                         VALUES (";
				text += "@AbsEntry ";
				list.Add(new SqlParameter("@AbsEntry", Function.ParseInt(line.AbsEntry)));
				text += ",@Line ";
				list.Add(new SqlParameter("@Line", Function.ParseInt(line.Line)));
				text += ", @srcPath ";
				list.Add(new SqlParameter("@srcPath", Function.ToString(line.srcPath)));
				text += ",@trgtPath ";
				list.Add(new SqlParameter("@trgtPath", Function.ToString(line.trgtPath)));
				text += ",@FileName ";
				list.Add(new SqlParameter("@FileName", Function.ToString(line.FileName)));
				text += ", @FileExt ";
				list.Add(new SqlParameter("@FileExt", Function.ToString(line.FileExt)));
				text += ",@Date ";
				list.Add(new SqlParameter("@Date", Function.ParseDateTime(line.Date)));
				text += ",@UsrID ";
				list.Add(new SqlParameter("@UsrID", Function.ParseInt(line.UsrID)));
				text += ", @Copied ";
				list.Add(new SqlParameter("@Copied", Function.ToString(line.Copied)));
				text += ",@Override ";
				list.Add(new SqlParameter("@Override", Function.ToString(line.Override)));
				text += ", @subPath ";
				list.Add(new SqlParameter("@subPath", Function.ToString(line.subPath)));
				text += " )";
			}
			else
			{
				text = "update [dbo].[ATC1] set ";
				text += "srcPath=@srcPath ";
				list.Add(new SqlParameter("@srcPath", Function.ToString(line.srcPath)));
				text += ",trgtPath=@trgtPath ";
				list.Add(new SqlParameter("@trgtPath", Function.ToString(line.trgtPath)));
				text += ",FileName=@FileName ";
				list.Add(new SqlParameter("@FileName", Function.ToString(line.FileName)));
				text += ", FileExt=@FileExt ";
				list.Add(new SqlParameter("@FileExt", Function.ToString(line.FileExt)));
				text += ",Date=@Date ";
				list.Add(new SqlParameter("@Date", Function.ParseDateTime(line.Date)));
				text += ",UsrID=@UsrID ";
				list.Add(new SqlParameter("@UsrID", Function.ParseInt(line.UsrID)));
				text += ",Copied=@Copied ";
				list.Add(new SqlParameter("@Copied", Function.ToString(line.Copied)));
				text += ",Override=@Override ";
				list.Add(new SqlParameter("@Override", Function.ToString(line.Override)));
				text += ", subPath=@subPath ";
				list.Add(new SqlParameter("@subPath", Function.ToString(line.subPath)));
				text += " where AbsEntry=@AbsEntry ";
				list.Add(new SqlParameter("@AbsEntry", Function.ParseInt(line.AbsEntry)));
				text += " and Line=@Line ";
				list.Add(new SqlParameter("@Line", Function.ParseInt(line.Line)));
			}
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC LINE FAILED ", ex.Message);
			return ex.Message;
		}
	}

	private bool CheckLineFile(int absEntry, int line)
	{
		try
		{
			string query = "SELECT AbsEntry FROM atc1  WHERE\r\n                        AbsEntry = " + absEntry + " and  Line = " + line;
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}
}
