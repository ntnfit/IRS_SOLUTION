using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Inventory.InventoryTransaction;

namespace Apzon.Services.Repository.Inventory.InventoryTransaction;

public class GoodReceiptItRespository : BaseService<DocumentsImpl>, IGoodReceiptIt, IService<DocumentsImpl>
{
	private const string Temptable = "#APZ_DOC";

	private const string TemptableLine = "#APZ_DOCLINE";

	private const string TemmBarcode = "#tempBarcode";

	public GoodReceiptItRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetItemInStockAWarehouse(List<string> itAndWh)
	{
		try
		{
			new DataTable();
			string itemInStockAWarehouseQuery = GetItemInStockAWarehouseQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@itemcode", itAndWh[0]));
			list.Add(new SqlParameter("@whscode", itAndWh[1]));
			return ExecuteDataTable(itemInStockAWarehouseQuery, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetItemInStockAWarehouseQuery()
	{
		return "SELECT sum(isnull(OnHand,0)) from oitw where itemcode=@itemcode" + " and whscode=@whscode";
	}

	public DataSet SelectOne(int docEntry)
	{
		try
		{
			DataSet dataSet = ExecuteData("APZ_InventoryTransaction_GoodReceiptIT_GetLines", CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry));
			if (dataSet == null || dataSet.Tables.Count < 2)
			{
				return dataSet;
			}
			List<string> tableName = Function.GetTableName(ApzObjectTypes.oInventoryGenEntry);
			SetDefaultValueToTable(tableName[1], dataSet.Tables[0]);
			SetDefaultValueToTable("OBTN", dataSet.Tables[1]);
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data GoodReceipt - Line ", ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetDataDoc(int baseType, int docNum, int docEntry, int priList, string fromDate, string toDate, Enums.NavigateType type, string userCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@basetype", baseType),
				new SqlParameter("@docNum", docNum),
				new SqlParameter("@docEntry", docEntry),
				new SqlParameter("@priceList", priList),
				new SqlParameter("@fromDate", Function.ParseDateTime(fromDate, "ddMMyyyy")),
				new SqlParameter("@toDate", Function.ParseDateTime(toDate, "ddMMyyyy")),
				new SqlParameter("@type", (int)type),
				new SqlParameter("@userCode", userCode)
			};
			DataTable dataTable = ExecuteDataTable("APZ_GoodReceiptIt_GetDataDoc", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				List<string> tableName = Function.GetTableName(ApzObjectTypes.oInventoryGenEntry);
				SetDefaultValueToTable(tableName[0], dataTable);
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
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
				return Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxDocEntry FAILED ", ex.Message);
			return 0;
		}
	}

	protected virtual string GetMaxDocEntryQuery()
	{
		return "select isnull( MAX(DocEntry),0) as DocEntry from OIGN";
	}

	public string Insert(DocumentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(DocumentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(DocumentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public DocumentsImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public int GetNewDocNum(int series)
	{
		try
		{
			string query = "SELECT \"NextNumber\" FROM NNM1 WHERE \"Series\" = @series;";
			SqlParameter sqlParameter = new SqlParameter("@series", series);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["NextNumber"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNewDocNum ", ex.Message);
			return 0;
		}
	}

	public HttpResult ReportComplete(DataSet dtset)
	{
		try
		{
			HttpResult httpResult = GetUnitOfWork().Documents.SaveDataSet(dtset.Copy(), ApzObjectTypes.oInventoryGenEntry);
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					DataTable dt = dtset.Tables[1].DefaultView.ToTable(false, "Barcode", "Quantity");
					CreateTempTable(dt, "#tempBarcode");
					BulkCopy(dt, "#tempBarcode");
					ExecuteNonQuery("APZ_GenPoFromSchedule_AutoReportComplete", CommandType.StoredProcedure);
					dbTransaction.Commit();
					return new HttpResult();
				}
				dbTransaction.Rollback();
				return httpResult;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetDepartment(string requester)
	{
		try
		{
			new DataTable();
			string text = " SELECT \"Name\", \"Code\" FROM OUDP";
			if (!string.IsNullOrEmpty(requester))
			{
				text = text + " AND \"Requester\" ='" + requester + "'";
			}
			text += ";";
			return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetBranch(string requester)
	{
		try
		{
			new DataTable();
			string branchQuery = GetBranchQuery(requester);
			return ExecuteDataTable(branchQuery, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetBranchQuery(string requester)
	{
		string text = "SELECT A.Requester , A.ReqName, C.Name ,ISNULL(C.Code,0) AS Code\r\n                                From OIGE A\r\n                                left join OUBR C\r\n                                ON C.Code = A.Branch\r\n                                where 1=1";
		if (!string.IsNullOrEmpty(requester))
		{
			text = text + " AND Requester ='" + requester + "'";
		}
		return text;
	}

	public DataTable GetInStock(string itemCode)
	{
		try
		{
			new DataTable();
			string inStockQuery = GetInStockQuery(itemCode);
			return ExecuteDataTable(inStockQuery, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetInStockQuery(string itemCode)
	{
		string text = "SELECT ISNULL(A.OnHand,0) AS QuantityOnStock\r\n                                From OITM A\r\n                                left join IGE1 B\r\n                                ON A.ItemCode = B.ItemCode\r\n                                where 1=1";
		if (!string.IsNullOrEmpty(itemCode))
		{
			text = text + " AND A.ItemCode ='" + itemCode + "'";
		}
		return text;
	}

	public DataTable GetItemInStockAllWarehouse(string itemCode)
	{
		try
		{
			new DataTable();
			string query = "SELECT SUM(\"OnHand\") FROM OITW WHERE \"ItemCode\" = N'" + itemCode + "';";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetItemProductionOrder(int docentry)
	{
		try
		{
			string query = "APZ_InventoryTransaction_GoodIssueIT_GetItemsProdOrder";
			SqlParameter sqlParameter = new SqlParameter("@docentry", docentry);
			return ExecuteDataTable(query, CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetItemProductionOrder FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetGroupNum()
	{
		try
		{
			string query = "SELECT \"ListNum\" AS \"Value\", \"ListName\" AS \"Name\" FROM OPLN;";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetGroupNum FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetListNum(string key)
	{
		try
		{
			string query = "SELECT \"ListNum\" from OPLN where \"ListName\" = @ListName;";
			SqlParameter sqlParameter = new SqlParameter("@ListName", key)
			{
				SqlDbType = SqlDbType.NVarChar
			};
			return ExecuteDataTable(query, CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListNum FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataSetResult GetDataByBarcode(string barcode, string userName, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@barcode", barcode),
				new SqlParameter("@userName", userName),
				new SqlParameter("@branch", branch)
			};
			return new DataSetResult("", ExecuteData("APZ_GoodReceiptIt_GetDataByBarcode", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public HttpResult GetCurrentRate(DataTable dtSearch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@poEntry", dtSearch.Rows[0]["poEntry"]),
				new SqlParameter("@docDate", dtSearch.Rows[0]["docDate"])
			};
			return new HttpResult(Enums.MessageCode.Success, "", ExecuteScalar("APZ_GoodReceiptIt_GetCurrentRate", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataSetResult GetDataByDraftKey(string barcode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@barcode", barcode)
			};
			return new DataSetResult("", ExecuteData("Apz_GoodReceiptPO_GetDataByDraftKey", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}
}
