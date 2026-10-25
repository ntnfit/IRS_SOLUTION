using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Xml.Linq;
using Apzon.Api.Controllers.Sales;
using Apzon.Api.Hubs;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Service;

namespace Apzon.Api.Controllers.Service;

public class SubActivitiesController : BaseApiController
{
	private Dictionary<int, string> dictionEmployee = new Dictionary<int, string>();

	private Dictionary<int, string> dictionUser = new Dictionary<int, string>();

	[HttpGet]
	public DataTable GetAll(string userCode)
	{
		return UnitOfWork.SubActivities.GetAll(userCode);
	}

	[HttpGet]
	public DataTable GetByCode(int clgCode, string userCode)
	{
		return UnitOfWork.SubActivities.GetByCode(clgCode, userCode);
	}

	[HttpGet]
	public DataTable GetBySeriesNum(int seriesNum, string userCode)
	{
		return UnitOfWork.SubActivities.GetBySeriesNum(seriesNum, userCode);
	}

	[HttpGet]
	public DataTable GetDocByAction(string action, string userCode)
	{
		return UnitOfWork.SubActivities.GetDocByAction(action, userCode);
	}

	[HttpGet]
	public DataTable GetCalendarResources(string user)
	{
		return UnitOfWork.SubActivities.GetCalendarResources(user);
	}

	[HttpGet]
	public DataTable GetDocByResoucre(string resourceId, string userCode)
	{
		return UnitOfWork.SubActivities.GetDocByResource(resourceId, userCode);
	}

	[HttpPost]
	public DataTable GetDocByInterval(DataTable dtSearch)
	{
		return UnitOfWork.SubActivities.GetDocByInterval(dtSearch);
	}

	[HttpPost]
	public DataTable GetDocByIntervalMobile(DataTable _dtSearch)
	{
		try
		{
			Function.ParseDateTimes(_dtSearch.Rows[0]["startDate"]);
			DataTable dt = UnitOfWork.SubActivities.DMSGetDocByInterval(_dtSearch);
			return DataCalendar(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public DataTable SearchByIntervalMobile(DataTable _dtSearch)
	{
		try
		{
			DataTable dt = UnitOfWork.SubActivities.SearchByIntervalMobile(_dtSearch);
			return DataCalendar(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public HttpResult GetNotesByClgCode(string clgCode)
	{
		return UnitOfWork.SubActivities.GetNotesByClgCode(clgCode);
	}

	[HttpGet]
	public DataTable GetByCodeMobile(int clgCode, string userCode)
	{
		DataTable byCode = UnitOfWork.SubActivities.GetByCode(clgCode, userCode);
		return DataCalendar(byCode);
	}

	public DataTable DataCalendar(DataTable dt)
	{
		try
		{
			DataSet userList = UnitOfWork.CFL.GetUserList();
			DataTable dataTable = new DataTable();
			dataTable = ((!userList.IsHasStructure()) ? userList.Tables[0] : userList.Tables[1]);
			if (dataTable.IsNotNull() && dataTable.ExistsColumn("USERID") && dataTable.ExistsColumn("EmployeeName"))
			{
				dictionUser = dataTable.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["USERID"]), (DataRow s) => string.IsNullOrEmpty(Function.ToString(s["EmployeeName"])) ? Function.ToString(s["U_NAME"]) : (Function.ToString(s["EmployeeName"]) + "- " + Function.ToString(s["empID"])));
			}
			DataSet employeeListForSalesEmpSetup = UnitOfWork.CFL.GetEmployeeListForSalesEmpSetup();
			DataTable dataTable2 = new DataTable();
			dataTable2 = ((!employeeListForSalesEmpSetup.IsHasStructure()) ? employeeListForSalesEmpSetup.Tables[0] : employeeListForSalesEmpSetup.Tables[1]);
			if (dataTable2.IsNotNull() && dataTable2.ExistsColumn("empID") && dataTable2.ExistsColumn("lastName") && dataTable2.ExistsColumn("firstName"))
			{
				dictionEmployee = dataTable2.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["empID"]), (DataRow s) => Function.ToString(s["firstName"]) + " " + ((!string.IsNullOrEmpty(Function.ToString(s["middleName"]))) ? Function.ToString(s["middleName"]) : "") + " " + Function.ToString(s["lastName"]));
			}
			dt.Columns.Add("AssingTo", typeof(string));
			dt.Columns.Add("BeginTimeS", typeof(string));
			dt.Columns.Add("EndTimeS", typeof(string));
			dt.Columns.Add("BeginTimeSpan", typeof(string));
			dt.Columns.Add("EndTimeSpan", typeof(string));
			dt.Columns.Add("UserMobile", typeof(string));
			dt.Columns.Add("EmpMobile", typeof(string));
			dt.Columns.Add("EmpId", typeof(int));
			dt.Columns.Add("UserId", typeof(int));
			if (dt != null && dt.Rows.Count > 0)
			{
				for (int num = 0; num < dt.Rows.Count; num++)
				{
					Function.ParseDateTimes(dt.Rows[num]["Recontact"]);
					Function.ParseDateTimes(dt.Rows[num]["endDate"]);
					int num2 = Function.ParseInt(dt.Rows[num]["BeginTime"]);
					dt.Rows[num]["BeginTimeS"] = ((num2 > 0) ? Function.ParseIntToTimeSpan(num2).ToString("hh\\:mm") : "0");
					dt.Rows[num]["BeginTimeSpan"] = ((num2 > 0) ? Function.ParseIntToTimeSpan(num2).ToString("hh\\:mm") : "0");
					int num3 = Function.ParseInt(dt.Rows[num]["ENDTime"]);
					dt.Rows[num]["EndTimeS"] = ((num3 > 0) ? Function.ParseIntToTimeSpan(num3).ToString("hh\\:mm") : "0");
					dt.Rows[num]["EndTimeSpan"] = ((num3 > 0) ? Function.ParseIntToTimeSpan(num3).ToString("hh\\:mm") : "0");
					string text = Function.ToString(dt.Rows[num]["GroupUser"]);
					string text2 = Function.ToString(dt.Rows[num]["GroupEmployee"]);
					string text3 = "";
					string text4 = "";
					string text5 = "";
					if (!string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
					{
						List<string> list = text.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
						foreach (string item in list)
						{
							int key = Function.ParseInt(item);
							if (dictionUser.ContainsKey(key))
							{
								List<string> list2 = dictionUser[key].Split(new char[1] { '-' }, StringSplitOptions.RemoveEmptyEntries).ToList();
								text3 = text3 + list2[0] + "; ";
								text4 = text4 + key + "-" + dictionUser[key] + "; ";
							}
						}
						string value = ReplaceLastOccurrence(text4, ";", "");
						string value2 = ReplaceLastOccurrence(text3, ";", "");
						dt.Rows[num]["UserId"] = list[0];
						dt.Rows[num]["AssingTo"] = value2;
						dt.Rows[num]["UserMobile"] = value;
					}
					if (string.IsNullOrEmpty(text2))
					{
						continue;
					}
					List<string> list3 = text2.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
					foreach (string item2 in list3)
					{
						int key2 = Function.ParseInt(item2);
						if (dictionEmployee.ContainsKey(key2))
						{
							text5 = text5 + key2 + "-" + dictionEmployee[key2] + "; ";
							text3 = text3 + dictionEmployee[key2] + "; ";
						}
					}
					string value3 = ReplaceLastOccurrence(text5, ";", "");
					string value4 = ReplaceLastOccurrence(text3, ";", "");
					dt.Rows[num]["EmpId"] = list3[0];
					dt.Rows[num]["AssingTo"] = value4;
					dt.Rows[num]["EmpMobile"] = value3;
				}
			}
			if (dt.Rows.Count > 0)
			{
				dt = (from t in dt.AsEnumerable()
					orderby Function.ParseInt(t["BeginTime"])
					select t).CopyToDataTable();
			}
			return dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string ReplaceLastOccurrence(string Source, string Find, string Replace)
	{
		try
		{
			int num = Source.LastIndexOf(Find);
			if (num == -1)
			{
				return Source;
			}
			return Source.Remove(num, Find.Length).Insert(num, Replace);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	[HttpPost]
	public string SaveActivityInterval([FromBody] DataTable dt)
	{
		return UnitOfWork.SubActivities.SaveActivityInterval(dt);
	}

	[HttpGet]
	public string GetResourceIdsInfo(string type, string ids)
	{
		try
		{
			string text = "U";
			string result = "";
			if (string.IsNullOrEmpty(ids))
			{
				return result;
			}
			XElement xElement = new XElement("ResourceIds");
			string[] array = Function.ToString(ids).Split(';');
			foreach (string text2 in array)
			{
				if (!(type == "User"))
				{
					if (type == "Employee")
					{
						text = "E";
					}
				}
				else
				{
					text = "U";
				}
				XElement xElement2 = new XElement("ResourceId");
				xElement2.Add(new XAttribute("Type", "System.String"), new XAttribute("Value", text + Function.ToString(text2)));
				xElement.Add(xElement2);
			}
			return xElement.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	[HttpGet]
	public DataTable LoadTaskAndSubTask(string userName, ApzObjectTypes objType, string objId)
	{
		return UnitOfWork.SubActivities.LoadTaskAndSubTask(userName, objType, objId);
	}

	[HttpGet]
	public DataTable LoadTask(string userName, string taskStatus)
	{
		return UnitOfWork.SubActivities.LoadTask(userName, taskStatus);
	}

	[HttpGet]
	public DataTable LoadTaskByObjectType(string userName, ApzObjectTypes objType, string objId)
	{
		return UnitOfWork.SubActivities.LoadTaskByObjectType(userName, objType, objId);
	}

	[HttpGet]
	public DataTable LoadSingleTask(int clgCode, string userName)
	{
		return UnitOfWork.SubActivities.LoadSingleTask(clgCode, userName);
	}

	[HttpGet]
	public DataTable LoadSubTask(int clgCode, string userName)
	{
		return UnitOfWork.SubActivities.LoadSubTask(clgCode, userName);
	}

	[HttpGet]
	public DataTable LoadSubNode(int clgCode, string userName)
	{
		return UnitOfWork.SubActivities.LoadSubNode(clgCode, userName);
	}

	[HttpGet]
	public DataTable LoadParentTask(int parentTask)
	{
		return UnitOfWork.SubActivities.LoadParentTask(parentTask);
	}

	[HttpGet]
	public DataTable LoadTaskManagementSetting(string userSign)
	{
		return UnitOfWork.SubActivities.LoadTaskManagementSetting(userSign);
	}

	[HttpGet]
	public string GetCardTypeByCardCode(string cardCode)
	{
		return UnitOfWork.SubActivities.GetCardTypeByCardCode(cardCode);
	}

	private NotifiedObj GetNotifiedFromDataRow(DataRow dt, ApzObjectTypes objectTypes)
	{
		try
		{
			NotifiedObj notifiedObj = new NotifiedObj
			{
				CreateUserCode = UnitOfWork.Users.GetUserCodeById(Function.ParseInt(dt["AssignedBy"])),
				CreateUserName = UnitOfWork.Users.GetUserNameById(Function.ParseInt(dt["AssignedBy"])),
				AssignUser = ((!string.IsNullOrEmpty(Function.ToString(dt["GroupUser"]))) ? Function.ToString(dt["GroupUser"]) : UnitOfWork.EmpMasterData.GetListUserOfEmp(Function.ToString(dt["GroupEmployee"]))),
				DueDate = ((objectTypes == ApzObjectTypes.oTaskManagement) ? Function.ParseDateTimeMinToMaxValue(dt["endDate"], Function.ParseInt(dt["endTime"])) : Function.ParseDateTimeMinToMaxValue(dt["Recontact"], Function.ParseInt(dt["BeginTime"]))),
				NotifiedId = Function.ToString(dt["ClgCode"]),
				Location = UnitOfWork.Location.GetLocationNameByCode(Function.ParseInt(dt["Location"])),
				Detail = Function.ToString(dt["Details"]),
				ObjectTypes = objectTypes
			};
			notifiedObj.RecurrenceInfo = Function.ToString(dt["RecurrenceInfo"]);
			notifiedObj.ApptType = Function.ParseInt(dt["CntctType"]);
			switch (Function.ToString(dt["RemType"]))
			{
			case "M":
				notifiedObj.TimeBeforeStart = TimeSpan.FromMinutes(Function.ParseInt(dt["RemQty"]));
				break;
			case "D":
				notifiedObj.TimeBeforeStart = TimeSpan.FromDays(Function.ParseInt(dt["RemQty"]));
				break;
			case "H":
				notifiedObj.TimeBeforeStart = TimeSpan.FromHours(Function.ParseInt(dt["RemQty"]));
				break;
			}
			notifiedObj.NotifiedUser = UnitOfWork.Users.GetLstUserNameByCode(notifiedObj.AssignUser);
			if (Function.ToString(dt["AllUserInvite"]) == "Y" || Function.ToString(dt["AllEmplInvite"]) == "Y")
			{
				notifiedObj.AssignAll = true;
			}
			if (objectTypes == ApzObjectTypes.oNone)
			{
				NotifiedObj notifiedObj2 = notifiedObj;
				ApzObjectTypes objectTypes2;
				if (Function.ToString(dt["Action"]).Equals("T"))
				{
					objectTypes2 = ApzObjectTypes.oTaskManagement;
				}
				else
				{
					objectTypes2 = (Function.ToString(dt["Action"]).Equals("V") ? ApzObjectTypes.oEventManagement : ApzObjectTypes.oNoteManagement);
				}
				notifiedObj2.ObjectTypes = objectTypes2;
				notifiedObj.NotifiedUser += string.Format("{0};{1};", notifiedObj.CreateUserCode, UnitOfWork.SubActivities.GetUserComment(Function.ToString((int)notifiedObj.ObjectTypes), Function.ParseInt(dt["ClgCode"])));
			}
			return notifiedObj;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new NotifiedObj();
	}

	[HttpPost]
	public HttpResult AddTask(DataSet dt)
	{
		try
		{
			HttpResult httpResult = UnitOfWork.SubActivities.AddTask(dt);
			if (string.IsNullOrEmpty(httpResult.Message) && Function.ParseInt(dt.Tables[0].Rows[0]["Status"]) != 2)
			{
				NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(dt.Tables[0].Rows[0], ApzObjectTypes.oTaskManagement);
				notifiedFromDataRow.NotifiedId = Function.ToString(httpResult.Content);
				notifiedFromDataRow.TranType = Enums.TransactionType.A;
				new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public HttpResult SaveTask(DataSet dt)
	{
		try
		{
			HttpResult httpResult = UnitOfWork.SubActivities.SaveTask(dt);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(dt.Tables[0].Rows[0], ApzObjectTypes.oTaskManagement);
				notifiedFromDataRow.TranType = ((Function.ParseInt(dt.Tables[0].Rows[0]["Status"]) != 1) ? Enums.TransactionType.C : Enums.TransactionType.A);
				new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public HttpResult ChangeTaskStatus(DataSet dt)
	{
		try
		{
			HttpResult httpResult = UnitOfWork.SubActivities.ChangeTaskStatus(dt);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(dt.Tables[0].Rows[0], ApzObjectTypes.oTaskManagement);
				notifiedFromDataRow.TranType = ((Function.ParseInt(dt.Tables[0].Rows[0]["Status"]) != 1) ? Enums.TransactionType.C : Enums.TransactionType.A);
				new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public string RemoveTask(DataTable dt)
	{
		try
		{
			string text = UnitOfWork.SubActivities.RemoveTask(dt);
			if (string.IsNullOrEmpty(text))
			{
				NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(dt.Rows[0], ApzObjectTypes.oTaskManagement);
				notifiedFromDataRow.TranType = Enums.TransactionType.D;
				new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	[HttpPost]
	public HttpResult SaveSubTaskDetails(DataSet dt)
	{
		return UnitOfWork.SubActivities.SaveSubTaskDetails(dt);
	}

	[HttpPost]
	public HttpResult SaveSubTaskAssigner(DataSet dt)
	{
		return UnitOfWork.SubActivities.SaveSubTaskAssigner(dt);
	}

	[HttpPost]
	public HttpResult SaveTaskPosition(DataSet dt)
	{
		return UnitOfWork.SubActivities.SaveTaskPosition(dt);
	}

	[HttpPost]
	public HttpResult SaveTaskManagementSetting(DataSet dt)
	{
		return UnitOfWork.SubActivities.SaveTaskManagementSetting(dt);
	}

	[HttpGet]
	public DataTable LoadNote(string userCode, int projectCode)
	{
		return UnitOfWork.SubActivities.LoadNote(userCode, projectCode);
	}

	[HttpGet]
	public DataTable LoadNoteByObjectType(string userName, ApzObjectTypes objTypeNote, ApzObjectTypes objType, string objId)
	{
		return UnitOfWork.SubActivities.LoadNoteByObjectType(userName, objTypeNote, objType, objId);
	}

	[HttpPost]
	public HttpResult SaveNote(DataSet dt)
	{
		return UnitOfWork.SubActivities.SaveNote(dt);
	}

	[HttpPost]
	public string DeleteNote(DataTable dt)
	{
		return UnitOfWork.SubActivities.DeleteNote(dt);
	}

	[HttpGet]
	public DataTable LoadNoteByCode(int clgCode)
	{
		return UnitOfWork.SubActivities.LoadNoteByCode(clgCode);
	}

	[HttpPost]
	public DataTable LoadNoteByInterval(DataTable dtSearch)
	{
		return UnitOfWork.SubActivities.LoadNoteByInterval(dtSearch);
	}

	[HttpGet]
	public string PushAppNotification(string noteId)
	{
		return UnitOfWork.SubActivities.PushAppNotification(noteId);
	}

	[HttpGet]
	public DataTable LoadEvent(string userCode, int projectCode)
	{
		return UnitOfWork.SubActivities.LoadEvent(userCode, projectCode);
	}

	[HttpGet]
	public DataTable LoadEventByObjectType(string userName, ApzObjectTypes objTypeEvent, ApzObjectTypes objType, string objId)
	{
		return UnitOfWork.SubActivities.LoadEventByObjectType(userName, objTypeEvent, objType, objId);
	}

	[HttpGet]
	public DataTable LoadEventByCode(int clgCode, string userCode)
	{
		return UnitOfWork.SubActivities.LoadEventByCode(clgCode, userCode);
	}

	[HttpPost]
	public DataTable LoadEventByInterval(DataTable dtSearch)
	{
		return UnitOfWork.SubActivities.LoadEventByInterval(dtSearch);
	}

	[HttpGet]
	public DataTable LoadEventByOpportunity(ApzObjectTypes objType, int oprId, string bpCode)
	{
		return UnitOfWork.SubActivities.LoadEventByOpportunity(objType, oprId, bpCode);
	}

	[HttpPost]
	public HttpResult AddEvent([FromBody] DataSet ds)
	{
		try
		{
			HttpResult httpResult = UnitOfWork.SubActivities.AddEvent(ds);
			if (string.IsNullOrEmpty(httpResult.Message) && Function.ToString(ds.Tables[0].Rows[0]["Reminder"]) == "Y")
			{
				NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(ds.Tables[0].Rows[0], ApzObjectTypes.oEventManagement);
				notifiedFromDataRow.NotifiedId = Function.ToString(httpResult.Content).Split(',')[0];
				notifiedFromDataRow.TranType = Enums.TransactionType.A;
				new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public HttpResult SaveAllEvent([FromBody] DataSet ds)
	{
		try
		{
			HttpResult httpResult = UnitOfWork.SubActivities.SaveEvent(ds, 1);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(ds.Tables[0].Rows[0], ApzObjectTypes.oEventManagement);
				if (Function.ToString(ds.Tables[0].Rows[0]["Reminder"]) == "Y")
				{
					notifiedFromDataRow.TranType = Enums.TransactionType.U;
				}
				else
				{
					notifiedFromDataRow.TranType = Enums.TransactionType.C;
				}
				new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public HttpResult SaveThisEvent([FromBody] DataSet ds)
	{
		try
		{
			HttpResult httpResult = UnitOfWork.SubActivities.SaveEvent(ds, 2);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(ds.Tables[0].Rows[0], ApzObjectTypes.oEventManagement);
				if (Function.ToString(ds.Tables[0].Rows[0]["Reminder"]) == "Y")
				{
					notifiedFromDataRow.TranType = Enums.TransactionType.U;
				}
				else
				{
					notifiedFromDataRow.TranType = Enums.TransactionType.C;
				}
				new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public string RemoveAllEvent([FromBody] DataTable dt)
	{
		try
		{
			string text = UnitOfWork.SubActivities.RemoveEvent(dt, 1);
			if (string.IsNullOrEmpty(text))
			{
				NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(dt.Rows[0], ApzObjectTypes.oEventManagement);
				notifiedFromDataRow.TranType = Enums.TransactionType.D;
				new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	[HttpPost]
	public string RemoveUnchangedEvent([FromBody] DataTable dt)
	{
		try
		{
			string text = UnitOfWork.SubActivities.RemoveEvent(dt, 2);
			if (string.IsNullOrEmpty(text))
			{
				NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(dt.Rows[0], ApzObjectTypes.oEventManagement);
				notifiedFromDataRow.TranType = Enums.TransactionType.D;
				new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	[HttpGet]
	public DataSet LoadComment(int Id, string type, int userSign)
	{
		return UnitOfWork.SubActivities.LoadComment(Id, type, userSign);
	}

	[HttpPost]
	public HttpResult SaveComment(Tuple<DataTable, List<int>> tuple)
	{
		try
		{
			HttpResult httpResult = UnitOfWork.SubActivities.SaveComment(tuple);
			DataTable item = tuple.Item1;
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				if (Function.ToString(item.Rows[0]["ObjType"]).Equals("1470000130"))
				{
					NotifiedObj notifiedObj = new NotifiedObj
					{
						ObjectTypes = ApzObjectTypes.oTrackingIssue,
						DueDate = Function.ParseDateTimes(item.Rows[0]["CreatedTime"]),
						Detail = Function.ToString(item.Rows[0]["Content"]),
						CreateUserCode = UnitOfWork.Users.GetUserCodeById(Function.ParseInt(item.Rows[0]["UserSign"])),
						CreateUserName = UnitOfWork.Users.GetUserNameById(Function.ParseInt(item.Rows[0]["UserSign"])),
						NotifiedId = Function.ToString(item.Rows[0]["ID"]),
						NotifiedUser = string.Format("{0};{1};{2};", UnitOfWork.TrackingIssue.GetUseHandleById(Function.ParseInt(item.Rows[0]["ID"])), UnitOfWork.TrackingIssue.GetUseReportById(Function.ParseInt(item.Rows[0]["ID"])), UnitOfWork.SubActivities.GetUserComment(Function.ToString(item.Rows[0]["ObjType"]), Function.ParseInt(item.Rows[0]["ID"]))),
						TranType = Enums.TransactionType.M,
						CmtCode = Function.ParseInt(Function.ToString(httpResult.Content))
					};
					notifiedObj.NotifiedUser = notifiedObj.NotifiedUser.Replace(notifiedObj.CreateUserCode + ";", "");
					new NotificationClient().SendNotifiedToClient(notifiedObj);
				}
				else
				{
					DataTable dataById = UnitOfWork.SubActivities.GetDataById(Function.ParseInt(item.Rows[0]["ID"]));
					if (dataById.IsNotNull())
					{
						NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(dataById.Rows[0], ApzObjectTypes.oNone);
						notifiedFromDataRow.DueDate = Function.ParseDateTimes(item.Rows[0]["CreatedTime"]);
						notifiedFromDataRow.Detail = Function.ToString(item.Rows[0]["Content"]);
						notifiedFromDataRow.CreateUserCode = UnitOfWork.Users.GetUserCodeById(Function.ParseInt(item.Rows[0]["UserSign"]));
						notifiedFromDataRow.CreateUserName = UnitOfWork.Users.GetUserNameById(Function.ParseInt(item.Rows[0]["UserSign"]));
						notifiedFromDataRow.NotifiedUser = notifiedFromDataRow.NotifiedUser.Replace(notifiedFromDataRow.CreateUserCode + ";", "");
						notifiedFromDataRow.CmtCode = Function.ParseInt(httpResult.Content);
						notifiedFromDataRow.TranType = Enums.TransactionType.M;
						new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
					}
				}
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public HttpResult SaveCommentMobile(CommentObj tuple)
	{
		Tuple<DataTable, List<int>> tuple2 = new Tuple<DataTable, List<int>>(tuple.CommentInfo, tuple.AdditionNotifyUser);
		return SaveComment(tuple2);
	}

	[HttpPost]
	public string ClearUnreadComment(DataTable dt)
	{
		return UnitOfWork.SubActivities.ClearUnreadComment(dt);
	}

	[HttpPost]
	public string DeleteComment([FromBody] int id)
	{
		return UnitOfWork.SubActivities.DeleteComment(id);
	}

	[HttpPost]
	public string RemoveCommentById([FromBody] Tuple<int, string> id)
	{
		return UnitOfWork.SubActivities.RemoveCommentById(id.Item1, id.Item2);
	}

	[HttpPost]
	public HttpResult MarkAsRead([FromBody] int comment)
	{
		return UnitOfWork.SubActivities.MarkAsRead(comment);
	}

	[HttpGet]
	public DataTable GetUnlockedUsers()
	{
		return UnitOfWork.SubActivities.GetUnlockedUsers();
	}

	[HttpGet]
	public DataTable GetActiveEmployees()
	{
		return UnitOfWork.SubActivities.GetActiveEmployees();
	}

	[HttpGet]
	public DataTable GetEmployeeByUserId(int userId)
	{
		return UnitOfWork.SubActivities.GetEmployeeByUserId(userId);
	}

	[HttpGet]
	public DataTable GetAllNotification(int userId)
	{
		return UnitOfWork.SubActivities.GetAllNotification(userId);
	}

	public DataTable GetStruct()
	{
		return UnitOfWork.SubActivities.GetStruct();
	}

	[HttpPost]
	public HttpResult AddMuntiTask(DataTable dt)
	{
		return UnitOfWork.SubActivities.AddMuntiTask(dt);
	}

	[HttpGet]
	public string CheckExists(int oclg_code)
	{
		return UnitOfWork.SubActivities.CheckExists(oclg_code);
	}

	[HttpGet]
	public DataTable GetStructTask()
	{
		try
		{
			DataTable dataTable = UnitOfWork.SubActivities.GetStruct();
			DataRow dataRow = dataTable.NewRow();
			dataRow["ClgCode"] = 0;
			dataRow["Closed"] = "N";
			dataRow["status"] = 1;
			dataRow["Action"] = "T";
			dataRow["CntctDate"] = DateTime.Today;
			dataRow["Priority"] = "1";
			dataRow["DocType"] = -1;
			dataRow["UpdateDate"] = DateTime.Today;
			dataRow["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			dataTable.Rows.Add(dataRow);
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public HttpResult AddTaskDMS(DataSet dt)
	{
		try
		{
			SetValueTaskDMS(dt);
			HttpResult httpResult = UnitOfWork.SubActivities.AddTask(dt);
			if (string.IsNullOrEmpty(httpResult.Message) && Function.ParseInt(dt.Tables[0].Rows[0]["Status"]) != 2)
			{
				NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(dt.Tables[0].Rows[0], ApzObjectTypes.oTaskManagement);
				notifiedFromDataRow.NotifiedId = Function.ToString(httpResult.Content);
				notifiedFromDataRow.TranType = Enums.TransactionType.A;
				new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public HttpResult SaveTaskDMS(DataSet dt)
	{
		try
		{
			SetValueTaskDMS(dt);
			HttpResult httpResult = UnitOfWork.SubActivities.SaveTask(dt);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(dt.Tables[0].Rows[0], ApzObjectTypes.oTaskManagement);
				notifiedFromDataRow.TranType = ((Function.ParseInt(dt.Tables[0].Rows[0]["Status"]) != 1) ? Enums.TransactionType.C : Enums.TransactionType.A);
				new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataSet SetValueTaskDMS(DataSet dt)
	{
		try
		{
			if (dt != null && dt.Tables.Count > 0)
			{
				for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
				{
					string text = Function.ToString(dt.Tables[0].Rows[i]["BeginTime"]);
					dt.Tables[0].Rows[i]["BeginTime"] = Function.ParseInt(text.Replace(":", ""));
					string text2 = Function.ToString(dt.Tables[0].Rows[i]["ENDTime"]);
					dt.Tables[0].Rows[i]["ENDTime"] = Function.ParseInt(text2.Replace(":", ""));
				}
			}
			return dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}
}
