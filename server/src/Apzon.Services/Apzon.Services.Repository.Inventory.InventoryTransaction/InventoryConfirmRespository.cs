using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Inventory;
using Apzon.Services.Interfaces.Inventory.InventoryTransaction;

namespace Apzon.Services.Repository.Inventory.InventoryTransaction;

public class InventoryConfirmRespository : BaseService<InventoryConfirmImpl>, IInventoryConfirm
{
	public InventoryConfirmRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public int GetMaxDocEntry()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(DocEntry),0) as maxCode\r\n                                    from [@INVCOM_M]";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["maxCode"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMax DocEntry  FAILED ", ex.Message);
			return 0;
		}
	}

	public DataSet SearchDataByBarCode(string barcode, string whs)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "apz_InventoryConfrim_GetItem";
			result = ExecuteData(query, CommandType.StoredProcedure, new SqlParameter("@BatchNo", barcode), new SqlParameter("@whs", whs));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchDataByBarCode ", ex.Message);
		}
		return result;
	}

	public string GetRecord(int docEntry, int type)
	{
		DataTable dataTable = new DataTable();
		int num = 0;
		try
		{
			string query = "APZ_InventoryConfirm_GetRecord_NextLastFirtPrev";
			dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry), new SqlParameter("@Type", type));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				num = dataTable.Rows[0].Field<int>("DocEntry");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRecord ", ex.Message);
		}
		return num.ToString();
	}

	public DataSet GetLineItem(int docEntry)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "apz_InventoryConfrim_GetById";
			result = ExecuteData(query, CommandType.StoredProcedure, new SqlParameter("@Id", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetLineItem ", ex.Message);
		}
		return result;
	}
}
