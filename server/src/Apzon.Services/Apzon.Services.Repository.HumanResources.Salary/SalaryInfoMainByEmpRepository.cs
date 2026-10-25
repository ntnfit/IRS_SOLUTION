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
using Apzon.Services.Interfaces.HumanResources.Salary;

namespace Apzon.Services.Repository.HumanResources.Salary;

public class SalaryInfoMainByEmpRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), ISalaryInfoMainByEmp
{
	private string Temp_SalInfo = "#APZ_SalInfoByEmp";

	public DataTable GetDetail(int empId)
	{
		try
		{
			string query = "APZ_SalInformation_GetDetail";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@empId", empId));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDetail  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string SaveDoc(DataTable dt)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			return Process(ApzObjectTypes.oSalaryInfo, -1, transtype, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDoc(int lineNum, int empId)
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
			return Process(ApzObjectTypes.oSalaryInfo, -1, Enums.TransactionType.D, dataTable).Message;
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
			_ = transtype;
			CreateTempTable(dtS, Temp_SalInfo);
			BulkCopy(dtS, Temp_SalInfo);
			DataTable dataTable = ExecuteDataTable("apz_process_SaveSalInfoByEmp", CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
				string text = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
				ret.Message = text + " - " + ret.Message;
				return;
			}
			ret.Message = "";
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
			int num = ExecuteNonQuery("apz_process_SalInfo_Remove", CommandType.StoredProcedure, list.ToArray());
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
