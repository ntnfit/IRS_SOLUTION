using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Administrator;
using SAPbobsCOM;

namespace Apzon.Services.Repository.Administrator;

public class ActivityCalendarRepository : BaseService<DataTable>, IActivityCalendar
{
	public ActivityCalendarRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDoc()
	{
		try
		{
			return ExecuteDataTable("APZ_ActivityCalendar_GetAll", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetUsers()
	{
		try
		{
			return ExecuteDataTable("APZ_ActivityCalendar_GetDistinctUsers", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetUsers  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDocByUser(int attendUser)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@AttendUser", Function.ToString(attendUser)));
			return ExecuteDataTable("APZ_ActivityCalendar_GetByCode", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDocByEmpl(int attendEmpl)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@AttendEmpl", Function.ToString(attendEmpl)));
			return ExecuteDataTable("APZ_ActivityCalendar_GetByCode", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetUnlockedUsers()
	{
		try
		{
			return ExecuteDataTable("select isnull(a.USERID,0) as USERID,isnull( a.USER_CODE,0) as Requester ,isnull( a.U_NAME,'') as ReqName ,a.Department, isnull(b.Name,'') as DepartmentName,a.Branch,isnull(c.Name ,'') as BranchName, ISNULL(a.UserSign,0) as UserSign\r\n                            from OUSR a left join OUDP b ON a.Department = b.Code \r\n                            left join OUBR c ON c.Code = a.Branch where a.Locked = 'N'", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRequesterByUser", ex.Message.ToString());
			return new DataTable();
		}
	}

	public DataTable GetActiveEmployees()
	{
		try
		{
			return ExecuteDataTable("select isnull( lastName,'') as LastName ,isnull ( firstName,'') as FirstName,  lastName + ',' + firstName as ReqName ,isnull( empID,0) as EmpID , isnull (UserSign,0) as UserSign FROM OHEM where Active = 'Y' ", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRequesterByEmployee", ex.Message.ToString());
			return new DataTable();
		}
	}

	public string SaveActivityInterval(DataTable dt)
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
				using (DatabaseService.OpenConnection())
				{
					using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
					List<SqlParameter> list = new List<SqlParameter>();
					list.Add(new SqlParameter("@clgCode", num3));
					list.Add(new SqlParameter("@durType", text2));
					list.Add(new SqlParameter("@duration", num4));
					list.Add(new SqlParameter("@recontact", value.Date.ToString("yyyyMMdd")));
					list.Add(new SqlParameter("@startTime", Function.ToString(num)));
					list.Add(new SqlParameter("@endDate", dateTime.Date.ToString("yyyyMMdd")));
					list.Add(new SqlParameter("@endTime", Function.ToString(num2)));
					list.Add(new SqlParameter("@resourceId", Function.ToString(text)));
					if (ExecuteNonQuery("APZ_ActivityCalendar_SaveInterval", CommandType.StoredProcedure, list.ToArray()) > 0)
					{
						dbTransaction.Commit();
						return "";
					}
					dbTransaction.Rollback();
					return "This activity is closed or removed!";
				}
			}
			return "This activity is closed or removed!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveInterval  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveActivityStatus(int clgCode, int status)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@clgCode", clgCode));
				list.Add(new SqlParameter("@status", status));
				if (ExecuteNonQuery("APZ_ActivityCalendar_SaveStatus", CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "Save error!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveStatus  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveActivityAction(int clgCode, string action)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@clgCode", clgCode));
				list.Add(new SqlParameter("@action", action));
				if (ExecuteNonQuery("APZ_ActivityCalendar_SaveAction", CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "Save error!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveAction  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetActivityByCode(int clgCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ClgCode", Function.ToString(clgCode)));
			return ExecuteDataTable("APZ_ActivityCalendar_GetByCode", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
