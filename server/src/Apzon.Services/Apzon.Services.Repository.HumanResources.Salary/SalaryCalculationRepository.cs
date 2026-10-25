using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources.Salary;

namespace Apzon.Services.Repository.HumanResources.Salary;

public class SalaryCalculationRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), ISalaryCalculation
{
	private string _saveType;

	private string Temp_SalInfo = "#APZ_SalMainInfo";

	private string Temp_SalAllowance = "#APZ_SalAllowance";

	private string Temp_SalDeduction = "#APZ_SalDeduction";

	private string Temp_SalOverTime = "#APZ_SalOvertime";

	private string Temp_SendMailStt = "#APZ_SendMailStt";

	private string Temp_FileName = "#APZ_SAL5";

	private string processType;

	public string SaveDoc(DataSet ds)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			return Process(ApzObjectTypes.oSalCalculation, -1, transtype, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string SaveSendMailStt(DataSet ds)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.U;
			return Process(ApzObjectTypes.oSalCalculation, -1, transtype, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string SaveRptFileName(DataSet ds)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			_saveType = "F";
			return Process(ApzObjectTypes.oSalCalculation, -1, transtype, ds).Message;
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
			ProcessSave2(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.D:
			ProcessDelete(ds, ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessSave(ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (_saveType == "F")
			{
				if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
				{
					ret.Message = "No data to save";
					return;
				}
				CreateTempTable(ds.Tables[0], Temp_FileName);
				BulkCopy(ds.Tables[0], Temp_FileName);
				DataTable dataTable = ExecuteDataTable("APZ_SaveRptFileName", CommandType.StoredProcedure, (SqlParameter[])null);
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
				return;
			}
			if (ds == null || ds.Tables.Count < 3 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				ret.Message = "No data to save";
				return;
			}
			_ = transtype;
			Task task = Task.Run(() =>
			{
				CreateTempTable(ds.Tables["dtMainInfo"], Temp_SalInfo);
				BulkCopy(ds.Tables["dtMainInfo"], Temp_SalInfo);
			});
			Task task2 = Task.Run(() =>
			{
				CreateTempTable(ds.Tables["dtAllowance"], Temp_SalAllowance);
				BulkCopy(ds.Tables["dtAllowance"], Temp_SalAllowance);
			});
			Task task3 = Task.Run(() =>
			{
				CreateTempTable(ds.Tables["dtDeduction"], Temp_SalDeduction);
				BulkCopy(ds.Tables["dtDeduction"], Temp_SalDeduction);
			});
			Task task4 = Task.Run(() =>
			{
				CreateTempTable(ds.Tables["dtOverTime"], Temp_SalOverTime);
				BulkCopy(ds.Tables["dtOverTime"], Temp_SalOverTime);
			});
			Task.WaitAll(task, task2, task3, task4);
			DataTable dataTable2 = ExecuteDataTable("apz_process_SaveSalCalculation", CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				ret.Message = Function.ToString(dataTable2.Rows[0]["ErrorMessage"]);
				string text2 = Function.ToString(dataTable2.Rows[0]["ErrorNumber"]);
				ret.Message = text2 + " - " + ret.Message;
				return;
			}
			ret.Message = "";
			string text3 = Function.ToString(ds.Tables[0].Rows[0]["Period"]);
			List<string> list = new List<string>();
			foreach (int item in (from t in ds.Tables[0].AsEnumerable()
				select t.Field<int>("EmpId")).Distinct().ToList())
			{
				list.Add(string.Join(",", text3, item));
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

	private void ProcessSave2(ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (!ds.IsNotNull())
			{
				ret.Message = "No data to save";
			}
			else
			{
				CreateTempTable(ds.Tables[0], Temp_SendMailStt);
				BulkCopy(ds.Tables[0], Temp_SendMailStt);
				DataTable dataTable = ExecuteDataTable("APZ_SalaryCalculation_SaveSendMailStt", CommandType.StoredProcedure, (SqlParameter[])null);
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

	private void ProcessDelete(DataSet ds, ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			int num = ExecuteNonQuery("", CommandType.StoredProcedure, list.ToArray());
			ret.Message = ((num > 0) ? "" : "Delete error! ");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
			ret.Message = "Update error: " + ex.Message;
		}
	}

	public DataSet Calculalte(DataTable dtDoc)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@empNo", dtDoc.Rows[0]["empId"]),
				new SqlParameter("@period", dtDoc.Rows[0]["Period"]),
				new SqlParameter("@branch", dtDoc.Rows[0]["branch"])
			};
			return ExecuteData("APZ_SalaryCalculation_Calculate1", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public DataSet ViewSalaryCalculation(DataTable dtDoc)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@empNo", dtDoc.Rows[0]["empId"]),
				new SqlParameter("@period", dtDoc.Rows[0]["Period"]),
				new SqlParameter("@branch", dtDoc.Rows[0]["branch"])
			};
			return ExecuteData("APZ_SalaryCalculation_View", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public DataTable SearchEmployees(DataTable dtDoc)
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
				new SqlParameter("@period", dtDoc.Rows[0]["Period"])
			};
			return ExecuteDataTable("APZ_SalaryCalculation_SearchEmployees", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetSalarySlip(DataTable dtDoc)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@IDNV", dtDoc.Rows[0]["listEmpId"]),
				new SqlParameter("@branch", dtDoc.Rows[0]["branch"]),
				new SqlParameter("@period", dtDoc.Rows[0]["period"])
			};
			return ExecuteDataTable("APZ_SALARY_SLIPV1", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetRptFileName()
	{
		try
		{
			return ExecuteDataTable("Select Top 1 RptFileName from APZ_SAL5", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
