using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaServiceCallRepository : ServiceCallRepository
{
	public HanaServiceCallRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	protected override string UpdateData(DataRow doc, DataTable line, DataTable activiti, DataTable expenses, DataTable solution, List<AttachmentFile_Impl> attachment, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			string xmlObject = Mapping(doc, line, activiti, expenses, solution, attachment);
			return DataService.UpdateObject(objType, sessionId, xmlObject).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected override HttpResult AddNew(DataRow doc, DataTable line, DataTable activiti, DataTable expenses, DataTable solution, List<AttachmentFile_Impl> attachment, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			string xmlObject = Mapping(doc, line, activiti, expenses, solution, attachment);
			return DataService.AddObject(objType, sessionId, xmlObject);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string Mapping(DataRow doc, DataTable line, DataTable activiti, DataTable expenses, DataTable solution, List<AttachmentFile_Impl> attachment)
	{
		string text = "{";
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(doc["subject"])))
			{
				text += Function.GetJsonString("Subject", Function.ToString(doc["subject"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["customer"])))
			{
				text += Function.GetJsonString("CustomerCode", Function.ToString(doc["customer"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["custmrName"])))
			{
				text += Function.GetJsonString("CustomerName", Function.ToString(doc["custmrName"]));
			}
			if (Function.ParseInt(doc["contctCode"]) != 0)
			{
				text += Function.GetJsonString("ContactCode", Function.ParseInt(doc["contctCode"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["manufSN"])))
			{
				text += Function.GetJsonString("ManufacturerSerialNum", Function.ToString(doc["manufSN"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["internalSN"])))
			{
				text += Function.GetJsonString("InternalSerialNum", Function.ToString(doc["internalSN"]));
			}
			if (Function.ParseInt(doc["contractID"]) == 0)
			{
				text += Function.GetJsonString("ContractID", Function.ParseInt(doc["contractID"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["resolDate"])) && Function.ParseDateTimes(line.Rows[0]["resolDate"]) > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("ResolutionDate", Function.ParseDateTimes(line.Rows[0]["resolDate"]).ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["resolTime"])))
			{
				text += Function.GetJsonString("ResolutionTime", Function.ToString(line.Rows[0]["resolTime"]));
			}
			if (Function.ParseInt(line.Rows[0]["origin"]) != 0)
			{
				text += Function.GetJsonString("Origin", Function.ParseInt(line.Rows[0]["origin"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["itemCode"])))
			{
				text += Function.GetJsonString("ItemCode", Function.ToString(doc["itemCode"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["itemName"])))
			{
				text += Function.GetJsonString("ItemDescription", Function.ToString(doc["itemName"]));
			}
			if (Function.ParseInt(doc["status"]) != 0)
			{
				text += Function.GetJsonString("Status", Function.ParseInt(doc["status"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["priority"])))
			{
				text += Function.GetJsonString("Priority", Function.ToString(doc["priority"]));
			}
			if (Function.ParseInt(line.Rows[0]["callType"]) != 0)
			{
				text += Function.GetJsonString("CallType", Function.ParseInt(line.Rows[0]["callType"]));
			}
			if (Function.ParseInt(line.Rows[0]["problemTyp"]) != 0)
			{
				text += Function.GetJsonString("ProblemType", Function.ParseInt(line.Rows[0]["problemTyp"]));
			}
			if (Function.ParseInt(line.Rows[0]["assignee"]) != 0)
			{
				text += Function.GetJsonString("AssigneeCode", Function.ParseInt(line.Rows[0]["assignee"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["descrption"])))
			{
				text += Function.GetJsonString("Description", Function.ToString(line.Rows[0]["descrption"]));
			}
			if (Function.ParseInt(line.Rows[0]["technician"]) != 0)
			{
				text += Function.GetJsonString("TechnicianCode", Function.ParseInt(line.Rows[0]["technician"]));
			}
			text += Function.GetJsonString("Resolution", Function.ToString(line.Rows[0]["resolution"]));
			if (!string.IsNullOrEmpty(Function.ToString(doc["createDate"])) && Function.ParseDateTimes(doc["createDate"]) > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("CreationDate", Function.ParseDateTimes(doc["createDate"]).ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["createTime"])))
			{
				text += Function.GetJsonString("CreationTime", Function.ParseInt(doc["createTime"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["isQueue"])))
			{
				text += Function.GetJsonString("BelongsToAQueue", Function.ToString(line.Rows[0]["isQueue"]));
			}
			if (Function.ParseInt(doc["status"]) == -1)
			{
				text = (string.IsNullOrEmpty(Function.ToString(doc["closeTime"])) ? (text + Function.GetJsonString("ClosingTime", DateTime.Now.TimeOfDay)) : (text + Function.GetJsonString("ClosingTime", Function.ToString(doc["closeTime"]))));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["Queue"])))
			{
				text += Function.GetJsonString("Queue", Function.ToString(line.Rows[0]["Queue"]));
			}
			if (Function.ParseInt(doc["status"]) == -1)
			{
				text = ((string.IsNullOrEmpty(Function.ToString(doc["closeDate"])) || !(Function.ParseDateTimes(doc["closeDate"]) > new DateTime(1900, 1, 1))) ? (text + Function.GetJsonString("ClosingDate", DateTime.Now.ToString("yyyyMMdd"))) : (text + Function.GetJsonString("ClosingDate", Function.ParseDateTimes(doc["closeDate"]).ToString("yyyyMMdd"))));
			}
			if (Function.ParseInt(doc["Series"]) != 0)
			{
				text += Function.GetJsonString("Series", Function.ParseInt(doc["Series"]));
			}
			if (Function.ParseInt(doc["DocNum"]) != 0)
			{
				text += Function.GetJsonString("DocNum", Function.ParseInt(doc["DocNum"]));
			}
			if (Function.ParseInt(doc["Series"]) == -1)
			{
				text += Function.GetJsonString("HandWritten", "Y");
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["StartDate"])) && Function.ParseDateTimes(line.Rows[0]["StartDate"]) > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("StartDate", Function.ParseDateTimes(line.Rows[0]["StartDate"]).ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["StartTime"])))
			{
				text += Function.GetJsonString("StartTime", Function.ToString(line.Rows[0]["StartTime"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["EndDate"])) && Function.ParseDateTimes(line.Rows[0]["EndDate"]) > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("EndDueDate", Function.ParseDateTimes(line.Rows[0]["EndDate"]).ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["EndTime"])))
			{
				text += Function.GetJsonString("EndTime", Function.ToString(line.Rows[0]["EndTime"]));
			}
			if (Function.ParseInt(line.Rows[0]["Duration"]) != 0)
			{
				text += Function.GetJsonString("Duration", Function.ParseInt(line.Rows[0]["Duration"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["DurType"])))
			{
				text += Function.GetJsonString("DurationType", Function.ToString(line.Rows[0]["DurType"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["Reminder"])))
			{
				text += Function.GetJsonString("Reminder", Function.ToString(line.Rows[0]["Reminder"]));
			}
			if (Function.ParseInt(line.Rows[0]["RemQty"]) != 0)
			{
				text += Function.GetJsonString("ReminderPeriod", Function.ParseInt(line.Rows[0]["RemQty"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["RemType"])))
			{
				text += Function.GetJsonString("ReminderType", Function.ToString(line.Rows[0]["RemType"]));
			}
			if (Function.ParseInt(line.Rows[0]["Location"]) != 0)
			{
				text += Function.GetJsonString("Location", Function.ParseInt(line.Rows[0]["Location"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["AddrName"])))
			{
				text += Function.GetJsonString("AddressName", Function.ToString(line.Rows[0]["AddrName"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["AddrType"])))
			{
				text += Function.GetJsonString("AddressType", Function.ToString(line.Rows[0]["AddrType"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["Street"])))
			{
				text += Function.GetJsonString("Street", Function.ToString(line.Rows[0]["Street"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["City"])))
			{
				text += Function.GetJsonString("City", Function.ToString(line.Rows[0]["City"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["Room"])))
			{
				text += Function.GetJsonString("Room", Function.ToString(line.Rows[0]["Room"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["State"])))
			{
				text += Function.GetJsonString("State", Function.ToString(line.Rows[0]["State"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["Country"])))
			{
				text += Function.GetJsonString("Country", Function.ToString(line.Rows[0]["Country"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(line.Rows[0]["DisplInCal"])))
			{
				text += Function.GetJsonString("DisplayInCalendar", Function.ToString(line.Rows[0]["DisplInCal"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc["NumAtCard"])))
			{
				text += Function.GetJsonString("CustomerRefNo", Function.ToString(doc["NumAtCard"]));
			}
			if (Function.ParseInt(line.Rows[0]["ProSubType"]) != 0)
			{
				text += Function.GetJsonString("ProblemSubType", Function.ParseInt(line.Rows[0]["ProSubType"]));
			}
			text = ((attachment == null || attachment.Count <= 0) ? (text + Function.GetJsonString("AttachmentEntry", null)) : (text + Function.GetJsonString("AttachmentEntry", Function.ParseInt(attachment[0].AbsEntry))));
			text += "\"ServiceCallActivities\": [";
			if (activiti != null && activiti.Rows.Count > 0)
			{
				for (int i = 0; i < activiti.Rows.Count; i++)
				{
					text += "{";
					if (Function.ParseInt(activiti.Rows[i]["ClgCode"]) != 0)
					{
						text += Function.GetJsonString("ActivityCode", Function.ParseInt(activiti.Rows[i]["ClgCode"]));
					}
					text += "},";
				}
			}
			text += "],";
			text += "\"ServiceCallInventoryExpenses\": [";
			if (expenses != null && expenses.Rows.Count > 0)
			{
				for (int j = 0; j < expenses.Rows.Count; j++)
				{
					text += "{";
					if (!string.IsNullOrEmpty(Function.ToString(expenses.Rows[j]["Object"])))
					{
						text += Function.GetJsonString("DocumentType", Function.ToString(expenses.Rows[j]["Object"]));
					}
					if (Function.ParseInt(expenses.Rows[j]["DocNumber"]) != 0)
					{
						text += Function.GetJsonString("DocumentNumber", Function.ParseInt(expenses.Rows[j]["DocNumber"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(expenses.Rows[j]["StckTrnDir"])))
					{
						text += Function.GetJsonString("StockTransferDirection", Function.ToString(expenses.Rows[j]["StckTrnDir"]));
					}
					text += "}";
				}
			}
			text += "],";
			text += "\"ServiceCallSolutions\": [";
			if (solution != null && solution.Rows.Count > 0)
			{
				for (int k = 0; k < solution.Rows.Count; k++)
				{
					text += "{";
					if (Function.ParseInt(solution.Rows[k]["SltCode"]) != 0)
					{
						text += Function.GetJsonString("SolutionID", Function.ParseInt(solution.Rows[k]["SltCode"]));
					}
					text += "}";
				}
			}
			else
			{
				text += Function.GetJsonString("SolutionID", -1);
			}
			text += "]";
			text += "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}
}
