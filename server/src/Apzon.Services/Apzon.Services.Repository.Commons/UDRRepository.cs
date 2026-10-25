using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class UDRRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IUDRs
{
	protected string Temp_OUDR = "#APZ_OUDR";

	protected string Temp_UDR1 = "#APZ_UDR1";

	protected string Temp_UDR2 = "#APZ_UDR2";

	public DataTable GetListUDR()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "INSERT INTO APZ_PRINTLAYOUT (TypeCode, TypeName, DocCode, DocName,FileName) \r\n                               SELECT Code, Name, TableName, MenuCapt,'' FROM OUDR \r\n                               WHERE NOT EXISTS \r\n                                                (SELECT * FROM APZ_PRINTLAYOUT \r\n                                                 WHERE OUDR.Code = APZ_PRINTLAYOUT.TypeCode);";
			ExecuteNonQueryNoParam(query);
			string query2 = "INSERT INTO dbo.OMNI (menucode, MenuName, FMenuName, Leves, FatherNum, Enbled)\r\n                               SELECT 'frm_' + Code, MenuCapt, 'User Defined Object', 0, 'UserDefinedObject', 'Y'\r\n                               FROM dbo.OUDR\r\n\t\t                       WHERE NOT EXISTS ( SELECT 1 FROM dbo.OMNI\r\n\t\t\t\t\t\t\t                       WHERE OUDR.Code = REPLACE(OMNI.menucode, 'frm_', '') ) \r\n                                                         AND MenuItem = 'Y'";
			ExecuteNonQueryNoParam(query2);
			string query3 = "select * from OUDR";
			result = ExecuteDataTable(query3, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListUDR ", ex.Message);
		}
		return result;
	}

	public DataTable GetUDRByCode(Common.Table_Code item)
	{
		DataTable result = new DataTable();
		try
		{
			string query = $"select * from [{item.Name}] where Code ='{item.Code}'";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetUDRByCode ", ex.Message);
		}
		return result;
	}

	public DataSetResult GetParamByStore(string storeName, string code, string userName, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (string.IsNullOrEmpty(code))
			{
				code = "";
			}
			list.Add(new SqlParameter("@code", code));
			if (string.IsNullOrEmpty(storeName))
			{
				storeName = "";
			}
			list.Add(new SqlParameter("@storeName", storeName));
			if (string.IsNullOrEmpty(userName))
			{
				userName = "";
			}
			list.Add(new SqlParameter("@username", userName));
			list.Add(new SqlParameter("@branch", branch));
			return new DataSetResult("", ExecuteData("Apz_UDR_GetParamByStore", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public DataSetResult GetListReport(string userName, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@username", userName));
			list.Add(new SqlParameter("@branch", branch));
			list.Add(new SqlParameter("@type", "N"));
			return new DataSetResult("", ExecuteData("Apz_UDR_GetListReport", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public HttpResult Save(DataSet ds, string action)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			switch (action)
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
			return Process(ApzObjectTypes.oUserDefinedReports, -1, transtype, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum, string objcode = "")
	{
		return base.ProcessData(objtype, bplId, ref transtype, document, ref objId, ref objNum, objcode);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public virtual HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			if (ds == null || (ds.Tables.Count < 2 && tranType != Enums.TransactionType.D) || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				httpResult.Message = "No data to save";
				return httpResult;
			}
			DataTable dt = StandardizedTable(ds.Tables["dtOUDR"], "APZ_OUDR");
			CreateTempTable(dt, Temp_OUDR);
			BulkCopy(dt, Temp_OUDR);
			if (tranType != Enums.TransactionType.D)
			{
				if (ds.Tables["dtUDR1"].IsNotNull())
				{
					dt = StandardizedTable(ds.Tables["dtUDR1"], "APZ_UDR1");
					CreateTempTable(dt, Temp_UDR1);
					BulkCopy(dt, Temp_UDR1);
				}
				if (ds.Tables["dtUDR2"].IsNotNull())
				{
					dt = StandardizedTable(ds.Tables["dtUDR2"], "APZ_UDR2");
					CreateTempTable(dt, Temp_UDR2);
					BulkCopy(dt, Temp_UDR2);
				}
			}
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable = ExecuteDataTable("Apz_UDR_SaveProcess", CommandType.StoredProcedure, sqlParameter);
			if (dataTable.IsNotNull())
			{
				if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
				{
					return new HttpResult();
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.D) ? "Delete error!" : "Save error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetParamsByCode(string code)
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM APZ_UDR1 WHERE \"Code\" = @Code", CommandType.Text, new SqlParameter("@Code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public DataTable GetQueryByCode(string code)
	{
		try
		{
			return ExecuteDataTable("SELECT \"Code\",\"StoreName\",\"QueryType\" FROM APZ_OUDR WHERE \"Code\" = @Code", CommandType.Text, new SqlParameter("@Code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}
}
