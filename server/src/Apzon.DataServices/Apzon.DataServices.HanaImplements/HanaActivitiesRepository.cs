using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaActivitiesRepository : ActivitiesRepository
{
	public HanaActivitiesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	protected override HttpResult SaveAttachment(List<AttachmentFile_Impl> lstFile, DataTable dtAction, string sessionId, bool addNew)
	{
		int num = 0;
		if (!addNew)
		{
			num = Function.ParseInt(ServiceUnitOfWork.Commons.ExecuteScalar("select \"AtcEntry\" from OCLG where \"ClgCode\"=@clgCode;", CommandType.Text, new SqlParameter("@clgCode", Function.ParseInt(dtAction.Rows[0]["ClgCode"]))));
		}
		if (addNew || num <= 0)
		{
			return _unitOfWork.Attachment2.Add(lstFile, sessionId);
		}
		return _unitOfWork.Attachment2.Update(num, lstFile, sessionId);
	}

	protected override string Add(DataRow drAction, string sessionId, int atcEntry)
	{
		try
		{
			string xmlObject = Mapping(drAction, atcEntry);
			HttpResult httpResult = DataService.AddObject(ApzObjectTypes.oActivitySubject, sessionId, xmlObject);
			return (httpResult.MessageCode != Enums.MessageCode.Success) ? httpResult.Message : ("OK:" + httpResult.Content);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected override string Update(DataRow drAction, string sessionId, int atcEntry)
	{
		try
		{
			string xmlObject = Mapping(drAction, atcEntry);
			HttpResult httpResult = DataService.UpdateObject(ApzObjectTypes.oActivitySubject, sessionId, xmlObject);
			return (httpResult.MessageCode != Enums.MessageCode.Success) ? httpResult.Message : ("OK:" + httpResult.Content);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string Mapping(DataRow drAction, int atcEntry)
	{
		try
		{
			string text = "{";
			if (ColumnExist(drAction, "ClgCode") && Function.ParseInt(drAction["ClgCode"]) > 0)
			{
				text += Function.GetJsonString("ActivityCode", Function.ParseInt(drAction["ClgCode"]));
			}
			if (ColumnExist(drAction, "Action") && !string.IsNullOrEmpty(Function.ToString(drAction["Action"])))
			{
				switch (Function.ToString(drAction["Action"]))
				{
				case "M":
					text += Function.GetJsonString("Activity", Function.ToString(BoActivities.cn_Meeting));
					break;
				case "T":
					text += Function.GetJsonString("Activity", Function.ToString(BoActivities.cn_Task));
					break;
				case "E":
					text += Function.GetJsonString("Activity", Function.ToString(BoActivities.cn_Note));
					break;
				case "P":
					text += Function.GetJsonString("Activity", Function.ToString(BoActivities.cn_Campaign));
					break;
				case "N":
					text += Function.GetJsonString("Activity", Function.ToString(BoActivities.cn_Other));
					break;
				}
			}
			if (ColumnExist(drAction, "CntctDate") && !string.IsNullOrEmpty(Function.ToString(drAction["CntctDate"])))
			{
				text += Function.GetJsonString("ActivityDate", Function.ParseDateTimes(drAction["CntctDate"]).ToString("yyyy-MM-dd"));
			}
			if (ColumnExist(drAction, "CntctTime") && !string.IsNullOrEmpty(Function.ToString(drAction["CntctTime"])))
			{
				text += Function.GetJsonString("ActivityTime", Function.ParseInt(drAction["CntctTime"]));
			}
			if (ColumnExist(drAction, "CntctType") && Function.ParseInt(drAction["CntctType"]) != 0)
			{
				text += Function.GetJsonString("ActivityType", Function.ParseInt(drAction["CntctType"]));
			}
			if (ColumnExist(drAction, "AddrName") && !string.IsNullOrEmpty(Function.ToString(drAction["AddrName"])))
			{
				text += Function.GetJsonString("AddressName", Function.ToString(drAction["AddrName"]));
			}
			if (ColumnExist(drAction, "AddrType") && !string.IsNullOrEmpty(Function.ToString(drAction["AddrType"])))
			{
				string text2 = Function.ToString(drAction["AddrType"]);
				if (!(text2 == "S"))
				{
					if (text2 == "B")
					{
						text += Function.GetJsonString("AddressType", Function.ToString(BoAddressType.bo_BillTo));
					}
				}
				else
				{
					text += Function.GetJsonString("AddressType", Function.ToString(BoAddressType.bo_ShipTo));
				}
			}
			text = ((atcEntry <= 0) ? (text + Function.GetJsonString("AttachmentEntry", null)) : (text + Function.GetJsonString("AttachmentEntry", atcEntry)));
			if (ColumnExist(drAction, "CntctTime") && !string.IsNullOrEmpty(Function.ToString(drAction["CardCode"])))
			{
				text += Function.GetJsonString("CardCode", Function.ToString(drAction["CardCode"]));
			}
			if (ColumnExist(drAction, "city") && !string.IsNullOrEmpty(Function.ToString(drAction["city"])))
			{
				text += Function.GetJsonString("City", Function.ToString(drAction["city"]));
			}
			if (ColumnExist(drAction, "Closed") && !string.IsNullOrEmpty(Function.ToString(drAction["Closed"])))
			{
				string text2 = Function.ToString(drAction["Closed"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("Closed", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("Closed", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			if (ColumnExist(drAction, "CloseDate") && !string.IsNullOrEmpty(Function.ToString(drAction["CloseDate"])))
			{
				text += Function.GetJsonString("CloseDate", Function.ParseDateTimes(drAction["CloseDate"]).ToString("yyyy-MM-dd"));
			}
			if (ColumnExist(drAction, "CntctCode") && Function.ParseInt(drAction["CntctCode"]) > 0)
			{
				text += Function.GetJsonString("ContactPersonCode", Function.ParseInt(drAction["CntctCode"]));
			}
			if (ColumnExist(drAction, "country") && !string.IsNullOrEmpty(Function.ToString(drAction["country"])))
			{
				text += Function.GetJsonString("Country", Function.ToString(drAction["country"]));
			}
			if (ColumnExist(drAction, "Details") && !string.IsNullOrEmpty(Function.ToString(drAction["Details"])))
			{
				text += Function.GetJsonString("Details", Function.ToString(drAction["Details"]));
			}
			if (ColumnExist(drAction, "DocEntry") && !string.IsNullOrEmpty(Function.ToString(drAction["DocEntry"])))
			{
				text += Function.GetJsonString("DocEntry", Function.ToString(drAction["DocEntry"]));
			}
			if (ColumnExist(drAction, "DocType") && !string.IsNullOrEmpty(Function.ToString(drAction["DocType"])))
			{
				text += Function.GetJsonString("DocType", Function.ToString(drAction["DocType"]));
			}
			if (ColumnExist(drAction, "Duration") && Function.ParseDecimal(drAction["Duration"]) > 0m)
			{
				text += Function.GetJsonString("Duration", Function.ParseDecimal(drAction["Duration"]));
			}
			if (ColumnExist(drAction, "DurType") && !string.IsNullOrEmpty(Function.ToString(drAction["DurType"])))
			{
				switch (Function.ToString(drAction["DurType"]))
				{
				case "M":
					text += Function.GetJsonString("DurationType", Function.ToString(BoDurations.du_Minuts));
					break;
				case "H":
					text += Function.GetJsonString("DurationType", Function.ToString(BoDurations.du_Hours));
					break;
				case "D":
					text += Function.GetJsonString("DurationType", Function.ToString(BoDurations.du_Days));
					break;
				}
			}
			if (ColumnExist(drAction, "endDate") && !string.IsNullOrEmpty(Function.ToString(drAction["endDate"])))
			{
				text += Function.GetJsonString("EndDueDate", Function.ParseDateTimes(drAction["endDate"]).ToString("yyyy-MM-dd"));
			}
			if (ColumnExist(drAction, "ENDTime") && Function.ParseInt(drAction["ENDTime"]) > 0)
			{
				text += Function.GetJsonString("EndTime", Function.ParseInt(drAction["ENDTime"]));
			}
			if (ColumnExist(drAction, "EndType") && !string.IsNullOrEmpty(Function.ToString(drAction["EndType"])))
			{
				switch (Function.ToString(drAction["EndType"]))
				{
				case "N":
					text += Function.GetJsonString("EndType", Function.ToString(EndTypeEnum.etNoEndDate));
					break;
				case "C":
					text += Function.GetJsonString("EndType", Function.ToString(EndTypeEnum.etByCounter));
					break;
				case "D":
					text += Function.GetJsonString("EndType", Function.ToString(EndTypeEnum.etByDate));
					break;
				}
			}
			if (ColumnExist(drAction, "Fax") && !string.IsNullOrEmpty(Function.ToString(drAction["Fax"])))
			{
				text += Function.GetJsonString("Fax", Function.ToString(drAction["Fax"]));
			}
			if (ColumnExist(drAction, "Friday") && !string.IsNullOrEmpty(Function.ToString(drAction["Friday"])))
			{
				string text2 = Function.ToString(drAction["Friday"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("Friday", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("Friday", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			if (ColumnExist(drAction, "AttendUser") && Function.ParseInt(drAction["AttendUser"]) > 0)
			{
				text += Function.GetJsonString("HandledBy", Function.ParseInt(drAction["AttendUser"]));
			}
			if (ColumnExist(drAction, "inactive") && !string.IsNullOrEmpty(Function.ToString(drAction["inactive"])))
			{
				string text2 = Function.ToString(drAction["inactive"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("InactiveFlag", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("InactiveFlag", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			if (ColumnExist(drAction, "Interval") && Function.ParseInt(drAction["Interval"]) > 0)
			{
				text += Function.GetJsonString("Interval", Function.ParseInt(drAction["Interval"]));
			}
			if (ColumnExist(drAction, "Location") && Function.ParseInt(drAction["Location"]) != 0)
			{
				text += Function.GetJsonString("Location", Function.ParseInt(drAction["Location"]));
			}
			if (ColumnExist(drAction, "MaxOccur") && Function.ParseInt(drAction["MaxOccur"]) != 0)
			{
				text += Function.GetJsonString("MaxOccurrence", Function.ParseInt(drAction["MaxOccur"]));
			}
			if (ColumnExist(drAction, "Monday") && !string.IsNullOrEmpty(Function.ToString(drAction["Monday"])))
			{
				string text2 = Function.ToString(drAction["Monday"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("Monday", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("Monday", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			if (ColumnExist(drAction, "CntctTime") && !string.IsNullOrEmpty(Function.ToString(drAction["Notes"])))
			{
				text += Function.GetJsonString("Notes", Function.ToString(drAction["Notes"]));
			}
			if (ColumnExist(drAction, "personal") && !string.IsNullOrEmpty(Function.ToString(drAction["personal"])))
			{
				string text2 = Function.ToString(drAction["personal"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("PersonalFlag", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("PersonalFlag", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			if (ColumnExist(drAction, "Tel") && !string.IsNullOrEmpty(Function.ToString(drAction["Tel"])))
			{
				text += Function.GetJsonString("Phone", Function.ToString(drAction["Tel"]));
			}
			if (ColumnExist(drAction, "prevActvty") && Function.ParseInt(drAction["prevActvty"]) > 0)
			{
				text += Function.GetJsonString("PreviousActivity", Function.ParseInt(drAction["prevActvty"]));
			}
			if (ColumnExist(drAction, "Priority") && !string.IsNullOrEmpty(Function.ToString(drAction["Priority"])))
			{
				switch (Function.ToString(drAction["Priority"]))
				{
				case "0":
					text += Function.GetJsonString("Priority", Function.ToString(BoMsgPriorities.pr_Low));
					break;
				case "1":
					text += Function.GetJsonString("Priority", Function.ToString(BoMsgPriorities.pr_Normal));
					break;
				case "2":
					text += Function.GetJsonString("Priority", Function.ToString(BoMsgPriorities.pr_High));
					break;
				}
			}
			if (ColumnExist(drAction, "DayInMonth") && Function.ParseInt(drAction["DayInMonth"]) != 0)
			{
				text += Function.GetJsonString("RecurrenceDayInMonth", Function.ParseInt(drAction["DayInMonth"]));
			}
			if (ColumnExist(drAction, "DayOfWeek") && Function.ParseInt(drAction["DayOfWeek"]) != 0)
			{
				switch (Function.ToString(drAction["DayOfWeek"]))
				{
				case "0":
					text += Function.GetJsonString("RecurrenceDayOfWeek", Function.ToString(RecurrenceDayOfWeekEnum.rdowDay));
					break;
				case "1":
					text += Function.GetJsonString("RecurrenceDayOfWeek", Function.ToString(RecurrenceDayOfWeekEnum.rdowWeekDay));
					break;
				case "2":
					text += Function.GetJsonString("RecurrenceDayOfWeek", Function.ToString(RecurrenceDayOfWeekEnum.rdowWeekendDay));
					break;
				case "3":
					text += Function.GetJsonString("RecurrenceDayOfWeek", Function.ToString(RecurrenceDayOfWeekEnum.rdowSun));
					break;
				case "4":
					text += Function.GetJsonString("RecurrenceDayOfWeek", Function.ToString(RecurrenceDayOfWeekEnum.rdowMon));
					break;
				case "5":
					text += Function.GetJsonString("RecurrenceDayOfWeek", Function.ToString(RecurrenceDayOfWeekEnum.rdowTue));
					break;
				case "6":
					text += Function.GetJsonString("RecurrenceDayOfWeek", Function.ToString(RecurrenceDayOfWeekEnum.rdowWed));
					break;
				case "7":
					text += Function.GetJsonString("RecurrenceDayOfWeek", Function.ToString(RecurrenceDayOfWeekEnum.rdowThu));
					break;
				case "8":
					text += Function.GetJsonString("RecurrenceDayOfWeek", Function.ToString(RecurrenceDayOfWeekEnum.rdowFri));
					break;
				case "9":
					text += Function.GetJsonString("RecurrenceDayOfWeek", Function.ToString(RecurrenceDayOfWeekEnum.rdowSat));
					break;
				}
			}
			if (ColumnExist(drAction, "Month") && Function.ParseInt(drAction["Month"]) != 0)
			{
				text += Function.GetJsonString("RecurrenceMonth", Function.ParseInt(drAction["Month"]));
			}
			if (ColumnExist(drAction, "RecurPat") && !string.IsNullOrEmpty(Function.ToString(drAction["RecurPat"])))
			{
				switch (Function.ToString(drAction["RecurPat"]))
				{
				case "N":
					text += Function.GetJsonString("RecurrencePattern", Function.ToString(RecurrencePatternEnum.rpNone));
					break;
				case "D":
					text += Function.GetJsonString("RecurrencePattern", Function.ToString(RecurrencePatternEnum.rpDaily));
					break;
				case "W":
					text += Function.GetJsonString("RecurrencePattern", Function.ToString(RecurrencePatternEnum.rpWeekly));
					break;
				case "M":
					text += Function.GetJsonString("RecurrencePattern", Function.ToString(RecurrencePatternEnum.rpMonthly));
					break;
				case "A":
					text += Function.GetJsonString("RecurrencePattern", Function.ToString(RecurrencePatternEnum.rpAnnually));
					break;
				}
			}
			if (ColumnExist(drAction, "Week") && Function.ParseInt(drAction["Week"]) != 0)
			{
				switch (Function.ToString(drAction["Week"]))
				{
				case "1":
					text += Function.GetJsonString("RecurrenceSequenceSpecifier", "rsFirst");
					break;
				case "2":
					text += Function.GetJsonString("RecurrenceSequenceSpecifier", "rsSecond");
					break;
				case "3":
					text += Function.GetJsonString("RecurrenceSequenceSpecifier", "rsThird");
					break;
				case "4":
					text += Function.GetJsonString("RecurrenceSequenceSpecifier", "rsFourth");
					break;
				case "5":
					text += Function.GetJsonString("RecurrenceSequenceSpecifier", "rssLast");
					break;
				}
			}
			if (ColumnExist(drAction, "Reminder") && !string.IsNullOrEmpty(Function.ToString(drAction["Reminder"])))
			{
				string text2 = Function.ToString(drAction["Reminder"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("Reminder", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("Reminder", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			if (ColumnExist(drAction, "RemTime") && Function.ParseInt(drAction["RemTime"]) > 0)
			{
				text += Function.GetJsonString("ReminderPeriod", Function.ParseInt(drAction["RemTime"]));
			}
			if (ColumnExist(drAction, "RemType") && !string.IsNullOrEmpty(Function.ToString(drAction["RemType"])))
			{
				switch (Function.ToString(drAction["RemType"]))
				{
				case "M":
					text += Function.GetJsonString("ReminderType", Function.ToString(BoDurations.du_Minuts));
					break;
				case "H":
					text += Function.GetJsonString("ReminderType", Function.ToString(BoDurations.du_Hours));
					break;
				case "D":
					text += Function.GetJsonString("ReminderType", Function.ToString(BoDurations.du_Days));
					break;
				}
			}
			if (ColumnExist(drAction, "SubOption") && !string.IsNullOrEmpty(Function.ToString(drAction["SubOption"])))
			{
				string text2 = Function.ToString(drAction["SubOption"]);
				if (!(text2 == "1"))
				{
					if (text2 == "2")
					{
						text += Function.GetJsonString("RepeatOption", Function.ToString(RepeatOptionEnum.roByWeekDay));
					}
				}
				else
				{
					text += Function.GetJsonString("RepeatOption", Function.ToString(RepeatOptionEnum.roByDate));
				}
			}
			if (ColumnExist(drAction, "room") && !string.IsNullOrEmpty(Function.ToString(drAction["room"])))
			{
				text += Function.GetJsonString("Room", Function.RawText(drAction["room"]));
			}
			if (ColumnExist(drAction, "SlpCode") && Function.ParseInt(drAction["SlpCode"]) > 0)
			{
				text += Function.GetJsonString("SalesEmployee", Function.ParseInt(drAction["SlpCode"]));
			}
			if (ColumnExist(drAction, "AttendEmpl") && Function.ParseInt(drAction["AttendEmpl"]) > 0)
			{
				text += Function.GetJsonString("HandledByEmployee", Function.ParseInt(drAction["AttendEmpl"]));
			}
			if (ColumnExist(drAction, "OprId") && Function.ParseInt(drAction["OprId"]) > 0)
			{
				text += Function.GetJsonString("SalesOpportunityId", Function.ParseInt(drAction["OprId"]));
			}
			if (ColumnExist(drAction, "OprLine") && Function.ParseInt(drAction["OprLine"]) > 0)
			{
				text += Function.GetJsonString("SalesOpportunityLine", Function.ParseInt(drAction["OprLine"]));
			}
			if (ColumnExist(drAction, "Saturday") && !string.IsNullOrEmpty(Function.ToString(drAction["Saturday"])))
			{
				string text2 = Function.ToString(drAction["Saturday"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("Saturday", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("Saturday", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			if (ColumnExist(drAction, "SeEndDate") && !string.IsNullOrEmpty(Function.ToString(drAction["SeEndDate"])))
			{
				text += Function.GetJsonString("SeriesEndDate", Function.ParseDateTimes(drAction["SeEndDate"]).ToString("yyyyMMdd"));
			}
			if (ColumnExist(drAction, "SeStartDate") && !string.IsNullOrEmpty(Function.ToString(drAction["SeStartDate"])))
			{
				text += Function.GetJsonString("SeriesStartDate", Function.ParseDateTimes(drAction["SeStartDate"]).ToString("yyyyMMdd"));
			}
			if (ColumnExist(drAction, "Recontact") && !string.IsNullOrEmpty(Function.ToString(drAction["Recontact"])))
			{
				text += Function.GetJsonString("StartDate", Function.ParseDateTimes(drAction["Recontact"]).ToString("yyyy-MM-dd"));
			}
			if (ColumnExist(drAction, "BeginTime") && Function.ParseInt(drAction["BeginTime"]) > 0)
			{
				text += Function.GetJsonString("StartTime", Function.ParseInt(drAction["BeginTime"]));
			}
			if (ColumnExist(drAction, "state") && !string.IsNullOrEmpty(Function.ToString(drAction["state"])))
			{
				text += Function.GetJsonString("State", Function.ToString(drAction["state"]));
			}
			if (ColumnExist(drAction, "status") && Function.ParseInt(drAction["status"]) != 0)
			{
				text += Function.GetJsonString("Status", Function.ParseInt(drAction["status"]));
			}
			if (ColumnExist(drAction, "street") && !string.IsNullOrEmpty(Function.ToString(drAction["street"])))
			{
				text += Function.GetJsonString("Street", Function.ToString(drAction["street"]));
			}
			if (ColumnExist(drAction, "CntctSbjct") && Function.ParseInt(drAction["CntctSbjct"]) != 0)
			{
				text += Function.GetJsonString("Subject", Function.ParseInt(drAction["CntctSbjct"]));
			}
			if (ColumnExist(drAction, "Sunday") && !string.IsNullOrEmpty(Function.ToString(drAction["Sunday"])))
			{
				string text2 = Function.ToString(drAction["Sunday"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("Sunday", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("Sunday", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			if (ColumnExist(drAction, "tentative") && !string.IsNullOrEmpty(Function.ToString(drAction["tentative"])))
			{
				string text2 = Function.ToString(drAction["tentative"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("TentativeFlag", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("TentativeFlag", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			if (ColumnExist(drAction, "Thursday") && !string.IsNullOrEmpty(Function.ToString(drAction["Thursday"])))
			{
				string text2 = Function.ToString(drAction["Thursday"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("Thursday", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("Thursday", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			if (ColumnExist(drAction, "Tuesday") && !string.IsNullOrEmpty(Function.ToString(drAction["Tuesday"])))
			{
				string text2 = Function.ToString(drAction["Tuesday"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("Tuesday", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("Tuesday", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			if (ColumnExist(drAction, "Wednesday") && !string.IsNullOrEmpty(Function.ToString(drAction["Wednesday"])))
			{
				string text2 = Function.ToString(drAction["Wednesday"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("Wednesday", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("Wednesday", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}
}
