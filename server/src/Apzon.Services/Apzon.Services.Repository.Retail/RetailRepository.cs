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
using Apzon.Services.Interfaces.Retail;

namespace Apzon.Services.Repository.Retail;

public class RetailRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IRetail
{
	private const string TableDoc = "APZ_RSRC";

	private const string TableLine = "APZ_SRC1";

	private const string TablePayment = "APZ_SRC2";

	private const string TableCredit = "APZ_SRC3";

	private const string TempDocument = "#APZ_RSRC";

	private const string TempDocumentLine = "#APZ_SRC1";

	private const string TempDocumentPayment = "#APZ_SRC2";

	private const string TempDocumentCredit = "#APZ_SRC3";

	private const string TableSO = "ORDR";

	private const string TableSOLine = "RDR1";

	private const string TempTableSO = "#ORDR";

	private const string TempTableSOLine = "#RDR1";

	private string _processType = "";

	public DataTable GetDocByCode(string userName, string branch, string barCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@Branch", branch),
				new SqlParameter("@ItemCode", barCode)
			};
			return ExecuteDataTable("APZ_Retail_GetByCode", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Doc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable SearchTransactions(DataTable dtSearch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@DocNum", Function.ParseInt(dtSearch.Rows[0]["DocNum"])),
				new SqlParameter("@CardCode", Function.ToString(dtSearch.Rows[0]["CardCode"])),
				new SqlParameter("@FromDate", Function.ParseDateTimes(dtSearch.Rows[0]["DateFrom"])),
				new SqlParameter("@ToDate", Function.ParseDateTimes(dtSearch.Rows[0]["DateTo"])),
				new SqlParameter("@StoreCode", Function.ToString(dtSearch.Rows[0]["StoreCode"])),
				new SqlParameter("@ItemCode", Function.ToString(dtSearch.Rows[0]["ItemCode"]))
			};
			return ExecuteDataTable("APZ_Retail_SearchTransactions", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Doc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetTransactionDetails(int docEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@DocEntry", docEntry)
			};
			DataSet dataSet = ExecuteData("APZ_Retail_GetTransDetail", CommandType.StoredProcedure, list.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				return dataSet;
			}
			return new DataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Doc  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public HttpResult Save(DataSet ds)
	{
		return Process(ApzObjectTypes.oRetail, -1, Enums.TransactionType.U, ds);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			if (_processType.ToUpper().Equals("CLOCK"))
			{
				return ProcessSaveClock(document, ref objId, ref objNum, ref transtype);
			}
			if (Exists(document))
			{
				transtype = Enums.TransactionType.U;
				return UpdateData(document, ref objId, ref objNum, ref transtype);
			}
			return AddNew(document, ref objId, ref objNum, ref transtype);
		default:
			return new HttpResult();
		}
	}

	private HttpResult AddNew(DataSet doc, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		if (doc.IsNotNull())
		{
			transtype = Enums.TransactionType.A;
			int series = -1;
			int cancelEntry = 0;
			InitData(doc, ref objId, ref objNum, ref series, transtype, ref cancelEntry);
			BeforeAction(doc, objId, objNum, transtype, cancelEntry);
			string query = string.Format("insert into {0} select * from {1} \r\n                                            update {2} set DocEntry=@DocEntry, LineStatus='O'\r\n                                            insert into {3} select * from {2} where {2}.ItemCode is not null and {2}.ItemCode <>'' and {2}.Quantity>0\r\n                                            update {4} set DocEntry=@DocEntry\r\n                                            insert into {5} select * from {4} \r\n                                            update {7} set DocEntry =@DocEntry\r\n                                            insert into {6} select * from {7}\r\n                                            update onnm set AutoKey=@DocEntry+1 where ObjectCode=@ObjCode\r\n                                            update nnm1 set NextNumber=@DocNum+1 WHERE ObjectCode=@ObjCode AND ISNULL(Series,-1)=@Series", "APZ_RSRC", "#APZ_RSRC", "#APZ_SRC1", "APZ_SRC1", "#APZ_SRC2", "APZ_SRC2", "#APZ_SRC3", "APZ_SRC3");
			ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@DocEntry", objId), new SqlParameter("@ObjCode", Function.ToString(700009)), new SqlParameter("@DocNum", objNum), new SqlParameter("@Series", series));
			AfterAction(doc, objId, objNum, transtype, cancelEntry);
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	private void BeforeAction(DataSet doc, string objId, string objNum, Enums.TransactionType transtype, int cancelEntry)
	{
		DataTable dataTable = ExecuteDataTable("apz_retail_beforeaction", CommandType.StoredProcedure, new SqlParameter("@obj_id", objId), new SqlParameter("@transtype", transtype.ToString()));
		if (dataTable.IsNotNull())
		{
			int num = Function.ParseInt(dataTable.Rows[0][0]);
			if (num > 0)
			{
				string text = Function.ToString(dataTable.Rows[0][1]);
				throw new Exception(num + "-" + text);
			}
		}
	}

	private void AfterAction(DataSet doc, string objId, string objNum, Enums.TransactionType transtype, int cancelEntry)
	{
		DataTable dataTable = ExecuteDataTable("apz_retail_afteraction", CommandType.StoredProcedure, new SqlParameter("@obj_id", objId), new SqlParameter("@transtype", transtype.ToString()));
		if (dataTable.IsNotNull())
		{
			int num = Function.ParseInt(dataTable.Rows[0][0]);
			if (num > 0)
			{
				string text = Function.ToString(dataTable.Rows[0][1]);
				throw new Exception(num + "-" + text);
			}
		}
	}

	private void InitData(DataSet doc, ref string objId, ref string objNum, ref int series, Enums.TransactionType transtype, ref int cancelEntry)
	{
		DataTable dataTable = doc.Tables[0];
		DataTable tableStruct = GetTableStruct("APZ_RSRC");
		if (!tableStruct.IsNotNull())
		{
			throw new Exception("Could not get data structure");
		}
		IEnumerable<string> source = from DataColumn t in tableStruct.Columns
			select t.ColumnName;
		dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
		int num = 0;
		int num2 = 0;
		switch (transtype)
		{
		case Enums.TransactionType.A:
		{
			num = Function.ParseInt(GetObjectId(ApzObjectTypes.oRetail));
			dataTable.Rows[0]["DocEntry"] = num;
			int num3 = Function.ParseInt(dataTable.Rows[0]["BPLId"]);
			num3 = ((num3 == 0) ? (-1) : num3);
			series = Function.ParseInt(dataTable.Rows[0]["Series"]);
			int usersign = Function.ParseInt(dataTable.Rows[0]["UserSign"]);
			num2 = Function.ParseInt(GetObjectNum(ApzObjectTypes.oRetail, num3, series, usersign));
			dataTable.Rows[0]["DocNum"] = num2;
			break;
		}
		case Enums.TransactionType.U:
			num = Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			num2 = Function.ParseInt(dataTable.Rows[0]["DocNum"]);
			break;
		}
		CreateTempTable(dataTable, "#APZ_RSRC");
		BulkCopy(dataTable, "#APZ_RSRC");
		DataTable dataTable2 = doc.Tables[1];
		DataTable tableStruct2 = GetTableStruct("APZ_SRC1");
		if (!tableStruct2.IsNotNull())
		{
			throw new Exception("Could not get data line structure");
		}
		source = from DataColumn t in tableStruct2.Columns
			select t.ColumnName;
		dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source.ToArray());
		CreateTempTable(dataTable2, "#APZ_SRC1");
		BulkCopy(dataTable2, "#APZ_SRC1");
		DataTable dataTable3 = doc.Tables[2];
		DataTable tableStruct3 = GetTableStruct("APZ_SRC2");
		if (!tableStruct3.IsNotNull())
		{
			throw new Exception("Could not get data payment structure");
		}
		source = from DataColumn t in tableStruct3.Columns
			select t.ColumnName;
		dataTable3 = dataTable3.DefaultView.ToTable(distinct: false, source.ToArray());
		CreateTempTable(dataTable3, "#APZ_SRC2");
		BulkCopy(dataTable3, "#APZ_SRC2");
		ExecuteNonQuery("select top 0 * into #APZ_SRC3 from APZ_SRC3", CommandType.Text);
		objId = num.ToString();
		objNum = num2.ToString();
	}

	private HttpResult UpdateData(DataSet document, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		return new HttpResult();
	}

	private bool Exists(DataSet document)
	{
		return false;
	}

	public DataSet GetStruct()
	{
		try
		{
			DataSet dataSet = new DataSet();
			DataTable tableStruct = GetTableStruct("ORDR");
			dataSet.Tables.Add(tableStruct.Copy());
			tableStruct = GetTableStruct("RDR1");
			dataSet.Tables.Add(tableStruct.Copy());
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable RetailInventorySearch(string itemCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_RetailInventory_Search";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@ItemCode", itemCode)
			};
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetReceiptData(int docEntry, int type)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "USP_POS_INVOICE";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@DocID", docEntry),
				new SqlParameter("@type", type)
			};
			result = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetSummaryData(string storeCode, int posID)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "USP_SAL_SalesAndTaxSum";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@StoreCode", storeCode),
				new SqlParameter("@PosID", posID)
			};
			result = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetRefundLines(int docEntry)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "apz_Retail_GetRefundLine";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@DocNum", docEntry)
			};
			result = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetAllTaxCodes()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "apz_Retail_GetTaxCode";
			result = ExecuteDataTable(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable CheckClock(string storeCode, int posID)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@StoreCode", storeCode),
				new SqlParameter("@PosID", posID)
			};
			return ExecuteDataTable("APZ_Retail_CLOCK_GetDoc", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string SaveClock(DataTable dt)
	{
		_processType = "Clock";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oRetail, -1, Enums.TransactionType.A, dataSet).Message;
	}

	private HttpResult ProcessSaveClock(DataSet dtS, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		HttpResult httpResult = new HttpResult();
		try
		{
			if (!dtS.IsNotNull())
			{
				httpResult.Message = "No data to save";
			}
			else
			{
				_ = transtype;
				CreateTempTable(dtS.Tables[0], "#APZ_CLOCK");
				BulkCopy(dtS.Tables[0], "#APZ_CLOCK");
				DataTable dataTable = ExecuteDataTable("apz_retail_SaveClock", CommandType.StoredProcedure, (SqlParameter[])null);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					httpResult.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
					string text = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
					httpResult.Message = text + " - " + httpResult.Message;
				}
				else
				{
					httpResult.Message = "";
				}
			}
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				objId = "";
				objNum = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			httpResult.Message = ex.Message;
		}
		return httpResult;
	}

	public DataTable GetSODocEntry(string storeCode, int posID)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@StoreCode", storeCode),
				new SqlParameter("@PosID", posID)
			};
			return ExecuteDataTable("SP_Retail_GetDocEntry", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetPromotionsData(DataSet ds)
	{
		try
		{
			DataTable result;
			using (DatabaseService.OpenConnection())
			{
				CreateTempTable(ds.Tables[0], "#ORDR");
				BulkCopy(ds.Tables[0], "#ORDR");
				CreateTempTable(ds.Tables[1], "#RDR1");
				BulkCopy(ds.Tables[1], "#RDR1");
				string query = "APZ_Retail_GetPromotions";
				result = ExecuteDataTable(query, CommandType.StoredProcedure);
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
