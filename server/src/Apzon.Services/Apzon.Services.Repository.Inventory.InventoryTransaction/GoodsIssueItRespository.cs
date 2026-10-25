using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Inventory.InventoryTransaction;

namespace Apzon.Services.Repository.Inventory.InventoryTransaction;

public class GoodsIssueItRespository : BaseService<DocumentsImpl>, IGoodsIssueIt, IService<DocumentsImpl>
{
	private const string Temptable = "#APZ_DOC";

	private const string TemptableLine = "#APZ_DOCLINE";

	public GoodsIssueItRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet SelectOne(int idDocEntry)
	{
		try
		{
			DataSet dataSet = ExecuteData("APZ_InventoryTransaction_GoodIssueIT_GetLines", CommandType.StoredProcedure, new SqlParameter("@docEntry", idDocEntry));
			if (dataSet == null || dataSet.Tables.Count < 2)
			{
				return dataSet;
			}
			List<string> tableName = GetTableName(ApzObjectTypes.oInventoryGenExit);
			SetDefaultValueToTable(tableName[1], dataSet.Tables[0]);
			SetDefaultValueToTable("OBTN", dataSet.Tables[1]);
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
			DataTable dataTable = ExecuteDataTable("APZ_GoodsIssueIt_GetDataDoc", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				List<string> tableName = GetTableName(ApzObjectTypes.oInventoryGenExit);
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
				return Convert.ToInt32(dataTable.Rows[0]["MaxDocEntry"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	protected virtual string GetMaxDocEntryQuery()
	{
		return "SELECT ISNULL( MAX(DocEntry ),0) as MaxDocEntry\r\n                            FROM OIGE ";
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

	public DataTable GetDepartment(string requester)
	{
		try
		{
			new DataTable();
			string departmentQuery = GetDepartmentQuery(requester);
			return ExecuteDataTable(departmentQuery, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDepartment  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetDepartmentQuery(string requester)
	{
		string text = "SELECT A.Requester , A.ReqName, b.Name , ISNULL(B.Code,0) AS Code\r\n                            From OIGE A\r\n                            LEFT JOIN OUDP B\r\n                            ON B.Code = A.Department\r\n                            where 1=1";
		if (!string.IsNullOrEmpty(requester))
		{
			text = text + " AND Requester ='" + requester + "'";
		}
		return text;
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBranch  FAILED ", ex.Message);
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetInStock  FAILED ", ex.Message);
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

	public DataTable GetGroupNum()
	{
		try
		{
			string query = "SELECT \"ListNum\" AS \"Value\", \"ListName\" AS \"Name\" FROM OPLN;";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetGroupNum  FAILED ", ex.Message);
			return new DataTable();
		}
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNewDocNum  FAILED ", ex.Message);
			return 0;
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC LINE  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetListNum(string key)
	{
		try
		{
			string query = "SELECT \"ListNum\" from OPLN where \"ListName\" = @ListName;";
			SqlParameter sqlParameter = new SqlParameter("@ListName", key);
			sqlParameter.SqlDbType = SqlDbType.NVarChar;
			return ExecuteDataTable(query, CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC LINE  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public List<string> GetTableName(ApzObjectTypes type)
	{
		List<string> list = new List<string>();
		string item = string.Empty;
		string item2 = string.Empty;
		switch (type)
		{
		case ApzObjectTypes.oQuotations:
			item = "OQUT";
			item2 = "QUT1";
			break;
		case ApzObjectTypes.oOrders:
			item = "ORDR";
			item2 = "RDR1";
			break;
		case ApzObjectTypes.oDeliveryNotes:
			item = "ODLN";
			item2 = "DLN1";
			break;
		case ApzObjectTypes.oInvoices:
			item = "OINV";
			item2 = "INV1";
			break;
		case ApzObjectTypes.oCreditNotes:
			item = "ORIN";
			item2 = "RIN1";
			break;
		case ApzObjectTypes.oBlanketAgreement:
			item = "OOAT";
			item2 = "OAT1";
			break;
		case ApzObjectTypes.oReturns:
			item = "ORDN";
			item2 = "RDN1";
			break;
		case ApzObjectTypes.oPurchaseQuotations:
			item = "OPQT";
			item2 = "PQT1";
			break;
		case ApzObjectTypes.oPurchaseOrders:
			item = "OPQT";
			item2 = "PQT1";
			break;
		case ApzObjectTypes.oPurchaseRequest:
			item = "OIGE";
			item2 = "IGE1";
			break;
		case ApzObjectTypes.oPurchaseInvoices:
			item = "OPCH";
			item2 = "PCH1";
			break;
		case ApzObjectTypes.oPurchaseDeliveryNotes:
			item = "OPDN";
			item2 = "PDN1";
			break;
		case ApzObjectTypes.oPurchaseCreditNotes:
			item = "ORPC";
			item2 = "RPC1";
			break;
		case ApzObjectTypes.oPurchaseReturns:
			item = "ORPD";
			item2 = "RPD1";
			break;
		case ApzObjectTypes.oInventoryGenEntry:
			item = "OIGN";
			item2 = "IGN1";
			break;
		case ApzObjectTypes.oInventoryGenExit:
			item = "OIGE";
			item2 = "IGE1";
			break;
		case ApzObjectTypes.oDrafts:
			item = "ODRF";
			item2 = "DRF1";
			break;
		}
		list.Add(item);
		list.Add(item2);
		return list;
	}
}
