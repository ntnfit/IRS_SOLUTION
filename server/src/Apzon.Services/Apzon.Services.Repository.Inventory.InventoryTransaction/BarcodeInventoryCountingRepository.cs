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

public class BarcodeInventoryCountingRepository : BaseService<DataSet>, IBarcodeInventoryCounting
{
	public BarcodeInventoryCountingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetDataStructure()
	{
		try
		{
			DataTable dataTable = SearchDoc(null);
			dataTable.TableName = "TableDoc";
			DataTable dataLineByDocEntry = GetDataLineByDocEntry(-1);
			dataLineByDocEntry.TableName = "TableLine";
			return new DataSet("StructureTable")
			{
				Tables = { dataTable, dataLineByDocEntry }
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable SearchDoc(DataTable dtSearch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (dtSearch.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["DocEntry"])))
			{
				list.Add(new SqlParameter("@docEntry", Function.ParseInt(dtSearch.Rows[0]["DocEntry"])));
			}
			else
			{
				list.Add(new SqlParameter("@docEntry", null));
			}
			if (dtSearch.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["CountDate"])))
			{
				list.Add(new SqlParameter("@countDate", Function.ParseDateTimes(dtSearch.Rows[0]["CountDate"])));
			}
			else
			{
				list.Add(new SqlParameter("@countDate", null));
			}
			if (dtSearch.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["TakerType"])))
			{
				list.Add(new SqlParameter("@takerType", Function.ParseInt(dtSearch.Rows[0]["TakerType"])));
			}
			else
			{
				list.Add(new SqlParameter("@takerType", null));
			}
			if (dtSearch.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["TakerId"])))
			{
				list.Add(new SqlParameter("@takerId", Function.ToString(dtSearch.Rows[0]["TakerId"])));
			}
			else
			{
				list.Add(new SqlParameter("@takerId", null));
			}
			if (dtSearch.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["WhsCode"])))
			{
				list.Add(new SqlParameter("@whsCode", Function.ToString(dtSearch.Rows[0]["WhsCode"])));
			}
			else
			{
				list.Add(new SqlParameter("@whsCode", null));
			}
			if (dtSearch.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["BinCode"])))
			{
				list.Add(new SqlParameter("@binCode", Function.ToString(dtSearch.Rows[0]["BinCode"])));
			}
			else
			{
				list.Add(new SqlParameter("@binCode", null));
			}
			if (dtSearch.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["Status"])))
			{
				list.Add(new SqlParameter("@status", Function.ParseInt(dtSearch.Rows[0]["Status"])));
			}
			else
			{
				list.Add(new SqlParameter("@status", null));
			}
			return ExecuteDataTable("APZ_BarcodeInventoryCounting_SearchDoc", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataLineByDocEntry(int docEntry)
	{
		try
		{
			return ExecuteDataTable("APZ_BarcodeInventoryCounting_GetDataLine", CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public int GetNewDocEntry()
	{
		try
		{
			return Function.ParseInt(Function.ParseInt(ExecuteScalar("SELECT MAX(\"DocEntry\") from APZ_OBIC;", CommandType.Text))) + 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public DataTable GetNextLastData(Enums.NavigateType type, int docEntry)
	{
		try
		{
			return ExecuteDataTable("APZ_BarcodeInventoryCounting_GetNextLastData", CommandType.StoredProcedure, new SqlParameter("@type", (int)type), new SqlParameter("@docEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataByBarcode(string barcodeString, string whsCode, string binCode)
	{
		try
		{
			return ExecuteDataTable("APZ_BarcodeInventoryCounting_GetDataCounting", CommandType.StoredProcedure, new SqlParameter("@barcodeString", barcodeString), new SqlParameter("@whsCode", whsCode), new SqlParameter("@binCode", binCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult AddNew(DataSet dsSave)
	{
		try
		{
			return Process(ApzObjectTypes.oBarcodeInventoryCounting, -1, Enums.TransactionType.A, dsSave);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult UpdateData(DataSet dsSave)
	{
		try
		{
			return Process(ApzObjectTypes.oBarcodeInventoryCounting, -1, Enums.TransactionType.U, dsSave);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string Delete(int docEntry)
	{
		try
		{
			return Process(ApzObjectTypes.oBarcodeInventoryCounting, -1, Enums.TransactionType.D, null, Function.ToString(docEntry)).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		if (transtype == Enums.TransactionType.A || transtype == Enums.TransactionType.U)
		{
			if (ds.Tables.Count != 2 || !ds.Tables[0].IsNotNull() || !ds.Tables[1].IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.Error, "Data save can not be null");
			}
			DataTable tableStruct = GetTableStruct("APZ_OBIC");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data header structure to save");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			DataTable dt = ds.Tables[0].DefaultView.ToTable(distinct: false, source.ToArray());
			DataTable tableStruct2 = GetTableStruct("APZ_BIC1");
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data Line structure to save");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			DataTable dt2 = ds.Tables[1].DefaultView.ToTable(distinct: false, source2.ToArray());
			CreateTempTable(dt, "#tempDoc");
			BulkCopy(dt, "#tempDoc");
			CreateTempTable(dt2, "#tempLine");
			BulkCopy(dt2, "#tempLine");
			DataTable dataTable = ExecuteDataTable("APZ_BarcodeInventoryCounting_Save", CommandType.StoredProcedure, new SqlParameter("@transType", Function.ToString(transtype)));
			if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
			{
				objId = Function.ToString(dataTable.Rows[0][0]);
				objNum = objId;
				return new HttpResult();
			}
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
		}
		if (transtype == Enums.TransactionType.D)
		{
			DataTable dataTable2 = ExecuteDataTable("APZ_BarcodeInventoryCounting_Delete", CommandType.StoredProcedure, new SqlParameter("@docEntry", Function.ParseInt(objId)));
			if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[0][1])))
			{
				objNum = objId;
				return new HttpResult(Enums.MessageCode.Success);
			}
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable2.Rows[0][0])}-{Function.ToString(dataTable2.Rows[0][1])}");
		}
		return new HttpResult(Enums.MessageCode.Error, "200001-function not support");
	}
}
