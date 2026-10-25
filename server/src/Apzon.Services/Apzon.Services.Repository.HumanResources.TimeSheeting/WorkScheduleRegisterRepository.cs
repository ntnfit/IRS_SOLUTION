using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources.TimeSheeting;

namespace Apzon.Services.Repository.HumanResources.TimeSheeting;

public class WorkScheduleRegisterRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IWorkScheduleRegister
{
	private string Temp_Header = "#APZ_OWSR";

	public DataTable Search(DataTable dtDoc)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@branch", dtDoc.Rows[0]["branch"]),
				new SqlParameter("@dept", dtDoc.Rows[0]["dept"]),
				new SqlParameter("@empNo", dtDoc.Rows[0]["empId"]),
				new SqlParameter("@empName", dtDoc.Rows[0]["empName"]),
				new SqlParameter("@status", dtDoc.Rows[0]["status"]),
				new SqlParameter("@validFrom", dtDoc.Rows[0]["validFrom"]),
				new SqlParameter("@validTo", dtDoc.Rows[0]["validTo"])
			};
			return ExecuteDataTable("APZ_WorkScheduleRegister_Search", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDetail(int empId)
	{
		try
		{
			string query = "Select * from APZ_OWSR where \"empID\" = @empId;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@empId", empId));
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDetail  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string Save(DataTable dt)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.U;
			return Process(ApzObjectTypes.oWorkScheduleRegister, -1, transtype, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Remove(int lineNum, int empId)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("lineNum", typeof(int));
			dataTable.Columns.Add("empId", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["lineNum"] = lineNum;
			dataRow["empId"] = empId;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			return Process(ApzObjectTypes.oWorkScheduleRegister, -1, Enums.TransactionType.D, dataTable).Message;
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
				return;
			}
			CreateTempTable(dtS, Temp_Header);
			BulkCopy(dtS, Temp_Header);
			DataTable dataTable = ExecuteDataTable("apz_process_SaveWorkScheduleRegister", CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
				string text = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
				ret.Message = text + " - " + ret.Message;
				return;
			}
			List<string> list = new List<string>();
			foreach (int item in (from t in dtS.AsEnumerable()
				select t.Field<int>("EmpId")).Distinct().ToList())
			{
				list.Add(string.Join(",", item));
			}
			objId = string.Join(";", list.ToArray());
			objNum = objId;
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
			list.Add(new SqlParameter("@lineNum", Function.ToString(dt.Rows[0]["lineNum"])));
			list.Add(new SqlParameter("@empId", Function.ToString(dt.Rows[0]["empId"])));
			int num = ExecuteNonQuery("DELETE FROM APZ_OWSR WHERE \"empID\"= @empId AND \"LineNum\" = @lineNum;", CommandType.Text, list.ToArray());
			objId = Function.ToString(dt.Rows[0]["empId"]);
			objNum = objId;
			ret.Message = ((num > 0) ? "" : "Delete error! ");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}
}
