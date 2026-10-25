using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Services.Interfaces.Production;

namespace Apzon.Services.Repository.Production;

public class ProductionOderRepository : BaseService<DataSet>, IProductionOder
{
	private const string TableDoc = "OWOR";

	private const string TableLine = "WOR1";

	private const string TempTableDoc = "#APZ_DOC";

	private const string TempTableLine = "#APZ_DOCLINE";

	private const string TempBatchDetail = "#APZ_Batch_Detail";

	private const string TempDocumentBatch = "#APZ_Doc_Batch";

	private const ApzObjectTypes FormObjectTypes = ApzObjectTypes.oProductionOrders;

	public ProductionOderRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable ShowOWOR()
	{
		throw new NotImplementedException();
	}

	public DataTable ShowWOR1()
	{
		throw new NotImplementedException();
	}

	public DataSet SearchLine(ProductionOder_LineImpl_WOR1 line)
	{
		try
		{
			DataSet dataSet = ExecuteData("Apz_ProductionOrder_SearchLine", CommandType.StoredProcedure, new SqlParameter("@DocEntry", line.DocEntry));
			SetDefaultValueToTable("OWOR", dataSet.Tables[0]);
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string GetTypeByDocEntry(int docEntry)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Apz_ProductionOrder_GetTypeByDocEntry", CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry));
			return dataTable.IsNotNull() ? Function.ToString(dataTable.Rows[0][0]) : "S";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "S";
		}
	}

	public DataTable GetTypeByDocEntryDataTable(string docEntry)
	{
		try
		{
			return ExecuteDataTable("Apz_ProductionOrder_GetTypeByDocEntryDataTable", CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataItemProduction(string tranType)
	{
		try
		{
			return ExecuteDataTable("Apz_ProductionOrder_GetListItemProductionOrders", CommandType.StoredProcedure, new SqlParameter("@tranType", tranType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataItemManualProduction(string docEntry)
	{
		try
		{
			return ExecuteDataTable("Apz_ProductionOrder_GetDataItemManualProduction", CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetListStandardOrders()
	{
		try
		{
			return ExecuteDataTable("Apz_ProductionOrder_GetListStandardOrders", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetMaxDocEntry()
	{
		try
		{
			string maxDocEntryQuery = GetMaxDocEntryQuery();
			return ExecuteDataTable(maxDocEntryQuery, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	protected virtual string GetMaxDocEntryQuery()
	{
		return "SELECT MAX(DocEntry)+1 DocEntry, MAX(DocNum)+1 DocNum FROM OWOR";
	}

	public DataTable GetItem_ITT1(BillOfMasterials_LineImpl_ITT1 line)
	{
		try
		{
			return ExecuteDataTable("Apz_ProductionOrder_GetItem_ITT1", CommandType.StoredProcedure, new SqlParameter("@Father", line.Father));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetList_SalesOder()
	{
		try
		{
			return ExecuteDataTable("Apz_ProductionOrder_GetList_SalesOder", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetItemDetails(string type, string code)
	{
		try
		{
			return ExecuteDataTable("Apz_ProductionOrder_GetItemDetails", CommandType.StoredProcedure, new SqlParameter[2]
			{
				new SqlParameter("@Type", type),
				new SqlParameter("@Code", code)
			}.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable Search(DataTable source, DataTable dt2)
	{
		try
		{
			List<string> list = new List<string>();
			List<SqlParameter> list2 = new List<SqlParameter>();
			if (source != null && source.Rows.Count > 0)
			{
				DataRow dataRow = source.Rows[0];
				for (int i = 0; i < source.Columns.Count; i++)
				{
					object obj = dataRow[source.Columns[i].ColumnName];
					string columnName = source.Columns[i].ColumnName;
					if (obj == null || string.IsNullOrEmpty(Function.ToString(obj)))
					{
						continue;
					}
					if (source.Columns[i].DataType == typeof(DateTime))
					{
						if (Function.ParseDateTime(obj).HasValue && !((DateTime)obj).Equals(new DateTime(1900, 1, 1)))
						{
							list.Add(string.Format("a.\"{0}\" >= @{0}", columnName));
							list2.Add(new SqlParameter($"@{columnName}", obj));
						}
					}
					else
					{
						list.Add(string.Format(SearchQuery(), columnName));
						list2.Add(new SqlParameter($"@{columnName}", obj));
					}
				}
				string text = "SELECT a.*, \r\n            CASE WHEN a.\"Type\" = 'S' THEN 'Standard' \r\n            WHEN a.\"Type\" = 'P' THEN 'Special'\r\n            WHEN a.\"Type\" = 'D' THEN 'Disassembly' END AS \"TypeName\", b.\"ItemName\", \r\n            CASE WHEN a.\"Status\" = 'P' THEN 'Planned'\r\n            WHEN a.\"Status\" = 'R' THEN 'Released'\r\n            WHEN a.\"Status\" = 'L' THEN 'Closed'\r\n            WHEN a.\"Status\" = 'C' THEN 'Canceled' ELSE '' END AS \"StatusName\"\r\n            FROM OWOR a LEFT OUTER JOIN OITM b ON a.\"ItemCode\" = b.\"ItemCode\" ";
				if (list.Count > 0)
				{
					text = text + " WHERE " + string.Join(" AND ", list.ToArray());
				}
				text += "; ";
				return ExecuteDataTable(text, CommandType.Text, list2.ToArray());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string SearchQuery()
	{
		return "a.\"{0}\" LIKE REPLACE(@{0}, '*', '%') ";
	}

	public DataTable GetADoc(int docEntry)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("apz_ProductionOrder_GetADoc", CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry));
			SetDefaultValueToTable("WOR1", dataTable);
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetSearchTableLine()
	{
		return GetUnitOfWork().Commons.GetSearchTable("WOR1");
	}

	public DataTable GetSearchTableDoc()
	{
		return GetUnitOfWork().Commons.GetSearchTable("OWOR");
	}

	public DataTable ShowUsers()
	{
		DataTable result = new DataTable();
		try
		{
			string query = ShowUsersQuery();
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string ShowUsersQuery()
	{
		return "SELECT USERID,U_NAME,USER_CODE FROM OUSR WHERE Locked='N'";
	}

	public DataTable GetIsIssued(int docEntry)
	{
		try
		{
			return ExecuteDataTable("apz_ProductionOrder_GetIsIssued", CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetOITWQuantity(string code, string whsCode)
	{
		DataTable result = new DataTable();
		try
		{
			string oITWQuantityQuery = GetOITWQuantityQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@code", code));
			list.Add(new SqlParameter("@whsCode", whsCode));
			result = ExecuteDataTable(oITWQuantityQuery, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string GetOITWQuantityQuery()
	{
		return "select \r\n\t                                     IsCommited   \r\n\t                                    ,OnOrder    \r\n                                        ,OnHand\r\n\t                                    ,(onhand + OnOrder - IsCommited) Available\r\n                                    from OITW  ItemCode=@code and WhsCode=@WhsCode";
	}

	public DataTable GetItemNameUoM(string itemCode)
	{
		try
		{
			string itemNameUoMQuery = GetItemNameUoMQuery();
			return ExecuteDataTable(itemNameUoMQuery, CommandType.Text, new SqlParameter("@itemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetItemNameUoMQuery()
	{
		return "SELECT T1.ItemName,T1.InvntryUom,T1.DfltWH WhsCode \r\n                FROM OITT T0 INNER JOIN OITM T1 ON T0.Code=T1.ItemCode AND T0.Code=@ItemCode";
	}

	public DataTable ValidateDocData(DataTable source)
	{
		DataTable dataTable = new DataTable();
		try
		{
			dataTable.Columns.Add("SONum");
			dataTable.Columns.Add("CardCode");
			dataTable.Columns.Add("DistrRule");
			dataTable.Columns.Add("Project");
			string query = ValidateDocDataQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@SONum", Function.ParseInt(source.Rows[0]["SONum"])));
			list.Add(new SqlParameter("@CardCode", Function.ToString(source.Rows[0]["CardCode"])));
			list.Add(new SqlParameter("@Project", Function.ToString(source.Rows[0]["Project"])));
			DataSet dataSet = ExecuteData(query, CommandType.Text, list.ToArray());
			DataRow dataRow = dataTable.NewRow();
			dataRow["SONum"] = (((dataSet.Tables[0] == null || dataSet.Tables[0].Rows.Count == 0) && !string.IsNullOrWhiteSpace(Function.ToString(source.Rows[0]["SONum"]))) ? "N" : "Y");
			dataRow["CardCode"] = (((dataSet.Tables[1] == null || dataSet.Tables[1].Rows.Count == 0) && !string.IsNullOrWhiteSpace(Function.ToString(source.Rows[0]["CardCode"]))) ? "N" : "Y");
			dataRow["Project"] = (((dataSet.Tables[2] == null || dataSet.Tables[2].Rows.Count == 0) && !string.IsNullOrWhiteSpace(Function.ToString(source.Rows[0]["Project"]))) ? "N" : "Y");
			dataRow["DistrRule"] = CheckDistRules(Function.ToString(source.Rows[0]["DistrRule"]));
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return dataTable;
	}

	protected virtual string ValidateDocDataQuery()
	{
		return "select DocEntry from ORDR where DocNum=@SONum;\r\n                            select cardCode from OCRD where cardcode=@CardCode --and ((validFor='N') OR (validFor='Y' And Getdate() between validFrom and validTo));\r\n                            select prjCode from oprj where prjcode=@Project and locked='N';";
	}

	private string CheckDistRules(string p)
	{
		string result = "Y";
		try
		{
			string query = "APZ_ProductionOrder_CheckDistRules";
			List<SqlParameter> list = new List<SqlParameter>();
			string[] array = p.Split(';');
			list.Add(new SqlParameter("@OCode", (array.Length != 0) ? Function.ToString(array[0]) : null));
			list.Add(new SqlParameter("@OCode2", (array.Length > 1) ? Function.ToString(array[1]) : null));
			list.Add(new SqlParameter("@OCode3", (array.Length > 2) ? Function.ToString(array[2]) : null));
			list.Add(new SqlParameter("@OCode4", (array.Length > 3) ? Function.ToString(array[3]) : null));
			list.Add(new SqlParameter("@OCode5", (array.Length > 4) ? Function.ToString(array[4]) : null));
			DataSet dataSet = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
			if (dataSet != null)
			{
				for (int i = 0; i < dataSet.Tables.Count; i++)
				{
					if (dataSet.Tables[i] == null || dataSet.Tables[i].Rows.Count == 0)
					{
						result = "N";
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public bool Exists(int docEntry)
	{
		try
		{
			string query = ExistsQuery();
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@DocEntry", docEntry));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	protected virtual string ExistsQuery()
	{
		return "SELECT 1 FROM OWOR WHERE DocEntry=@DocEntry";
	}

	public bool CheckLine(int docEntry, int lineNum)
	{
		try
		{
			string query = CheckLineQuery();
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@DocEntry", docEntry),
				new SqlParameter("@LineNum", lineNum)
			};
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	protected virtual string CheckLineQuery()
	{
		return "SELECT 1 FROM WOR1 WHERE DocEntry=@DocEntry and LineNum=@LineNum";
	}

	public DataTable Check_FirstRecord()
	{
		try
		{
			return ExecuteDataTable("Apz_ProductionOrder_Check_FirstRecord", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable Check_LastRecord()
	{
		try
		{
			return ExecuteDataTable("Apz_ProductionOrder_Check_LastRecord", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public HttpResult CancelDoc(int docEntry)
	{
		try
		{
			return Process(ApzObjectTypes.oProductionOrders, -1, Enums.TransactionType.C, new DataSet(), docEntry.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetRecord(string position, string docEntry)
	{
		try
		{
			string text = "";
			List<SqlParameter> list = new List<SqlParameter>();
			switch (position)
			{
			case "F":
				text = GetRecordFQuery();
				break;
			case "P":
				text += GetRecordPQuery();
				list.Add(new SqlParameter("@DocEntry", docEntry));
				break;
			case "N":
				text += GetRecordNQuery();
				list.Add(new SqlParameter("@DocEntry", docEntry));
				break;
			case "L":
				text = GetRecordLQuery();
				break;
			}
			if (list.Count > 0)
			{
				return ExecuteDataTable(text, CommandType.Text, list.ToArray());
			}
			return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetRecordFQuery()
	{
		return "select top 1 RN = ROW_NUMBER() OVER (ORDER BY [DocEntry]), DocEntry,DocNum,Series FROM OWOR";
	}

	protected virtual string GetRecordPQuery()
	{
		return "With CTE as (\r\n                                select RN = ROW_NUMBER() OVER (ORDER BY [DocEntry]), * FROM [OWOR])\r\n                                SELECT [Previous Row].* FROM CTE [Current Row]\r\n                                LEFT JOIN CTE [Previous Row] ON \r\n                                [Previous Row].RN = [Current Row].RN - 1\r\n                                WHERE [Current Row].[DocEntry] = @DocEntry";
	}

	protected virtual string GetRecordNQuery()
	{
		return "With CTE as ( select RN = ROW_NUMBER() OVER (ORDER BY [DocEntry]), DocEntry,DocNum,Series FROM OWOR)\r\n                                SELECT [Next Row].* FROM CTE [Current Row]\r\n                                LEFT JOIN CTE [Next Row] ON \r\n                                [Next Row].RN = [Current Row].RN + 1\r\n                                WHERE [Current Row].[DocEntry] = @DocEntry";
	}

	protected virtual string GetRecordLQuery()
	{
		return "select DocEntry,DocNum,Series from OWOR where DocEntry=(select max(DocEntry) from OWOR)";
	}

	public DataTable GetFirstRecord()
	{
		return GetUnitOfWork().Commons.GetFirstRecord("OWOR", "DocEntry");
	}

	public DataTable GetLastRecord()
	{
		return GetUnitOfWork().Commons.GetLastRecord("OWOR", "DocEntry");
	}

	public DataTable GetNextRecord(string docEntry)
	{
		return GetUnitOfWork().Commons.GetNextRecord("OWOR", "DocEntry", docEntry);
	}

	public DataTable GetPreviousRecord(string docEntry)
	{
		return GetUnitOfWork().Commons.GetPreviousRecord("OWOR", "DocEntry", docEntry);
	}

	public DataTable FindDataNextLast(int id, int type)
	{
		try
		{
			string query = "apz_ProductionOrder_FristNextLast";
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

	public DataTable GetSalesOrderEntry(int docNum)
	{
		try
		{
			string salesOrderEntryQuery = GetSalesOrderEntryQuery();
			return ExecuteDataTable(salesOrderEntryQuery, CommandType.Text, new SqlParameter("@DocNum", docNum));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetSalesOrderEntryQuery()
	{
		return "Select DocEntry From ORDR where docNum=@DocNum";
	}

	public HttpResult SaveDataSet(DataSet ds)
	{
		try
		{
			if (ds.Tables.Count < 2)
			{
				return new HttpResult(Enums.MessageCode.Error, "Not enough data");
			}
			return Process(ApzObjectTypes.oProductionOrders, -1, Enums.TransactionType.U, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			if (Exists(Function.ParseInt(document.Tables[0].Rows[0]["DocEntry"])))
			{
				transtype = Enums.TransactionType.U;
				return UpdateData(document, ref objId, ref objNum, ref transtype);
			}
			return AddNew(document, ref objId, ref objNum, ref transtype);
		case Enums.TransactionType.C:
		{
			DataTable dataTable = ExecuteDataTable("apz_productionorder_canceldoc", CommandType.StoredProcedure, new SqlParameter("@entry", Function.ParseInt(objId)));
			if (dataTable.IsNotNull())
			{
				int num = Function.ParseInt(dataTable.Rows[0][0]);
				if (num > 0)
				{
					string text = Function.ToString(dataTable.Rows[0][1]);
					throw new Exception(num + "-" + text);
				}
			}
			break;
		}
		}
		BatchAndSerialNumberProcess(document, objId, ApzObjectTypes.oProductionOrders, transtype, objNum);
		return new HttpResult();
	}

	private HttpResult AddNew(DataSet doc, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		if (doc.IsNotNull())
		{
			transtype = Enums.TransactionType.A;
			int series = -1;
			InitData(doc, ref objId, ref objNum, ref series, transtype);
			BeforeAction(doc, objId, objNum, transtype);
			string query = string.Format(AddNewQuery(), "#APZ_DOC", "#APZ_DOCLINE");
			ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@DocEntry", Function.ParseInt(objId)), new SqlParameter("@Series", series), new SqlParameter("@DocNum", Function.ParseInt(objNum)));
			AfterAction(doc, objId, objNum, transtype);
			BatchAndSerialNumberProcess(doc, objId, ApzObjectTypes.oProductionOrders, transtype, objNum);
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	protected virtual string AddNewQuery()
	{
		return "DELETE OWOR WHERE DocEntry=@DocEntry\r\n                                            DELETE WOR1 WHERE DocEntry=@DocEntry\r\n                                            INSERT INTO OWOR SELECT * FROM {0}\r\n                                            INSERT INTO WOR1 SELECT * FROM {1} WHERE {1}.ItemCode<>''\r\n                                            update onnm set AutoKey=ISNULL((SELECT MAX(DocEntry) FROM owor),0)+1 where ObjectCode='202'\r\n                                            update nnm1 set NextNumber=@DocNum+1 where ObjectCode='202' and Series=@Series";
	}

	private void AfterAction(DataSet doc, string objId, string objNum, Enums.TransactionType transtype)
	{
		DataTable dataTable = ExecuteDataTable("apz_document_itemlogmsg", CommandType.StoredProcedure, new SqlParameter("@obj_id", objId), new SqlParameter("@objtype", Function.ToString(202)), new SqlParameter("@transtype", transtype.ToString()));
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

	private void BeforeAction(DataSet doc, string objId, string objNum, Enums.TransactionType transtype)
	{
		DataTable dataTable = ExecuteDataTable("apz_productionorder_beforeaction", CommandType.StoredProcedure, new SqlParameter("@obj_id", objId), new SqlParameter("@transtype", transtype.ToString()));
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

	private void InitData(DataSet doc, ref string objId, ref string objNum, ref int series, Enums.TransactionType transtype)
	{
		DataTable dataTable = doc.Tables[0];
		DataTable tableStruct = GetTableStruct("OWOR");
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
			num = Function.ParseInt(GetObjectId(ApzObjectTypes.oProductionOrders));
			dataTable.Rows[0]["DocEntry"] = num;
			series = Function.ParseInt(dataTable.Rows[0]["Series"]);
			int usersign = Function.ParseInt(dataTable.Rows[0]["UserSign"]);
			num2 = Function.ParseInt(GetObjectNumBySeries(ApzObjectTypes.oProductionOrders, series, usersign));
			dataTable.Rows[0]["DocNum"] = num2;
			dataTable.Rows[0]["CreateDate"] = DateTime.Now.Date;
			break;
		}
		case Enums.TransactionType.U:
			num = Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			num2 = Function.ParseInt(dataTable.Rows[0]["DocNum"]);
			dataTable.Rows[0]["UpdateDate"] = DateTime.Now.Date;
			break;
		}
		CreateTempTable(dataTable, "#APZ_DOC");
		BulkCopy(dataTable, "#APZ_DOC");
		DataTable dataTable2 = doc.Tables[1];
		DataTable tableStruct2 = GetTableStruct("WOR1");
		if (!tableStruct2.IsNotNull())
		{
			throw new Exception("Could not get data line structure");
		}
		source = from DataColumn t in tableStruct2.Columns
			select t.ColumnName;
		dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source.ToArray());
		CreateTempTable(dataTable2, "#APZ_DOCLINE");
		BulkCopy(dataTable2, "#APZ_DOCLINE");
		objId = num.ToString();
		objNum = num2.ToString();
	}

	private HttpResult UpdateData(DataSet doc, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		if (doc.IsNotNull())
		{
			transtype = Enums.TransactionType.U;
			int series = -1;
			InitData(doc, ref objId, ref objNum, ref series, transtype);
			BeforeAction(doc, objId, objNum, transtype);
			string query = string.Format(UpdateDataQuery(), "#APZ_DOC", "#APZ_DOCLINE");
			ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@DocEntry", Function.ParseInt(objId)), new SqlParameter("@Series", series), new SqlParameter("@DocNum", Function.ParseInt(objNum)));
			AfterAction(doc, objId, objNum, transtype);
			BatchAndSerialNumberProcess(doc, objId, ApzObjectTypes.oProductionOrders, transtype, objNum);
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	protected virtual string UpdateDataQuery()
	{
		return "DELETE OWOR WHERE DocEntry=@DocEntry\r\n                                            DELETE WOR1 WHERE DocEntry=@DocEntry\r\n                                            INSERT INTO OWOR SELECT * FROM {0}\r\n                                            INSERT INTO WOR1 SELECT * FROM {1} WHERE {1}.ItemCode<>''\r\n                                            update onnm set AutoKey=@DocEntry+1 where ObjectCode='202'\r\n                                            update nnm1 set NextNumber=@DocNum+1 where ObjectCode='202' and Series=@Series";
	}

	public void BatchAndSerialNumberProcess(DataSet ds, string objId, ApzObjectTypes objType, Enums.TransactionType transtype, string objNum)
	{
		if (ds == null || ds.Tables.Count < 3 || ds.Tables[2] == null)
		{
			string query = string.Format(BatchAndSerialNumberProcessQuery(), "#APZ_Doc_Batch", "#APZ_Batch_Detail");
			ExecuteNonQuery(query, CommandType.Text);
		}
		else
		{
			DataTable dataTable = ds.Tables[2];
			DataTable tableStruct = GetTableStruct("OBTN");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data line structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			DataTable dt = dataTable.DefaultView.ToTable(false, "ItemCode", "DistNumber", "MnfSerial", "LotNumber", "BaseLineNumber", "LogQty", "ObjType", "AbsEntry", "SysNumber");
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, "#APZ_Batch_Detail");
			BulkCopy(dataTable, "#APZ_Batch_Detail");
			CreateTempTable(dt, "#APZ_Doc_Batch");
			BulkCopy(dt, "#APZ_Doc_Batch");
		}
		DataTable dataTable2 = ExecuteDataTable("APZ_Production_Batch_Process", CommandType.StoredProcedure, new SqlParameter("@obj_id", Function.ParseInt(objId)), new SqlParameter("@objtype", (int)objType), new SqlParameter("@transtype", transtype.ToString()), new SqlParameter("@objNum", Function.ParseInt(objNum)));
		if (dataTable2.IsNotNull())
		{
			int num = Function.ParseInt(dataTable2.Rows[0][0]);
			if (num > 0)
			{
				string text = Function.ToString(dataTable2.Rows[0][1]);
				throw new Exception(num + "-" + text);
			}
		}
	}

	protected virtual string BatchAndSerialNumberProcessQuery()
	{
		return "Select top 0 ItemCode, DistNumber, MnfSerial, LotNumber, 0 BaseLineNumber, 0 LogQty, ObjType, AbsEntry, SysNumber INTO {0}\r\n                                        from OBTN\r\n                                    Select top 0 * INTO {1} from OBTN ";
	}

	public int GetDocNum(int series)
	{
		try
		{
			return Function.ParseInt(GetObjectNumBySeries(ApzObjectTypes.oProductionOrders, series));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public DataTable GetByBaseEntry(int baseEntry, ApzObjectTypes type)
	{
		try
		{
			return ExecuteDataTable("APZ_Production_GetByBaseEntry", CommandType.StoredProcedure, new SqlParameter("@toType", (int)type), new SqlParameter("@DocEntry", baseEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetByBaseEntryQuery(ApzObjectTypes type)
	{
		return "SELECT T0.ItemCode,T0.DocNum,-1 BaseLine,T0.PlannedQty,T0.CmpltQty,T0.wareHouse,b.ItemName,ISNULL(T2.BPLId,-1) BplId, b.UgpEntry, b.InvntryUom,case when b.ManBtchNum='Y' then 'B' when b.ManSerNum='Y' then 'S' else 'N' END as ManBtchSerial ,\r\n            b.OnHand AS QuantityOnStock,\r\n\t\t\tc.OnHand as QuantityOnWareHouse, b.SalUnitMsr as SalUnitMsr, isnull(b.NumInSale,0) as NumInSale, b.BuyUnitMsr,Isnull(b.NumInBuy,0) NumInBuy,\r\n\t\t\tisnull(b.UgpEntry,-1) as UgpCode, b.InvntryUom as InvUnitMsr, isnull(d.AltQty, 1) / ISNULL(d.BaseQty, 1) as NumInInven,T0.wareHouse WhsCode,\r\n\t\t\tT0.Project,T0.OcrCode,T0.OcrCode2,T0.OcrCode3,T0.OcrCode4,T0.OcrCode5,c.AvgPrice\r\n                                FROM OWOR T0 \r\n                                     INNER JOIN OITM b on T0.ItemCode = b.ItemCode\r\n\t\t\t\t\t\t\t\t\tinner join OITW c on T0.ItemCode = c.ItemCode and T0.wareHouse = c.WhsCode\r\n\t\t\t\t\t\t\t\t\tleft join UGP1 d on b.UgpEntry = d.UgpEntry and d.UomEntry = b.IUoMEntry\r\n                                    INNER JOIN NNM1 T2 ON T0.Series=T2.Series AND T2.ObjectCode='202'\r\n                                  WHERE T0.DocEntry=@DocEntry" + type switch
		{
			ApzObjectTypes.oInventoryGenExit => " AND [Type] ='D'", 
			ApzObjectTypes.oInventoryGenEntry => " AND [Type] IN ('S','P')", 
			_ => "", 
		} + "UNION \r\n\r\n                SELECT T0.ItemCode,T1.DocNum,T0.LineNum,-T0.PlannedQty,-T0.IssuedQty,T0.wareHouse,b.ItemName,-1 , b.UgpEntry, b.InvntryUom,case when b.ManBtchNum='Y' then 'B' when b.ManSerNum='Y' then 'S' else 'N' END as ManBtchSerial ,\r\n                            b.OnHand AS QuantityOnStock,\r\n\t\t\t                c.OnHand as QuantityOnWareHouse, b.SalUnitMsr as SalUnitMsr, isnull(b.NumInSale,0) as NumInSale, b.BuyUnitMsr,Isnull(b.NumInBuy,0) NumInBuy,\r\n\t\t\t                isnull(b.UgpEntry,-1) as UgpCode, b.InvntryUom as InvUnitMsr, isnull(d.AltQty, 1) / ISNULL(d.BaseQty, 1) as NumInInven,T0.wareHouse WhsCode,\r\n\t\t\t                T0.Project,T0.OcrCode,T0.OcrCode2,T0.OcrCode3,T0.OcrCode4,T0.OcrCode5,c.AvgPrice\r\n\r\n                FROM dbo.WOR1 T0 \r\n                   INNER JOIN OWOR T1 ON T0.DocEntry=T1.DocEntry\r\n                   INNER JOIN OITM b on T0.ItemCode = b.ItemCode\r\n\t\t\t\t\t\t\t\t\tinner join OITW c on T0.ItemCode = c.ItemCode and T0.wareHouse = c.WhsCode\r\n\t\t\t\t\t\t\t\t\tleft join UGP1 d on b.UgpEntry = d.UgpEntry and d.UomEntry = b.IUoMEntry\r\n                                    WHERE T1.DocEntry=@DocEntry AND T0.IssueType='M' " + type switch
		{
			ApzObjectTypes.oInventoryGenEntry => " AND (T1.[Type] ='D' OR T0.PlannedQty < 0)", 
			ApzObjectTypes.oInventoryGenExit => " AND (T1.[Type] IN ('S','P') AND T0.PlannedQty > 0)", 
			_ => "", 
		};
	}

	public string UpdateStatus(ProductionOder_Impl_OWOR obj)
	{
		try
		{
			if (obj.DocEntry == 0)
			{
				return "No matching record found";
			}
			if (obj.Status == "C")
			{
				return CancelDoc(obj.DocEntry).Message;
			}
			DataSet byKey = GetByKey(obj.DocEntry);
			if (byKey.IsNotNull())
			{
				byKey.Tables[0].Rows[0]["Status"] = obj.Status;
				return Process(ApzObjectTypes.oProductionOrders, -1, Enums.TransactionType.U, byKey, obj.DocEntry.ToString()).Message;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "No matching record found";
	}

	public DataSet GetByKey(int docentry)
	{
		string byKeyQuery = GetByKeyQuery();
		return ExecuteData(byKeyQuery, CommandType.Text, new SqlParameter("@DocEntry", docentry));
	}

	public DataTable CheckByDocNum(string type, int docNum)
	{
		try
		{
			return ExecuteDataTable("SELECT T0.\"DocEntry\" FROM OWOR T0\r\n\t                                    WHERE T0.\"DocNum\" = @docNum AND T0.\"Status\" in ('R', 'P')\r\n\t                                    AND ((@Type='I' AND T0.\"Type\"='D') OR (@Type='R' AND T0.\"Type\" IN ('S','P')))\r\n\t\r\n\t                                    UNION ALL\r\n\t\r\n\t                                    SELECT T0.\"DocEntry\" FROM OWOR T0 INNER JOIN WOR1 T1 ON T0.\"DocEntry\" = T1.\"DocEntry\"\r\n\t                                    WHERE T0.\"DocNum\" = @docNum AND\r\n\t                                    T0.\"Status\" in ('R', 'P') AND T1.\"IssueType\" = 'M'\r\n\t                                    AND ((@Type = 'I' AND \"Type\" IN ('S','P') AND T1.\"BaseQty\" > 0)\r\n\t                                    OR (@Type = 'R' AND (\"Type\" = 'D'  OR T1.\"BaseQty\" < 0)));", CommandType.Text, new SqlParameter("@Type", type), new SqlParameter("@docNum", docNum));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetByKeyQuery()
	{
		return "SELECT * FROM OWOR WHERE DocEntry=@DocEntry SELECT * FROM WOR1 WHERE DocEntry=@DocEntry";
	}

	public DataSet GetProductionByBarcode(string barcode)
	{
		try
		{
			return ExecuteData("Apz_ProductionOrder_GetProductionByBarcode", CommandType.StoredProcedure, new SqlParameter("@barcode", barcode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetIssueComponents(int docEntry)
	{
		try
		{
			DataSet dataSet = ExecuteData("Apz_ProductionOrder_GetIssueComponents", CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry));
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].IsNotNull())
			{
				SetDefaultValueToTable("IGE1", dataSet.Tables[0]);
			}
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetTransferComponents(int docEntry, double parentQty)
	{
		try
		{
			DataSet dataSet = ExecuteData("Apz_ProductionOrder_GetTransferComponents", CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry), new SqlParameter("@ParentQty", parentQty));
			if (dataSet != null && dataSet.Tables.Count > 2 && dataSet.Tables[2].IsNotNull())
			{
				SetDefaultValueToTable("WTR1", dataSet.Tables[2]);
			}
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public HttpResult CheckBaseProduction(int entry, ApzObjectTypes objtype)
	{
		try
		{
			string text = "";
			string text2 = "";
			switch (objtype)
			{
			case ApzObjectTypes.oInventoryGenEntry:
				text = "OIGN";
				text2 = "IGN1";
				break;
			case ApzObjectTypes.oInventoryGenExit:
				text = "OIGE";
				text2 = "IGE1";
				break;
			}
			string query = "SELECT TOP 1 \"BaseRef\" FROM " + text2 + " T0 INNER JOIN " + text + " T1 ON T0.\"DocEntry\"=T1.\"DocEntry\" WHERE T0.\"BaseType\"=202 and T1.\"DocEntry\"=@DocEntry;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@DocEntry", entry));
			if (dataTable.IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.Success, "", Function.ParseInt(dataTable.Rows[0][0]));
			}
			return new HttpResult(Enums.MessageCode.Error, Messages.D404, 0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
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
			return new DataSetResult("", ExecuteData("Apz_ReceiptFromPO_GetDataByBarcode", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public DataTable GetReferencedDocument(string FromDocEntry)
	{
		try
		{
			return ExecuteDataTable("SELECT \"DocEntry\" As \"RefDocEntr\", \"DocNum\" As \"RefDocNum\", \"ObjType\" As \"RefObjType\", \"Comments\" As \"Remark\" \r\n                                        FROM OWOR WHERE \"DocEntry\" = @DocEntry;", CommandType.Text, new SqlParameter("@DocEntry", FromDocEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
