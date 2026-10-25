using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources.TimeSheeting;

namespace Apzon.Services.Repository.HumanResources.TimeSheeting;

public class TimeSheetingHourRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), ITimeSheetingHour
{
	private string TempTableTab = "#dtAPZ_OTSH";

	public DataTable LoadTimeSheetingHour(int branch, string dept, int empID, string empName, string status, string period, string workcode, int baseEntry)
	{
		try
		{
			string query = "APZ_TimeSheetingHour_Load";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Branch", branch));
			list.Add(new SqlParameter("@Dept", dept ?? ""));
			list.Add(new SqlParameter("@Emp", Function.ParseInt(empID)));
			list.Add(new SqlParameter("@EmpName", empName ?? ""));
			list.Add(new SqlParameter("@SStatus", status ?? ""));
			list.Add(new SqlParameter("@Period", period ?? ""));
			list.Add(new SqlParameter("@OvertimeType", workcode ?? ""));
			list.Add(new SqlParameter("@BaseEntry", Function.ParseInt(baseEntry)));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataSet LoadTimeSheetingHourCopyFrom(int baseEntry, string workcode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@BaseEntry", Function.ParseInt(baseEntry)));
			list.Add(new SqlParameter("@OvertimeType", Function.ToString(workcode)));
			DataSet dataSet = ExecuteData("APZ_TimeSheetingHour_LoadCopyFrom", CommandType.StoredProcedure, list.ToArray());
			if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
			{
				return dataSet;
			}
			return new DataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public DataTable LoadTimeSheetingHourSetting(string userSign)
	{
		try
		{
			string query = "SELECT * FROM APZ_TSHS WHERE \"UserSign\"=@UserSign;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserSign", userSign));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveTimeSheetingHour(DataTable dt)
	{
		return Process(ApzObjectTypes.oTimeSheetingHour, -1, Enums.TransactionType.A, dt);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTableTab);
			BulkCopy(document, TempTableTab);
			string text = Function.ToString(document.Rows[0]["Period"]);
			string text2 = Function.ToString(document.Rows[0]["OvertimeType"]);
			DataTable dataTable = ExecuteDataTable("APZ_TimeSheetingHour_Save", CommandType.StoredProcedure);
			if (dataTable.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
			{
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ToString(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			List<string> list = new List<string>();
			foreach (int item in (from t in document.AsEnumerable()
				select t.Field<int>("EmpId")).Distinct().ToList())
			{
				list.Add(string.Join(",", text, item, text2));
			}
			objId = string.Join(";", list.ToArray());
			objNum = text;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string SaveTimeSheetingHourSetting(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@Branch", Function.ToString(dt.Rows[0]["Branch"])));
				list.Add(new SqlParameter("@Department", Function.ToString(dt.Rows[0]["Department"])));
				list.Add(new SqlParameter("@Status", Function.ToString(dt.Rows[0]["Status"])));
				list.Add(new SqlParameter("@Period", Function.ToString(dt.Rows[0]["Period"])));
				list.Add(new SqlParameter("@OvertimeType", Function.ToString(dt.Rows[0]["OvertimeType"])));
				list.Add(new SqlParameter("@UserSign", Function.ToString(dt.Rows[0]["UserSign"])));
				if (ExecuteNonQuery(GetSaveTimeSheetingHourSettingQuery(), CommandType.Text, list.ToArray()) <= 0)
				{
					return "Error!";
				}
				dbTransaction.Commit();
				return string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	protected virtual string GetSaveTimeSheetingHourSettingQuery()
	{
		return "UPDATE APZ_TSHS SET Branch=@Branch, Department=@Department, Status=@Status, Period=@Period, OvertimeType=@OvertimeType, UserSign=@UserSign\r\n                            IF (@@ROWCOUNT = 0 ) \r\n                                BEGIN  \r\n                                    INSERT INTO APZ_TSHS (Branch, Department,Status, Period, OvertimeType, UserSign)  \r\n                                    VALUES (@Branch, @Department,@Status,@Period,@OvertimeType,@UserSign)  \r\n                                END  ";
	}
}
