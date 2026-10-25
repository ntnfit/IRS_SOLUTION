using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources.TimeSheeting;

namespace Apzon.Services.Repository.HumanResources.TimeSheeting;

public class LeaveInformationRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), ILeaveInformation
{
	private string processType = "";

	private string TempTableTab = "#dtAPZ_TSL2";

	public DataTable GetWorkPeriodsByBranch(int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Branch", branch));
			return ExecuteDataTable("APZ_EmployeeLeave_GetWorkPeriodsByBranch", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListWorkPeriod  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable SearchLeaveInfo(DataTable dtSearch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@branch", dtSearch.Rows[0]["branch"]),
				new SqlParameter("@dept", dtSearch.Rows[0]["dept"]),
				new SqlParameter("@empNo", dtSearch.Rows[0]["empId"]),
				new SqlParameter("@empName", dtSearch.Rows[0]["empName"]),
				new SqlParameter("@status", dtSearch.Rows[0]["status"]),
				new SqlParameter("@period", dtSearch.Rows[0]["period"])
			};
			return ExecuteDataTable("APZ_EmployeeLeave_SearchLeaveInformation", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveLeaveInformation(DataTable dt)
	{
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		processType = "SaveLeaveInformation";
		return Process(ApzObjectTypes.oLeaveInformation, -1, Enums.TransactionType.A, dataSet);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		string text = "";
		try
		{
			if (document.Tables.Count > 0)
			{
				if (processType == "SaveLeaveInformation")
				{
					CreateTempTable(document.Tables[0], TempTableTab);
					BulkCopy(document.Tables[0], TempTableTab);
					string text2 = Function.ToString(document.Tables[0].Rows[0]["Period"]);
					text = "APZ_TimeSheeting_SaveLeaveInformation";
					ExecuteScalar(text, CommandType.StoredProcedure);
					List<string> list = new List<string>();
					foreach (int item in (from t in document.Tables[0].AsEnumerable()
						select t.Field<int>("EmpId")).Distinct().ToList())
					{
						list.Add(string.Join(",", text2, item));
					}
					objId = string.Join(";", list.ToArray());
					objNum = text2;
				}
				return new HttpResult();
			}
			return new HttpResult(Enums.MessageCode.Error, "No data to save.");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
