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

public class SalaryInfoRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), ISalaryInfo
{
	private string Temp_SalInfo = "#APZ_SalInfo";

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
			DataTable dataTable = ExecuteDataTable("apz_process_SaveSalInfo", CommandType.StoredProcedure, (SqlParameter[])null);
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

	public DataSet Search(DataTable dtDoc)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@branch", dtDoc.Rows[0]["branch"]),
				new SqlParameter("@dept", dtDoc.Rows[0]["dept"]),
				new SqlParameter("@empNo", dtDoc.Rows[0]["empId"]),
				new SqlParameter("@empName", dtDoc.Rows[0]["empName"]),
				new SqlParameter("@status", dtDoc.Rows[0]["status"])
			};
			return ExecuteData("APZ_SalaryInformation_Search", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public DataTable SearchByDate(DataTable dtDoc)
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
			return ExecuteDataTable("APZ_SalaryInformation_SearchMainInfo", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public virtual DataTable GetMaxLineNum(int empId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@empID", empId)
			};
			return ExecuteDataTable("Select isnull(Max(LineNum),0) as MaxLineNum from APZ_OHSI where empID = @empID", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetLineNum  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
