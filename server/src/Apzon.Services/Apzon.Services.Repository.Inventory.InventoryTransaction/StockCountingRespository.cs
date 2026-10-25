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
using Apzon.Services.Interfaces.Inventory.InventoryTransaction;

namespace Apzon.Services.Repository.Inventory.InventoryTransaction;

public class StockCountingRespository : BaseService<DataSet>, IStockCounting
{
	private const string Doctemp = "#tempdoc";

	private const string Linetemp = "#templine";

	public StockCountingRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable SearchDataByWarehouse(string whs)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "apz_InventoryCount_SearchByWhs";
			SqlParameter sqlParameter = new SqlParameter("@whs", whs);
			result = ExecuteDataTable(query, CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchDataByWarehouse ", ex.Message);
		}
		return result;
	}

	public DataTable SearchDataByItemCode(string whs, string itemCode)
	{
		DataTable result = new DataTable();
		try
		{
			StringBuilder stringBuilder = new StringBuilder("apz_InventoryCount_SearchByItemCode");
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@whs", Function.ToString(whs)),
				new SqlParameter("@itemCode", Function.ToString(itemCode))
			};
			result = ExecuteDataTable(stringBuilder.ToString(), CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchDataByItemCode ", ex.Message);
		}
		return result;
	}

	public DataTable GetTableLine(int id)
	{
		try
		{
			string query = "select *from INC1 where DocEntry=@DocEntry";
			SqlParameter sqlParameter = new SqlParameter("@DocEntry", id);
			return ExecuteDataTable(query, CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetNo()
	{
		try
		{
			return ExecuteDataTable("apz_counting_GetNo", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
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
			return Process(ApzObjectTypes.oStockCounting, -1, transtype, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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
		case Enums.TransactionType.R:
			return SaveDataDraft(dsSave, transtype, ref objId, ref objNum);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			DataTable dt = ds.Tables[0];
			CreateTempTable(dt, "#tempdoc");
			BulkCopy(dt, "#tempdoc");
			DataTable dataTable = ds.Tables[1];
			DataTable tableStruct = GetTableStruct("INC1");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, "#templine");
			BulkCopy(dataTable, "#templine");
			string query = "APZ_InventoryCounting_Save";
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			if (ExecuteDataTable(query, CommandType.StoredProcedure, sqlParameter) != null)
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

	public int GetNextNumber(int series)
	{
		try
		{
			string query = "select NextNumber from nnm1  where Series=@Series";
			SqlParameter sqlParameter = new SqlParameter("@Series", series);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 1;
		}
	}

	public int GetNextNumberDraft(int series)
	{
		try
		{
			string query = "select NextNumber from nnm1  where Series=@Series";
			SqlParameter sqlParameter = new SqlParameter("@Series", series);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 1;
		}
	}

	public DataTable GetNoDraf()
	{
		try
		{
			return ExecuteDataTable("apz_InventoryCounting_GetNoDraft", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable FindNextLastFrist(int docId, int type)
	{
		try
		{
			return ExecuteDataTable("APZ_InventoryCounting_FindNextLastFrist", CommandType.StoredProcedure, new SqlParameter("@id", docId), new SqlParameter("@type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public HttpResult SaveDraft(DataSet ds, string action)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.D;
			if (!(action == "D"))
			{
				if (action == "R")
				{
					transtype = Enums.TransactionType.R;
				}
			}
			else
			{
				transtype = Enums.TransactionType.D;
			}
			return Process(ApzObjectTypes.oStockCounting, -1, transtype, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult SaveDataDraft(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			DataTable dt = ds.Tables[0];
			CreateTempTable(dt, "#tempdoc");
			BulkCopy(dt, "#tempdoc");
			DataTable dt2 = ds.Tables[1];
			CreateTempTable(dt2, "#templine");
			BulkCopy(dt2, "#templine");
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			if (ExecuteDataTable("APZ_InventoryCounting_SaveDraft", CommandType.StoredProcedure, sqlParameter) != null)
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
