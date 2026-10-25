using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

internal class JobsManagementRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IJobsManagement
{
	private string Temp_Job = "#APZ_OJOB";

	private string Temp_Recur = "#APZ_OREC";

	public DataTable GetAllJobs()
	{
		try
		{
			return ExecuteDataTable("Select * from APZ_OJOB where \"IsRemove\" = 'N';", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Jobs  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetRecurInfo(int jobId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@JobId", jobId));
			return ExecuteDataTable("Select * from APZ_OREC where \"JobId\" = @JobId;", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Recurring Info  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public string SaveData(DataSet ds)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			return Process(ApzObjectTypes.oJobsManagement, -1, transtype, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
			ProcessSave(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.U:
			ProcessSave(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		}
		return ret;
	}

	private void ProcessSave(ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (!ds.IsNotNull())
			{
				ret.Message = "No data to save";
			}
			else
			{
				_ = transtype;
				CreateTempTable(ds.Tables[0], Temp_Job);
				BulkCopy(ds.Tables[0], Temp_Job);
				CreateTempTable(ds.Tables[1], Temp_Recur);
				BulkCopy(ds.Tables[1], Temp_Recur);
				DataTable dataTable = ExecuteDataTable("APZ_HangfireJobs_Save", CommandType.StoredProcedure);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
					{
						ret.Message = "";
					}
					else
					{
						ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
						string text = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
						ret.Message = text + " - " + ret.Message;
					}
				}
				else
				{
					ret.Message = "Save Error!";
				}
			}
			if (!string.IsNullOrEmpty(ret.Message))
			{
				objId = "";
				objNum = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	public string UpdateStatus(int jobId, string status)
	{
		string result = "";
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@JobId", jobId));
			list.Add(new SqlParameter("@status", status));
			if (ExecuteNonQuery("Update APZ_OJOB set Status = @status where JobId = @JobId;", CommandType.Text, list.ToArray()) <= 0)
			{
				return "Update Job Status Failed!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Update Job Status  FAILED ", ex.Message);
		}
		return result;
	}
}
