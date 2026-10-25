using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources.Salary;

namespace Apzon.Services.Repository.HumanResources.Salary;

public class LeaveParameterRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), ILeaveParameter
{
	private string Temp_LP = "#APZ_OSLP";

	public DataTable GetDoc()
	{
		try
		{
			string query = "Select * from APZ_OSLP;";
			new List<SqlParameter>();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string SaveDoc(DataTable dt)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			return Process(ApzObjectTypes.oLeaveParameter, -1, transtype, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDoc(int code)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = code;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			return Process(ApzObjectTypes.oLeaveParameter, -1, Enums.TransactionType.D, dataTable).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
			ProcessSave(ref transtype, dt, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.U:
			ProcessSave(ref transtype, dt, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.D:
			ProcessDelete(dt, ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessSave(ref Enums.TransactionType transtype, DataTable dtS, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (!dtS.IsNotNull())
			{
				ret.Message = "No data to save";
			}
			else
			{
				_ = transtype;
				CreateTempTable(dtS, Temp_LP);
				BulkCopy(dtS, Temp_LP);
				DataTable dataTable = ExecuteDataTable("apz_process_SaveLeaveParameter", CommandType.StoredProcedure, (SqlParameter[])null);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
					string text = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
					ret.Message = text + " - " + ret.Message;
				}
				else
				{
					ret.Message = "";
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

	private void ProcessDelete(DataTable dt, ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@code", Function.ToString(dt.Rows[0]["Code"])));
			int num = ExecuteNonQuery("delete from  APZ_OSLP where \"Code\" = @code;", CommandType.Text, list.ToArray());
			ret.Message = ((num > 0) ? "" : "Delete error! ");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}
}
