using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.PM;

public class ProjectManagerController : BaseApiController
{
	private Dictionary<int, string> dictionEmployee = new Dictionary<int, string>();

	private Dictionary<int, string> dictionUser = new Dictionary<int, string>();

	[HttpGet]
	public DataTable LoadData(int absEntry, string code, string status, string bpCode, string startFrom, string startTo, string userName, string filterString = "")
	{
		return UnitOfWork.ProjectManager.LoadData(absEntry, code, status, bpCode, startFrom, startTo, userName, filterString);
	}

	[HttpGet]
	public DataSet GetDataById(int Id)
	{
		return UnitOfWork.ProjectManager.GetDataById(Id);
	}

	[HttpGet]
	public DataTable GetListTask(int projectId)
	{
		try
		{
			DataTable listTask = UnitOfWork.ProjectManager.GetListTask(projectId);
			return DMSMapDataTaskProject(listTask);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetListTracking(int projectId)
	{
		return UnitOfWork.ProjectManager.GetListTracking(projectId);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		try
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new HttpResult(Enums.MessageCode.Error, "INVALID SESSION ID");
			}
			string userName = SessionUserInfo.UserName;
			if (string.IsNullOrEmpty(userName))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			if (ds.Tables[0] != null || ds.Tables.Count != 0)
			{
				DataTable dataTable = ds.Tables[0];
				if (dataTable.Rows.Count > 0)
				{
					dataTable.Rows[0]["UserCodeSign"] = userName;
					dataTable.Rows[0]["CreateDate"] = DateTime.Now;
					dataTable.Rows[0]["CreateTime"] = Function.ParseTimeSpanToInt(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
					dataTable.Rows[0]["UpdateDate"] = DateTime.Now;
					dataTable.Rows[0]["UpdateTime"] = Function.ParseTimeSpanToInt(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
					dataTable.AcceptChanges();
				}
				ds.AcceptChanges();
			}
			return UnitOfWork.ProjectManager.Save(ds, "A");
		}
		catch (Exception ex)
		{
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		try
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new HttpResult(Enums.MessageCode.Error, "INVALID SESSION ID");
			}
			string userName = SessionUserInfo.UserName;
			if (string.IsNullOrEmpty(userName))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			if (ds.Tables[0] != null || ds.Tables.Count != 0)
			{
				DataTable dataTable = ds.Tables[0];
				if (dataTable.Rows.Count > 0)
				{
					dataTable.Rows[0]["UserCodeSign2"] = userName;
					dataTable.Rows[0]["UpdateDate"] = DateTime.Now;
					dataTable.Rows[0]["UpdateTime"] = Function.ParseTimeSpanToInt(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
					dataTable.AcceptChanges();
				}
				ds.AcceptChanges();
			}
			return UnitOfWork.ProjectManager.Save(ds, "U");
		}
		catch (Exception ex)
		{
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string Id)
	{
		return UnitOfWork.ProjectManager.DeleteDocs(Id);
	}

	[HttpGet]
	public DataTable LoadSubProjectByMasterId(int projectId)
	{
		return UnitOfWork.ProjectManager.LoadSubProjectByMasterId(projectId);
	}

	[HttpGet]
	public DataTable GetDataBOMSaleOrder(string code)
	{
		return UnitOfWork.ProjectManager.GetDataBOMSaleOrder(code);
	}

	[HttpGet]
	public DataTable GetDataItemProduction(string code)
	{
		return UnitOfWork.ProjectManager.GetDataItemProduction(code);
	}

	[HttpGet]
	public DataTable GetDataItemInBom(string code)
	{
		return UnitOfWork.ProjectManager.GetDataItemInBom(code);
	}

	[HttpGet]
	public DataTable GetDataItemGroup()
	{
		return UnitOfWork.ProjectManager.GetDataItemGroup();
	}

	[HttpPost]
	public HttpResult UpdateStatus([FromBody] Tuple<int, string> model)
	{
		return UnitOfWork.ProjectManager.UpdateStatus(model.Item1, model.Item2);
	}

	[HttpGet]
	public DataTable FindNextLastFrist(int id, int type)
	{
		return UnitOfWork.ProjectManager.FindNextLastFrist(id, type);
	}

	[HttpGet]
	public DataSet GetDataLineByWbsTemplate(string code)
	{
		return UnitOfWork.ProjectManager.GetDataLineByWbsTemplate(code);
	}

	[HttpGet]
	public DataTable GetIdWorkScheduling(int year)
	{
		return UnitOfWork.ProjectManager.GetIdWorkScheduling(year);
	}

	[HttpGet]
	public DataTable GetDataShift(string code)
	{
		return UnitOfWork.ProjectManager.GetDataShift(code);
	}

	[HttpGet]
	public DataTable GetTaskTemplateByCode(string code)
	{
		return UnitOfWork.ProjectManager.GetTaskTemplateByCode(code);
	}

	[HttpGet]
	public DataTable GetTaskByWbsCode(string baseType, int baseEntry, int baseLine, string wbsCode)
	{
		return UnitOfWork.ProjectManager.GetTaskByWbsCode(baseType, baseEntry, baseLine, wbsCode);
	}

	[HttpGet]
	public DataTable GetStruct()
	{
		return UnitOfWork.Project.GetStruct();
	}

	[HttpGet]
	public string CheckItemInDocument(int Id, int lineNum = -1)
	{
		return UnitOfWork.ProjectManager.CheckItemInDocument(Id, lineNum);
	}

	[HttpGet]
	public DataTable GetShifOrderByTime(string code)
	{
		return UnitOfWork.ProjectManager.GetShifOrderByTime(code);
	}

	[HttpGet]
	public DataTable GetBrachByEmpID(int empId)
	{
		return UnitOfWork.ProjectManager.GetBrachByEmpID(empId);
	}

	[HttpGet]
	public string CheckMltpBrnchs()
	{
		return UnitOfWork.ProjectManager.CheckMltpBrnchs();
	}

	public DataTable DMSMapDataTaskProject(DataTable dt)
	{
		try
		{
			if (dt != null && dt.Rows.Count > 0)
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
						orderby Function.ParseInt(t["ClgCode"])
						select t).CopyToDataTable();
				}
			}
			return dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataSet GetProjectByAbsEntryDMS(int absEntry)
	{
		try
		{
			DataSet projectByAbsEntryDMS = UnitOfWork.ProjectManager.GetProjectByAbsEntryDMS(absEntry);
			if (projectByAbsEntryDMS != null && projectByAbsEntryDMS.Tables.Count > 0)
			{
				DataTable dt = projectByAbsEntryDMS.Tables[9];
				dt = DMSMapDataTaskProject(dt);
			}
			return projectByAbsEntryDMS;
		}
		catch (Exception)
		{
			return new DataSet();
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

	[HttpGet]
	public DataTable GetScheduleFromDate(string startDate)
	{
		try
		{
			return UnitOfWork.ProjectManager.GetScheduleFromDate(startDate);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetAllBaseLine(int absEntry)
	{
		try
		{
			return UnitOfWork.ProjectManager.GetAllBaseLine(absEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public HttpResult SetApproval([FromBody] NameID_Short model)
	{
		try
		{
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.ProjectManager.SetApproval(userName, model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.Error, ex.Message);
		}
	}

	[HttpGet]
	public DataTable GetTreeData(string itemCode)
	{
		return UnitOfWork.ProjectManager.GetTreeData(itemCode);
	}

	[HttpGet]
	public DataTable LoadDataDMS(int absEntry, string code, string status, string bpCode, string startFrom, string startTo, string userName, string filterString = "")
	{
		return UnitOfWork.ProjectManager.LoadDataDMS(absEntry, code, status, bpCode, startFrom, startTo, userName, filterString);
	}

	[HttpPost]
	public HttpResult UndoApproval([FromBody] int absEntry)
	{
		string userName = SessionUserInfo.UserName;
		return UnitOfWork.ProjectManager.UndoApproval(absEntry, userName);
	}
}
