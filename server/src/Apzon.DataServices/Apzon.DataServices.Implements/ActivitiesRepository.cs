using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class ActivitiesRepository : Repository<Common.Table_Code>, IActivities, IRepository<Common.Table_Code>, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public ActivitiesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	protected virtual HttpResult SaveAttachment(List<AttachmentFile_Impl> lstFile, DataTable dtAction, string sessionId, bool addNew)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ClgCode,int", dtAction.Rows[0]["ClgCode"]);
		return ServiceUnitOfWork.Files.SaveAttachmentFiles(lstFile, dictionary, "OCLG");
	}

	public string Save(List<AttachmentFile_Impl> Attachments, DataTable dtAction, string sessionId)
	{
		try
		{
			bool flag = !Exists(Function.ParseInt(dtAction.Rows[0]["ClgCode"]), sessionId);
			int num = 0;
			HttpResult httpResult = SaveAttachment(Attachments, dtAction, sessionId, flag);
			if (httpResult.MessageCode != Enums.MessageCode.Success)
			{
				return httpResult.Message;
			}
			num = Function.ParseInt(httpResult.Content);
			if (flag)
			{
				return Add(dtAction.Rows[0], sessionId, num);
			}
			return Update(dtAction.Rows[0], sessionId, num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected virtual string Add(DataRow drAction, string sessionId, int atcEntry)
	{
		try
		{
			string text = Mapping(drAction, atcEntry);
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ActivitiesService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <AddActivity xmlns=\"ActivitiesService\">" + text + "</AddActivity>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
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
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("ActivityCode"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("ActivityCode"));
				if (xElement3 != null)
				{
					string text2 = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Add new Activity - {text2}");
					return "OK:" + text2;
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

	protected virtual string Update(DataRow drAction, string sessionId, int atcEntry)
	{
		try
		{
			string text = Mapping(drAction, atcEntry);
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ActivitiesService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <UpdateActivity xmlns=\"ActivitiesService\">" + text + "</UpdateActivity>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
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
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("ActivityCode"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("ActivityCode"));
				if (xElement3 != null)
				{
					string text2 = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Update new Activity - {text2}");
					return "OK:" + text2;
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

	public virtual string Delete(int actCode, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                             <BOM>\r\n                            <BO>\r\n                             <AdmInfo>\r\n                               <Object>oContacts</Object>\r\n                             </AdmInfo>\r\n                             <QueryParams>\r\n                               <ContactCode>" + actCode + "</ContactCode>\r\n                             </QueryParams>\r\n                            </BO>\r\n                           </BOM>\r\n                        </dis:RemoveObject></env:Body></env:Envelopes>";
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
					string arg = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Remove an Activity - {arg}");
					return string.Empty;
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public bool Exists(int clgCode, string sessionId)
	{
		try
		{
			if (clgCode <= 0)
			{
				return false;
			}
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ActivitiesService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetActivity xmlns=\"ActivitiesService\">\r\n                                     <ActivityParams>   <ActivityCode>" + clgCode + "</ActivityCode></ActivityParams>\r\n                                    </GetActivity>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
			XmlElement xmlElement = DataService.GetByKey(sessionId, xmlObject).Content as XmlElement;
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("ActivityCode"))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	private string Mapping(DataRow drAction, int atcEntry)
	{
		try
		{
			string text = "<Activity>";
			if (!string.IsNullOrEmpty(Function.ToString(drAction["Action"])) && ColumnExist(drAction, "Action"))
			{
				if (!string.IsNullOrEmpty(Function.ToString(drAction["ClgCode"])) && ColumnExist(drAction, "ClgCode"))
				{
					text = text + "<ActivityCode>" + Function.RawText(drAction["ClgCode"]) + "</ActivityCode>";
				}
				switch (Function.ToString(drAction["Action"]))
				{
				case "M":
					text = text + "<Activity>" + Function.ToString(BoActivities.cn_Meeting) + "</Activity>";
					break;
				case "T":
					text = text + "<Activity>" + Function.ToString(BoActivities.cn_Task) + "</Activity>";
					break;
				case "E":
					text = text + "<Activity>" + Function.ToString(BoActivities.cn_Note) + "</Activity>";
					break;
				case "P":
					text = text + "<Activity>" + Function.ToString(BoActivities.cn_Campaign) + "</Activity>";
					break;
				case "N":
					text = text + "<Activity>" + Function.ToString(BoActivities.cn_Other) + "</Activity>";
					break;
				}
			}
			if (!string.IsNullOrEmpty(Function.ToString(drAction["CntctDate"])) && ColumnExist(drAction, "CntctDate"))
			{
				text = text + "<ActivityDate>" + Convert.ToDateTime(drAction["CntctDate"]).ToString("yyyy-MM-dd") + "</ActivityDate>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(drAction["CntctTime"])) && ColumnExist(drAction, "CntctTime"))
			{
				text = text + "<ActivityTime>" + ConvertToTime(Function.ToString(drAction["CntctTime"])) + "</ActivityTime>";
			}
			if (ColumnExist(drAction, "CntctType") && Function.ParseInt(drAction["CntctType"]) != 0)
			{
				text = text + "<ActivityType>" + Function.ParseInt(drAction["CntctType"]) + "</ActivityType>";
			}
			if (ColumnExist(drAction, "AddrName") && !string.IsNullOrEmpty(Function.ToString(drAction["AddrName"])))
			{
				text = text + "<AddressName>" + Function.RawText(drAction["AddrName"]) + "</AddressName>";
			}
			if (ColumnExist(drAction, "AddrType") && !string.IsNullOrEmpty(Function.ToString(drAction["AddrType"])))
			{
				string text2 = Function.ToString(drAction["AddrType"]);
				if (!(text2 == "S"))
				{
					if (text2 == "B")
					{
						text = text + "<AddressType>" + Function.ToString(BoAddressType.bo_BillTo) + "</AddressType>";
					}
				}
				else
				{
					text = text + "<AddressType>" + Function.ToString(BoAddressType.bo_ShipTo) + "</AddressType>";
				}
			}
			if (!string.IsNullOrEmpty(Function.ToString(drAction["CardCode"])) && ColumnExist(drAction, "CardCode"))
			{
				text = text + "<CardCode>" + Function.RawText(drAction["CardCode"]) + "</CardCode>";
			}
			if (ColumnExist(drAction, "city") && !string.IsNullOrEmpty(Function.ToString(drAction["city"])))
			{
				text = text + "<City>" + Function.RawText(drAction["city"]) + "</City>";
			}
			if (ColumnExist(drAction, "Closed") && !string.IsNullOrEmpty(Function.ToString(drAction["Closed"])))
			{
				string text2 = Function.ToString(drAction["Closed"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text = text + "<Closed>" + Function.ToString(BoYesNoEnum.tNO) + "</Closed>";
					}
				}
				else
				{
					text = text + "<Closed>" + Function.ToString(BoYesNoEnum.tYES) + "</Closed>";
				}
			}
			if (ColumnExist(drAction, "CloseDate") && !string.IsNullOrEmpty(Function.ToString(drAction["CloseDate"])))
			{
				text = text + "<CloseDate>" + Convert.ToDateTime(drAction["CloseDate"]).ToString("yyyy-MM-dd") + "</CloseDate>";
			}
			if (ColumnExist(drAction, "CntctCode") && Function.ParseInt(drAction["CntctCode"]) > 0)
			{
				text = text + "<ContactPersonCode>" + Function.ParseInt(drAction["CntctCode"]) + "</ContactPersonCode>";
			}
			if (ColumnExist(drAction, "country") && !string.IsNullOrEmpty(Function.ToString(drAction["country"])))
			{
				text = text + "<Country>" + Function.RawText(drAction["country"]) + "</Country>";
			}
			if (ColumnExist(drAction, "Details") && !string.IsNullOrEmpty(Function.ToString(drAction["Details"])))
			{
				text = text + "<Details>" + Function.RawText(drAction["Details"]) + "</Details>";
			}
			if (ColumnExist(drAction, "DocEntry") && !string.IsNullOrEmpty(Function.ToString(drAction["DocEntry"])))
			{
				text = text + "<DocEntry>" + Function.RawText(drAction["DocEntry"]) + "</DocEntry>";
			}
			if (ColumnExist(drAction, "DocType") && !string.IsNullOrEmpty(Function.ToString(drAction["DocType"])))
			{
				text = text + "<DocType>" + Function.RawText(drAction["DocType"]) + "</DocType>";
			}
			if (ColumnExist(drAction, "Duration") && Function.ParseDecimal(drAction["Duration"]) > 0m)
			{
				text = text + "<Duration>" + Function.ParseDecimal(drAction["Duration"]) + "</Duration>";
			}
			if (ColumnExist(drAction, "DurType") && !string.IsNullOrEmpty(Function.ToString(drAction["DurType"])))
			{
				switch (Function.ToString(drAction["DurType"]))
				{
				case "M":
					text = text + "<DurationType>" + Function.ToString(BoDurations.du_Minuts) + "</DurationType>";
					break;
				case "H":
					text = text + "<DurationType>" + Function.ToString(BoDurations.du_Hours) + "</DurationType>";
					break;
				case "D":
					text = text + "<DurationType>" + Function.ToString(BoDurations.du_Days) + "</DurationType>";
					break;
				}
			}
			if (ColumnExist(drAction, "endDate") && !string.IsNullOrEmpty(Function.ToString(drAction["endDate"])))
			{
				text = text + "<EndDueDate>" + Convert.ToDateTime(drAction["endDate"]).ToString("yyyy-MM-dd") + "</EndDueDate>";
			}
			if (ColumnExist(drAction, "ENDTime") && Function.ParseInt(drAction["ENDTime"]) > 0)
			{
				text = text + "<EndTime>" + ConvertToTime(Function.ToString(drAction["ENDTime"])) + "</EndTime>";
			}
			if (ColumnExist(drAction, "EndType") && !string.IsNullOrEmpty(Function.ToString(drAction["EndType"])))
			{
				switch (Function.ToString(drAction["EndType"]))
				{
				case "N":
					text = text + "<EndType>" + Function.ToString(EndTypeEnum.etNoEndDate) + "</EndType>";
					break;
				case "C":
					text = text + "<EndType>" + Function.ToString(EndTypeEnum.etByCounter) + "</EndType>";
					break;
				case "D":
					text = text + "<EndType>" + Function.ToString(EndTypeEnum.etByDate) + "</EndType>";
					break;
				}
			}
			if (ColumnExist(drAction, "Fax") && !string.IsNullOrEmpty(Function.ToString(drAction["Fax"])))
			{
				text = text + "<Fax>" + Function.RawText(drAction["Fax"]) + "</Fax>";
			}
			if (ColumnExist(drAction, "Friday") && !string.IsNullOrEmpty(Function.ToString(drAction["Friday"])))
			{
				string text2 = Function.ToString(drAction["Friday"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text = text + "<Friday>" + Function.ToString(BoYesNoEnum.tNO) + "</Friday>";
					}
				}
				else
				{
					text = text + "<Friday>" + Function.ToString(BoYesNoEnum.tYES) + "</Friday>";
				}
			}
			if (ColumnExist(drAction, "AttendUser") && Function.ParseInt(drAction["AttendUser"]) > 0)
			{
				text = text + "<HandledBy>" + Function.ParseInt(drAction["AttendUser"]) + "</HandledBy>";
			}
			if (ColumnExist(drAction, "inactive") && !string.IsNullOrEmpty(Function.ToString(drAction["inactive"])))
			{
				string text2 = Function.ToString(drAction["inactive"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text = text + "<InactiveFlag>" + Function.ToString(BoYesNoEnum.tNO) + "</InactiveFlag>";
					}
				}
				else
				{
					text = text + "<InactiveFlag>" + Function.ToString(BoYesNoEnum.tYES) + "</InactiveFlag>";
				}
			}
			if (ColumnExist(drAction, "Interval") && Function.ParseInt(drAction["Interval"]) > 0)
			{
				text = text + "<Interval>" + Function.ParseInt(drAction["Interval"]) + "</Interval>";
			}
			if (ColumnExist(drAction, "Location") && Function.ParseInt(drAction["Location"]) != 0)
			{
				text = text + "<Location>" + Function.ParseInt(drAction["Location"]) + "</Location>";
			}
			if (ColumnExist(drAction, "MaxOccur") && Function.ParseInt(drAction["MaxOccur"]) != 0)
			{
				text = text + "<MaxOccurrence>" + Function.ParseInt(drAction["MaxOccur"]) + "</MaxOccurrence>";
			}
			if (ColumnExist(drAction, "Monday") && !string.IsNullOrEmpty(Function.ToString(drAction["Monday"])))
			{
				string text2 = Function.ToString(drAction["Monday"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text = text + "<Monday>" + Function.ToString(BoYesNoEnum.tNO) + "</Monday>";
					}
				}
				else
				{
					text = text + "<Monday>" + Function.ToString(BoYesNoEnum.tYES) + "</Monday>";
				}
			}
			if (!string.IsNullOrEmpty(Function.ToString(drAction["Notes"])) && ColumnExist(drAction, "Notes"))
			{
				text = text + "<Notes>" + Function.ToString(drAction["Notes"]) + "</Notes>";
			}
			if (ColumnExist(drAction, "personal") && !string.IsNullOrEmpty(Function.ToString(drAction["personal"])))
			{
				string text2 = Function.ToString(drAction["personal"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text = text + "<PersonalFlag>" + Function.ToString(BoYesNoEnum.tNO) + "</PersonalFlag>";
					}
				}
				else
				{
					text = text + "<PersonalFlag>" + Function.ToString(BoYesNoEnum.tYES) + "</PersonalFlag>";
				}
			}
			if (ColumnExist(drAction, "Tel") && !string.IsNullOrEmpty(Function.ToString(drAction["Tel"])))
			{
				text = text + "<Phone>" + Function.RawText(drAction["Tel"]) + "</Phone>";
			}
			if (ColumnExist(drAction, "prevActvty") && Function.ParseInt(drAction["prevActvty"]) > 0)
			{
				text = text + "<PreviousActivity>" + Function.ParseInt(drAction["prevActvty"]) + "</PreviousActivity>";
			}
			if (ColumnExist(drAction, "Priority") && !string.IsNullOrEmpty(Function.ToString(drAction["Priority"])))
			{
				switch (Function.ToString(drAction["Priority"]))
				{
				case "0":
					text = text + "<Priority>" + Function.ToString(BoMsgPriorities.pr_Low) + "</Priority>";
					break;
				case "1":
					text = text + "<Priority>" + Function.ToString(BoMsgPriorities.pr_Normal) + "</Priority>";
					break;
				case "2":
					text = text + "<Priority>" + Function.ToString(BoMsgPriorities.pr_High) + "</Priority>";
					break;
				}
			}
			if (ColumnExist(drAction, "DayInMonth") && Function.ParseInt(drAction["DayInMonth"]) != 0)
			{
				text = text + "<RecurrenceDayInMonth>" + Function.ParseInt(drAction["DayInMonth"]) + "</RecurrenceDayInMonth>";
			}
			if (ColumnExist(drAction, "DayOfWeek") && Function.ParseInt(drAction["DayOfWeek"]) != 0)
			{
				switch (Function.ToString(drAction["DayOfWeek"]))
				{
				case "0":
					text = text + "<RecurrenceDayOfWeek>" + Function.ToString(RecurrenceDayOfWeekEnum.rdowDay) + "</RecurrenceDayOfWeek>";
					break;
				case "1":
					text = text + "<RecurrenceDayOfWeek>" + Function.ToString(RecurrenceDayOfWeekEnum.rdowWeekDay) + "</RecurrenceDayOfWeek>";
					break;
				case "2":
					text = text + "<RecurrenceDayOfWeek>" + Function.ToString(RecurrenceDayOfWeekEnum.rdowWeekendDay) + "</RecurrenceDayOfWeek>";
					break;
				case "3":
					text = text + "<RecurrenceDayOfWeek>" + Function.ToString(RecurrenceDayOfWeekEnum.rdowSun) + "</RecurrenceDayOfWeek>";
					break;
				case "4":
					text = text + "<RecurrenceDayOfWeek>" + Function.ToString(RecurrenceDayOfWeekEnum.rdowMon) + "</RecurrenceDayOfWeek>";
					break;
				case "5":
					text = text + "<RecurrenceDayOfWeek>" + Function.ToString(RecurrenceDayOfWeekEnum.rdowTue) + "</RecurrenceDayOfWeek>";
					break;
				case "6":
					text = text + "<RecurrenceDayOfWeek>" + Function.ToString(RecurrenceDayOfWeekEnum.rdowWed) + "</RecurrenceDayOfWeek>";
					break;
				case "7":
					text = text + "<RecurrenceDayOfWeek>" + Function.ToString(RecurrenceDayOfWeekEnum.rdowThu) + "</RecurrenceDayOfWeek>";
					break;
				case "8":
					text = text + "<RecurrenceDayOfWeek>" + Function.ToString(RecurrenceDayOfWeekEnum.rdowFri) + "</RecurrenceDayOfWeek>";
					break;
				case "9":
					text = text + "<RecurrenceDayOfWeek>" + Function.ToString(RecurrenceDayOfWeekEnum.rdowSat) + "</RecurrenceDayOfWeek>";
					break;
				}
			}
			if (ColumnExist(drAction, "Month") && Function.ParseInt(drAction["Month"]) != 0)
			{
				text = text + "<RecurrenceMonth>" + Function.ParseInt(drAction["Month"]) + "</RecurrenceMonth>";
			}
			if (ColumnExist(drAction, "RecurPat") && !string.IsNullOrEmpty(Function.ToString(drAction["RecurPat"])))
			{
				switch (Function.ToString(drAction["RecurPat"]))
				{
				case "N":
					text = text + "<RecurrencePattern>" + Function.ToString(RecurrencePatternEnum.rpNone) + "</RecurrencePattern>";
					break;
				case "D":
					text = text + "<RecurrencePattern>" + Function.ToString(RecurrencePatternEnum.rpDaily) + "</RecurrencePattern>";
					break;
				case "W":
					text = text + "<RecurrencePattern>" + Function.ToString(RecurrencePatternEnum.rpWeekly) + "</RecurrencePattern>";
					break;
				case "M":
					text = text + "<RecurrencePattern>" + Function.ToString(RecurrencePatternEnum.rpMonthly) + "</RecurrencePattern>";
					break;
				case "A":
					text = text + "<RecurrencePattern>" + Function.ToString(RecurrencePatternEnum.rpAnnually) + "</RecurrencePattern>";
					break;
				}
			}
			if (ColumnExist(drAction, "Week") && Function.ParseInt(drAction["Week"]) != 0)
			{
				switch (Function.ToString(drAction["Week"]))
				{
				case "1":
					text += "<RecurrenceSequenceSpecifier>rsFirst</RecurrenceSequenceSpecifier>";
					break;
				case "2":
					text += "<RecurrenceSequenceSpecifier>rsSecond</RecurrenceSequenceSpecifier>";
					break;
				case "3":
					text += "<RecurrenceSequenceSpecifier>rsThird</RecurrenceSequenceSpecifier>";
					break;
				case "4":
					text += "<RecurrenceSequenceSpecifier>rsFourth</RecurrenceSequenceSpecifier>";
					break;
				case "5":
					text += "<RecurrenceSequenceSpecifier>rssLast</RecurrenceSequenceSpecifier>";
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
						text = text + "<Reminder>" + Function.ToString(BoYesNoEnum.tNO) + "</Reminder>";
					}
				}
				else
				{
					text = text + "<Reminder>" + Function.ToString(BoYesNoEnum.tYES) + "</Reminder>";
				}
			}
			if (ColumnExist(drAction, "RemTime") && Function.ParseInt(drAction["RemTime"]) > 0)
			{
				text = text + "<ReminderPeriod>" + ConvertToTime(Function.ToString(drAction["RemTime"])) + "</ReminderPeriod>";
			}
			if (ColumnExist(drAction, "RemType") && !string.IsNullOrEmpty(Function.ToString(drAction["RemType"])))
			{
				switch (Function.ToString(drAction["RemType"]))
				{
				case "M":
					text = text + "<ReminderType>" + Function.ToString(BoDurations.du_Minuts) + "</ReminderType>";
					break;
				case "H":
					text = text + "<ReminderType>" + Function.ToString(BoDurations.du_Hours) + "</ReminderType>";
					break;
				case "D":
					text = text + "<ReminderType>" + Function.ToString(BoDurations.du_Days) + "</ReminderType>";
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
						text = text + "<RepeatOption>" + Function.ToString(RepeatOptionEnum.roByWeekDay) + "</RepeatOption>";
					}
				}
				else
				{
					text = text + "<RepeatOption>" + Function.ToString(RepeatOptionEnum.roByDate) + "</RepeatOption>";
				}
			}
			if (ColumnExist(drAction, "room") && !string.IsNullOrEmpty(Function.ToString(drAction["room"])))
			{
				text = text + "<Room>" + Function.RawText(drAction["room"]) + "</Room>";
			}
			if (ColumnExist(drAction, "SlpCode") && Function.ParseInt(drAction["SlpCode"]) > 0)
			{
				text = text + "<SalesEmployee>" + Function.ParseInt(drAction["SlpCode"]) + "</SalesEmployee>";
			}
			if (ColumnExist(drAction, "AttendEmpl") && Function.ParseInt(drAction["AttendEmpl"]) > 0)
			{
				text = text + "<HandledByEmployee>" + Function.ParseInt(drAction["AttendEmpl"]) + "</HandledByEmployee>";
			}
			if (ColumnExist(drAction, "OprId") && Function.ParseInt(drAction["OprId"]) > 0)
			{
				text = text + "<SalesOpportunityId>" + Function.ParseInt(drAction["OprId"]) + "</SalesOpportunityId>";
			}
			if (ColumnExist(drAction, "OprLine") && Function.ParseInt(drAction["OprLine"]) > 0)
			{
				text = text + "<SalesOpportunityLine>" + Function.ParseInt(drAction["OprLine"]) + "</SalesOpportunityLine>";
			}
			if (ColumnExist(drAction, "Saturday") && !string.IsNullOrEmpty(Function.ToString(drAction["Saturday"])))
			{
				string text2 = Function.ToString(drAction["Saturday"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text = text + "<Saturday>" + Function.ToString(BoYesNoEnum.tNO) + "</Saturday>";
					}
				}
				else
				{
					text = text + "<Saturday>" + Function.ToString(BoYesNoEnum.tYES) + "</Saturday>";
				}
			}
			if (ColumnExist(drAction, "SeEndDate") && !string.IsNullOrEmpty(Function.ToString(drAction["SeEndDate"])))
			{
				text = text + "<SeriesEndDate>" + Convert.ToDateTime(drAction["SeEndDate"]).ToString("yyyyMMdd") + "</SeriesEndDate>";
			}
			if (ColumnExist(drAction, "SeStartDate") && !string.IsNullOrEmpty(Function.ToString(drAction["SeStartDate"])))
			{
				text = text + "<SeriesStartDate>" + Convert.ToDateTime(drAction["SeStartDate"]).ToString("yyyyMMdd") + "</SeriesStartDate>";
			}
			if (ColumnExist(drAction, "Recontact") && !string.IsNullOrEmpty(Function.ToString(drAction["Recontact"])))
			{
				text = text + "<StartDate>" + Convert.ToDateTime(drAction["Recontact"]).ToString("yyyy-MM-dd") + "</StartDate>";
			}
			if (ColumnExist(drAction, "BeginTime") && Function.ParseInt(drAction["BeginTime"]) > 0)
			{
				text = text + "<StartTime>" + ConvertToTime(Function.ToString(drAction["BeginTime"])) + "</StartTime>";
			}
			if (ColumnExist(drAction, "state") && !string.IsNullOrEmpty(Function.ToString(drAction["state"])))
			{
				text = text + "<State>" + Function.RawText(drAction["state"]) + "</State>";
			}
			if (ColumnExist(drAction, "status") && Function.ParseInt(drAction["status"]) != 0)
			{
				text = text + "<Status>" + Function.ParseInt(drAction["status"]) + "</Status>";
			}
			if (ColumnExist(drAction, "street") && !string.IsNullOrEmpty(Function.ToString(drAction["street"])))
			{
				text = text + "<Street>" + Function.RawText(drAction["street"]) + "</Street>";
			}
			if (ColumnExist(drAction, "CntctSbjct") && Function.ParseInt(drAction["CntctSbjct"]) != 0)
			{
				text = text + "<Subject>" + Function.ParseInt(drAction["CntctSbjct"]) + "</Subject>";
			}
			if (ColumnExist(drAction, "Sunday") && !string.IsNullOrEmpty(Function.ToString(drAction["Sunday"])))
			{
				string text2 = Function.ToString(drAction["Sunday"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text = text + "<Sunday>" + Function.ToString(BoYesNoEnum.tNO) + "</Sunday>";
					}
				}
				else
				{
					text = text + "<Sunday>" + Function.ToString(BoYesNoEnum.tYES) + "</Sunday>";
				}
			}
			if (ColumnExist(drAction, "tentative") && !string.IsNullOrEmpty(Function.ToString(drAction["tentative"])))
			{
				string text2 = Function.ToString(drAction["tentative"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text = text + "<TentativeFlag>" + Function.ToString(BoYesNoEnum.tNO) + "</TentativeFlag>";
					}
				}
				else
				{
					text = text + "<TentativeFlag>" + Function.ToString(BoYesNoEnum.tYES) + "</TentativeFlag>";
				}
			}
			if (ColumnExist(drAction, "Thursday") && !string.IsNullOrEmpty(Function.ToString(drAction["Thursday"])))
			{
				string text2 = Function.ToString(drAction["Thursday"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text = text + "<Thursday>" + Function.ToString(BoYesNoEnum.tNO) + "</Thursday>";
					}
				}
				else
				{
					text = text + "<Thursday>" + Function.ToString(BoYesNoEnum.tYES) + "</Thursday>";
				}
			}
			if (ColumnExist(drAction, "Tuesday") && !string.IsNullOrEmpty(Function.ToString(drAction["Tuesday"])))
			{
				string text2 = Function.ToString(drAction["Tuesday"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text = text + "<Tuesday>" + Function.ToString(BoYesNoEnum.tNO) + "</Tuesday>";
					}
				}
				else
				{
					text = text + "<Tuesday>" + Function.ToString(BoYesNoEnum.tYES) + "</Tuesday>";
				}
			}
			if (ColumnExist(drAction, "Wednesday") && !string.IsNullOrEmpty(Function.ToString(drAction["Wednesday"])))
			{
				string text2 = Function.ToString(drAction["Wednesday"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text = text + "<Wednesday>" + Function.ToString(BoYesNoEnum.tNO) + "</Wednesday>";
					}
				}
				else
				{
					text = text + "<Wednesday>" + Function.ToString(BoYesNoEnum.tYES) + "</Wednesday>";
				}
			}
			return text + "</Activity>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	protected bool ColumnExist(DataRow dr, string colName)
	{
		if (dr.Table.Columns.Contains(colName))
		{
			return true;
		}
		return false;
	}

	public void Dispose()
	{
		Dispose();
	}

	public string ConvertToTime(string value)
	{
		try
		{
			if (value.Length == 3)
			{
				string arg = value.Substring(1, 2);
				string arg2 = value.Substring(0, 1);
				return $"{arg2}:{arg}:00";
			}
			if (value.Length == 4)
			{
				string arg3 = value.Substring(2, 2);
				string arg4 = value.Substring(0, 2);
				return $"{arg4}:{arg3}:00";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public string SaveActivityInterval(string sessionId, DataTable dt)
	{
		try
		{
			if (dt.IsNotNull())
			{
				DateTime value = Function.ParseDateTimes(dt.Rows[0]["newStart"]);
				DateTime dateTime = Function.ParseDateTimes(dt.Rows[0]["newEnd"]);
				int num = Function.ParseInt(dt.Rows[0]["startTime"]);
				int num2 = Function.ParseInt(dt.Rows[0]["endTime"]);
				int num3 = Function.ParseInt(dt.Rows[0]["ClgCode"]);
				string text = Function.ToString(dt.Rows[0]["resourceId"]);
				string text2;
				if (dateTime.Subtract(value).TotalMinutes < 60.0)
				{
					text2 = Function.ToString(BoDurations.du_Minuts);
				}
				else
				{
					text2 = ((!(dateTime.Subtract(value).TotalMinutes >= 1440.0)) ? Function.ToString(BoDurations.du_Hours) : Function.ToString(BoDurations.du_Days));
				}
				decimal num4;
				if (text2 == Function.ToString(BoDurations.du_Minuts))
				{
					num4 = Function.ParseDecimal(dateTime.Subtract(value).TotalMinutes);
				}
				else
				{
					num4 = ((!(text2 == Function.ToString(BoDurations.du_Days))) ? Function.ParseDecimal(dateTime.Subtract(value).TotalHours) : Function.ParseDecimal(dateTime.Subtract(value).TotalDays));
				}
				int num5 = Function.ParseInt(text.Substring(1, text.Length));
				string text3 = Function.ToString(text.Substring(0, 1));
				string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                             <BOM>\r\n                            <BO>\r\n                             <AdmInfo>\r\n                               <Object>oContacts</Object>\r\n                             </AdmInfo>\r\n                             <QueryParams>\r\n                               <ContactCode>" + num3 + "</ContactCode>\r\n                             </QueryParams>\r\n                             <Contacts><row>\r\n                               <DurationType>" + text2 + "</DurationType>\r\n                               <Duration>" + num4 + "</Duration>\r\n                               <Recontact>" + value.Date.ToString("yyyyMMdd") + "</Recontact>\r\n                               <StartTime>" + Function.ToString(num) + "</StartTime>\r\n                               <EndDuedate>" + dateTime.Date.ToString("yyyyMMdd") + "</EndDuedate>\r\n                               <EndTime>" + Function.ToString(num2) + "</EndTime>" + ((Function.ParseInt(num5) == 0) ? "" : ((text3 == "U") ? ("<HandledBy>" + Function.ParseInt(num5) + "</HandledBy>") : ("<HandledByEmployee>" + Function.ParseInt(num5) + "</HandledByEmployee>"))) + "</row></Contacts>\r\n                            </BO>\r\n                           </BOM>\r\n                        </dis:UpdateObject></env:Body></env:Envelopes>";
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
						string arg = Function.ToString(xElement3.Value);
						Logging.Write("WATCH", $"Update Activity - {arg}");
						return string.Empty;
					}
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public string SaveActivityStatus(string sessionId, int clgCode, int status)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                             <BOM>\r\n                            <BO>\r\n                             <AdmInfo>\r\n                               <Object>oContacts</Object>\r\n                             </AdmInfo>\r\n                             <QueryParams>\r\n                               <ContactCode >" + clgCode + "</ContactCode>\r\n                             </QueryParams>\r\n                            <Contacts><row>\r\n                            <Status>" + status + "</Status>\r\n                            </row></Contacts>\r\n                            </BO>\r\n                           </BOM>\r\n                        </dis:UpdateObject></env:Body></env:Envelopes>";
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
					string arg = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Update Activity - {arg}");
					return string.Empty;
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public string SaveActivityAction(string sessionId, int clgCode, string action)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                             <BOM>\r\n                            <BO>\r\n                             <AdmInfo>\r\n                               <Object>oContacts</Object>\r\n                             </AdmInfo>\r\n                             <QueryParams>\r\n                               <ContactCode>" + clgCode + "</ContactCode>\r\n                             </QueryParams>\r\n                                <Contacts><row>\r\n                                    <Activity>" + action + "</Activity>\r\n                                </row></Contacts>\r\n                            </BO>\r\n                           </BOM>\r\n                        </dis:UpdateObject></env:Body></env:Envelopes>";
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
					string arg = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Update Activity - {arg}");
					return string.Empty;
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}
}
