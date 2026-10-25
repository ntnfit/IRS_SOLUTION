using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup.Inventory;

namespace Apzon.Services.Repository.Administrator.Setup.Inventory;

public class CycleCountDeterminationSubTableRepository : BaseService<DataTable>, ICycleCountDeterminationSubTable
{
	private const string TempLine = "#templine";

	public CycleCountDeterminationSubTableRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetInventoryCycles()
	{
		try
		{
			return ExecuteDataTable("APZ_CycleCountDeterminationSubtable_GetInventoryCycles", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetInventoryCycles  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetByCycles(int cyclesBy)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@cyclesBy", cyclesBy);
			return ExecuteDataTable("APZ_CycleCountDeterminationSubtable_GetByCycles", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetByCycles  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetUsers()
	{
		try
		{
			return ExecuteDataTable("APZ_CycleCountDeterminationSubtable_GetUsers", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetUsers  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetByWhsCode(string whsCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@whsCode", whsCode);
			return ExecuteDataTable("APZ_CycleCountDeterminationSubtable_GetByWhsCode", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetByWhsCode  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable SearchDataBywhs(string whs, int cycleBy)
	{
		DataTable result = new DataTable();
		try
		{
			StringBuilder stringBuilder = new StringBuilder("apz_CycleCountDeterminationSubtable_GetByWhs");
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@whs", Function.ToString(whs)),
				new SqlParameter("@cycleBy", Function.ParseInt(cycleBy))
			};
			result = ExecuteDataTable(stringBuilder.ToString(), CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchDataBywhs ", ex.Message);
		}
		return result;
	}

	public DataTable GetByCycleCode(int cycleCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@cycleCode", cycleCode);
			return ExecuteDataTable("APZ_CycleCountDeterminationSubtable_GetByCycleCode", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetByCycleCode  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataTable ds, string action)
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
			return Process(ApzObjectTypes.oCycleCountDeterminationSubTable, -1, transtype, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dsSave, ref string objId, ref string objNum)
	{
		new HttpResult();
		Enums.TransactionType transactionType = transtype;
		if ((uint)transactionType <= 1u)
		{
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		}
		return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
	}

	public HttpResult SaveData(DataTable doc, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			if (doc == null || doc.Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data must not null!");
			}
			DataTable tableStruct = GetTableStruct("CCS1");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			doc = doc.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(doc, "#templine");
			BulkCopy(doc, "#templine");
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			if (ExecuteDataTable("APZ_CycleCountDeterminationSubtable_Save", CommandType.StoredProcedure, sqlParameter) != null)
			{
				return new HttpResult(Enums.MessageCode.Success, "", "Save success!");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Save error!" : "Update error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
