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
using Apzon.Services.Interfaces.HumanResources.Salary;

namespace Apzon.Services.Repository.HumanResources.Salary;

public class PerIncomeTaxRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IPerIncomeTax
{
	private string processType = "";

	private string Temp_PerIncomeTax = "#APZ_OPIT";

	private string Temp_Detail = "#APZ_PIT1";

	public DataTable FindDataNextLast_PerInTax(int id, int type)
	{
		try
		{
			string query = "APZ_FindNextLastFrist_PerInTax";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@id", id));
			list.Add(new SqlParameter("@type", type));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "FindDataNextLast  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetPerIncomeTax()
	{
		try
		{
			return ExecuteDataTable("Select * from APZ_OPIT Order by \"Name\";", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Load  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDetail(int code)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@code", code);
			return ExecuteDataTable("Select * from APZ_PIT1 where \"Code\" = @code;", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Load   FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public string RemovePerIncomeTax(int code)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				SqlParameter sqlParameter = new SqlParameter("@code", code);
				if (ExecuteNonQuery("Delete from APZ_OPIT where Code  NOT IN (select code from APZ_PIT1) AND  Code = @code", CommandType.Text, sqlParameter) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "Cannot remove name because it has been used in Detail";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Remove  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public int GetMaxCode()
	{
		try
		{
			string query = "SELECT MAX(Code)FROM APZ_OPIT";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxCode FAILED ", ex.Message);
			return 0;
		}
	}

	public HttpResult SavePersonalIncomeTax(DataSet ds, string type)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			switch (type)
			{
			case "A":
				transtype = Enums.TransactionType.A;
				break;
			case "U":
				transtype = Enums.TransactionType.U;
				break;
			case "D":
				transtype = Enums.TransactionType.D;
				break;
			}
			return Process(ApzObjectTypes.oPersonalIncomeTax, -1, transtype, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
			return DeleteData(ref ret, ref objId, ref objNum);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			DataTable dataTable = ds.Tables[0];
			DataTable tableStruct = GetTableStruct("APZ_OPIT");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, Temp_PerIncomeTax);
			BulkCopy(dataTable, Temp_PerIncomeTax);
			DataTable dataTable2 = ds.Tables[1];
			DataTable tableStruct2 = GetTableStruct("APZ_PIT1");
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			CreateTempTable(dataTable2, Temp_Detail);
			BulkCopy(dataTable2, Temp_Detail);
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable3 = ExecuteDataTable("APZ_SavePersonalIncomeTaxs", CommandType.StoredProcedure, sqlParameter);
			if (dataTable3 != null && dataTable3.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable3.Rows[0][1])))
				{
					objId = Function.ToString(dataTable3.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable3.Rows[0][0])}-{Function.ToString(dataTable3.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Save error!" : "Update error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string RemovePerIncomeTax(string code)
	{
		try
		{
			return Process(ApzObjectTypes.oPersonalIncomeTax, -1, Enums.TransactionType.D, new DataSet(), code).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@code", objId);
			if (ExecuteNonQuery("APZ_PersonalIncomeTax_DeleteDocs", CommandType.StoredProcedure, sqlParameter) > 0)
			{
				return new HttpResult(Enums.MessageCode.Success, "", "");
			}
			return new HttpResult(Enums.MessageCode.Error, "", "Delete Error!");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
