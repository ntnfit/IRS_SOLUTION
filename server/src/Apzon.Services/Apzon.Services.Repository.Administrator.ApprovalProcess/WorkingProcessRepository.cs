using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.ApprovalProcess;

namespace Apzon.Services.Repository.Administrator.ApprovalProcess;

public class WorkingProcessRepository : BaseService<DataSet>, IWorkingProcess
{
	public WorkingProcessRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetWorkingProcessSettingData(int userId)
	{
		try
		{
			return ExecuteData("APZ_WorkingProcessSetting_GetData", CommandType.StoredProcedure, new SqlParameter("@userId", Function.ParseInt(userId)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetSetting(int userId)
	{
		try
		{
			return ExecuteData("APZ_WorkingProcess_GetSetting", CommandType.StoredProcedure, new SqlParameter("@userId", Function.ParseInt(userId)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public HttpResult SaveWorkingProcessSetting(Tuple<DataSet> tupData)
	{
		try
		{
			return Process(ApzObjectTypes.oWorkingProcessSetting, -1, Enums.TransactionType.U, tupData.Item1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		CreateTempTable(document.Tables[0], "#APZ_WPS1");
		BulkCopy(document.Tables[0], "#APZ_WPS1");
		CreateTempTable(document.Tables[1], "#APZ_WPS2");
		BulkCopy(document.Tables[1], "#APZ_WPS2");
		if (ExecuteDataTable("APZ_Process_WorkingProcessSetting", CommandType.StoredProcedure).Rows.Count <= 0)
		{
			return new HttpResult(Enums.MessageCode.Error, "Error");
		}
		return new HttpResult(Enums.MessageCode.Success);
	}

	public DataTable GetTemplateData(int userId, string status, string requestType)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@userId", userId));
			list.Add(new SqlParameter("@status", Function.ToString(status)));
			list.Add(new SqlParameter("@requestType", Function.ToString(requestType)));
			return ExecuteDataTable("APZ_WorkingProcess_GetTemplateData", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetTemplateDetails(int wddCode)
	{
		try
		{
			return ExecuteDataTable("APZ_WorkingProcess_GetTemplateDetails", CommandType.StoredProcedure, new SqlParameter("@wddCode", wddCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetTooltipData(int wddCode, int wtmCode, int docEntry, int userId)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("DispText", typeof(string));
			dataTable.Columns.Add("Value", typeof(string));
			foreach (DataRow row in ExecuteDataTable($"select \"ColId\", \"DispText\", \"TableName\", \"ExecQuery\", \"VisOrder\" from APZ_WPS2 where \"UserId\" = {userId} order by \"VisOrder\";", CommandType.Text).Rows)
			{
				DataTable dataTable2 = ExecuteDataTable(Function.ToString(row["ExecQuery"]), CommandType.Text, new SqlParameter("@wddCode", wddCode), new SqlParameter("@draftEntry", docEntry), new SqlParameter("@wtmCode", wtmCode));
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["DispText"] = row["DispText"];
				dataRow2["Value"] = Function.ToString(dataTable2.Rows[0][0]);
				dataTable.Rows.Add(dataRow2);
			}
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetTileViewSetting(int userId)
	{
		try
		{
			string query = "select * from APZ_WPS3 where \"UserId\" = @userId;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@userId", Function.ParseInt(userId)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public bool CheckDocumentCompleteApproval(int wddCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_WorkingProcess_CheckDocumentCompleteApproval", CommandType.StoredProcedure, new SqlParameter("@wddCode", wddCode));
			if (dataTable.Rows.Count > 0 && Function.ToString(dataTable.Rows[0][0]).Equals("Approved"))
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public DataTable FilterMenu(Tuple<DataSet> tupFilter)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				DataTable dt = tupFilter.Item1.Tables[0];
				DataTable dt2 = tupFilter.Item1.Tables[1];
				DataTable dt3 = tupFilter.Item1.Tables[2];
				CreateTempTable(dt, "#dtSource");
				BulkCopy(dt, "#dtSource");
				CreateTempTable(dt2, "#dtConds");
				BulkCopy(dt2, "#dtConds");
				CreateTempTable(dt3, "#dtList");
				BulkCopy(dt3, "#dtList");
				return ExecuteDataTable("APZ_WorkingProcess_FilterMenu", CommandType.StoredProcedure);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveMenu(Tuple<int, DataTable> tup)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				CreateTempTable(tup.Item2, "#dtMenu");
				BulkCopy(tup.Item2, "#dtMenu");
				ExecuteDataTable("APZ_WorkingProcess_SaveMenu", CommandType.StoredProcedure, new SqlParameter("@userId", tup.Item1));
				return new HttpResult();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public bool AllowLastGroupUserCreatedDoc(int wddCode, int userId)
	{
		try
		{
			string query = "APZ_WorkingProcess_CheckUserPermission";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@wddCode", wddCode), new SqlParameter("@userId", userId));
			return dataTable.IsNotNull() && Function.ToString(dataTable.Rows[0]["Status"]).Equals("Y");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public bool IsUserApproved(int wddCode, int userId)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select \"Status\" from WDD1 where \"WddCode\" = @wddCode and \"UserID\" = @userId;", CommandType.Text, new SqlParameter("@wddCode", wddCode), new SqlParameter("@userId", userId));
			return dataTable.IsNotNull() && Function.ToString(dataTable.Rows[0]["Status"]).Equals("Y");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}
}
