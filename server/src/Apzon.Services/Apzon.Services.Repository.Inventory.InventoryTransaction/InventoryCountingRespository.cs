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
using Apzon.Services.Interfaces.Inventory.InventoryTransaction;

namespace Apzon.Services.Repository.Inventory.InventoryTransaction;

public class InventoryCountingRespository : BaseService<DataSet>, IInventoryCounting
{
	private const string Doctemp = "#APZ_OINC";

	private const string Linetemp = "#APZ_INC1";

	private const string Batchtemp = "#APZ_INC2";

	public InventoryCountingRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable LoadData(int id, string whs, string startFrom, string startTo, string status)
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(startFrom, "yyyyMMdd");
			DateTime? dateTime2 = Function.ParseDateTime(startTo, "yyyyMMdd");
			string query = "APZ_IVCounting_LoadData";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@id", id), new SqlParameter("@startFrom", dateTime), new SqlParameter("@startTo", dateTime2), new SqlParameter("@whs", whs), new SqlParameter("@status", status));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadData", ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDataById(int Id)
	{
		try
		{
			return ExecuteData("APZ_IVCounting_GetLineById", CommandType.StoredProcedure, new SqlParameter("@Id", Id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
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
			return Process(ApzObjectTypes.oInventoryCounting, -1, transtype, ds);
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
		Enums.TransactionType transactionType = transtype;
		if ((uint)transactionType <= 1u)
		{
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		}
		return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
	}

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			if (ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data must not null!");
			}
			DataTable tableStruct = GetTableStruct("APZ_OINC");
			DataTable dataTable = ds.Tables[0];
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, "#APZ_OINC");
			BulkCopy(dataTable, "#APZ_OINC");
			DataTable tableStruct2 = GetTableStruct("APZ_INC1");
			DataTable dataTable2 = ds.Tables[1];
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			CreateTempTable(dataTable2, "#APZ_INC1");
			BulkCopy(dataTable2, "#APZ_INC1");
			DataTable tableStruct3 = GetTableStruct("APZ_INC2");
			DataTable dataTable3 = ds.Tables[2];
			if (!tableStruct3.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source3 = from DataColumn t in tableStruct3.Columns
				select t.ColumnName;
			dataTable3 = dataTable3.DefaultView.ToTable(distinct: false, source3.ToArray());
			CreateTempTable(dataTable3, "#APZ_INC2");
			BulkCopy(dataTable3, "#APZ_INC2");
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable4 = ExecuteDataTable("APZ_IVCounting_Save", CommandType.StoredProcedure, sqlParameter);
			if (dataTable4 != null && dataTable4.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable4.Rows[0][1])))
				{
					objId = Function.ToString(dataTable4.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable4.Rows[0][0])}-{Function.ToString(dataTable4.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Save error!" : "Update error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string CheckBatchSerNum(string itemCode)
	{
		try
		{
			return Function.ToString(ExecuteScalar("APZ_IVCounting_CheckBatchSerial", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public int GetMaxDocEntry()
	{
		try
		{
			string maxDocEntryQuery = GetMaxDocEntryQuery();
			DataTable dataTable = ExecuteDataTable(maxDocEntryQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxDocEntry FAILED ", ex.Message);
			return 0;
		}
	}

	protected virtual string GetMaxDocEntryQuery()
	{
		return "SELECT MAX(DocEntry) FROM APZ_OINC";
	}

	public HttpResult UpdateStatus(int docEntry, string status)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_IVCounting_UpdateStatus", CommandType.StoredProcedure, new SqlParameter("@status", status), new SqlParameter("@docEntry", docEntry));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
				{
					return new HttpResult(Enums.MessageCode.Success);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, "Update error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable FindNextLastFrist(int docId, int type)
	{
		try
		{
			return ExecuteDataTable("APZ_IVCounting_FindNextLastFrist", CommandType.StoredProcedure, new SqlParameter("@id", docId), new SqlParameter("@type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetOnHandByItem(string itemCode, string whs)
	{
		try
		{
			return ExecuteDataTable("Select \"OnHand\" From OITW Where \"ItemCode\" = @itemCode And \"WhsCode\" = @whs;", CommandType.Text, new SqlParameter("@itemCode", itemCode), new SqlParameter("@whs", whs));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
