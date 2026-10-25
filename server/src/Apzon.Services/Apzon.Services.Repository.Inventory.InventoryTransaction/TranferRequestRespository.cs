using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Inventory.InventoryTransaction;

namespace Apzon.Services.Repository.Inventory.InventoryTransaction;

public class TranferRequestRespository : BaseService<DataSet>, ITranferRequest
{
	private const string TempDocument = "#APZ_DOC";

	private const string TempDocumentLine = "#APZ_DOCLINE";

	private const string TempBatchDetail = "#APZ_Batch_Detail";

	private const string TempDocumentBatch = "#APZ_Doc_Batch";

	public TranferRequestRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public int GetMaxDocEntry()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(DocEntry ),0) as MaxDocEntry\r\n                            FROM OWTQ ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
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

	public int GetNewDocNum()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(DocNum),0) as MaxDocNum\r\n                            FROM OWTQ where DocType = @type ";
			SqlParameter sqlParameter = new SqlParameter("@type", 'I');
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["MaxDocNum"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	public DataTable Search(DocumentsImpl doc)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "DECLARE @UDF NVARCHAR(MAX) DECLARE @sql nvarchar(max) SET @UDF=(SELECT 'A.'+COLUMN_NAME+',' FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'OWTR' AND LEFT(COLUMN_NAME,2) ='U_' for xml path('')) set @sql = 'SELECT ' + @UDF +'ISNULL  (a.DocEntry,0) as DocEntry, ISNULL  (a.DocNum,0) as DocNum,ISNULL( a.Series ,0) as Series,a.DocDate,a.UpdateDate,a.CreateDate,Isnull(a.CardCode,'''') as CardCode,ISNULL (a.CardName,'''') as CardName,ISNULL (a.CntctCode, 0) as ContactPersonCode,ISNULL (a.VatSum, 0) as VatSum,ISNULL (a.ShipToCode, '''') as ShipToCode,ISNULL (a.DocRate, 0) as DocRate,ISNULL (a.NumAtCard, '''') as NumAtCard,ISNULL (a.Rounding, ''N'') as Rounding,ISNULL (a.SlpCode, 0) as SlpCode,ISNULL (a.Requester, '''') as Requester,ISNULL (a.ReqName, '''') as ReqName,ISNULL (a.Branch, '''') as Branch,ISNULL (a.Department, '''') as Department,ISNULL (b.ListNum, -1) as ListNum,ISNULL (a.Canceled, ''N'') as Canceled ,ISNULL (a.ObjType, '''') as ObjType,a.TaxDate ,Isnull(a.DocTotal,0) as DocTotal,ISNULL(a.Ref2,'''') as Ref2,ISNULL(a.Filler,'''') as Filler,ISNULL(a.Comments,'''') as Comments,ISNULL(a.JrnlMemo,'''') as JrnlMemo,ISNULL(a.GroupNum,-1) as GroupNum,isnull(b.ListName,'''') as PriceList,isnull(a.DocCur,'''') as DocCurrency,isnull(a.DocStatus,'''') as DocStatus,ISNULL(c.SeriesName ,''Primary'') AS SeriesName,isnull(a.ToWhsCode,'''') as ToWhsCode,isnull(a.DocDueDate,getdate()) as DocDueDate,isnull(a.DiscPrcnt,0) as DiscountPercent FROM OWTQ a left join OPLN  b on a.GroupNum=b.ListNum left join NNM1 c on a.Series=c.Series WHERE 1=1";
			if (doc.DocEntry == 0)
			{
				if (doc.DocDate.Date.ToString("yyyy-MM-dd") != "1900-01-01")
				{
					text = text + " AND DocDate >=convert(datetime, ''" + doc.DocDate.Date.ToString("yyyy-MM-dd") + "'')";
				}
				if (doc.TaxDate.Date.ToString("yyyy-MM-dd") != "1900-01-01")
				{
					text = text + " AND DocDate <=convert(datetime, ''" + doc.TaxDate.Date.ToString("yyyy-MM-dd") + "'')";
				}
				if (!string.IsNullOrEmpty(doc.CardCode))
				{
					text = text + " AND CardCode like ''%" + doc.CardCode + "%'' ";
				}
				text = text + " AND DocStatus=''" + doc.DocStatus + "''";
			}
			else
			{
				text = text + "AND a.DocEntry = ''" + doc.DocEntry + "'' ";
			}
			text += " order by docnum desc' exec(@sql)";
			result = ExecuteData(text, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Return Search - Doc ", ex.Message);
		}
		return result;
	}

	public string SaveDoc(DataSet doc)
	{
		try
		{
			return Process(ApzObjectTypes.oInventoryTransferRequest, -1, Enums.TransactionType.U, doc).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet doc, ref string objId, ref string objNum)
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			if (Exists(doc, objtype))
			{
				transtype = Enums.TransactionType.U;
				return UpdateData(doc, objtype, ref objId, ref objNum, ref transtype);
			}
			transtype = Enums.TransactionType.A;
			return AddNew(doc, objtype, ref objId, ref objNum, ref transtype);
		case Enums.TransactionType.L:
		{
			DataTable dataTable = ExecuteDataTable("apz_TransferRequest_close", CommandType.StoredProcedure, new SqlParameter("@obj_id", Function.ParseInt(objId)));
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
		GetUnitOfWork().Documents.BatchAndSerialNumberProcess(doc, objId, objtype, transtype, objNum);
		return new HttpResult();
	}

	private HttpResult AddNew(DataSet doc, ApzObjectTypes objtype, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		if (doc.IsNotNull())
		{
			transtype = Enums.TransactionType.A;
			string tableDocName = "";
			string tableLineName = "";
			int series = -1;
			int cancelEntry = 0;
			InitData(doc, objtype, ref objId, ref objNum, ref tableDocName, ref tableLineName, ref series, transtype, ref cancelEntry);
			BeforeAction(doc, objtype, objId, objNum, transtype, cancelEntry);
			string query = string.Format("insert into {0} select * from {1} \r\n                                            update {2} set DocEntry=@DocEntry, LineStatus='O',ObjType=@ObjCode\r\n                                            insert into {3} select * from {2} where {2}.ItemCode is not null and {2}.ItemCode <>''\r\n                                            update onnm set AutoKey=@DocEntry+1 where ObjectCode=@ObjCode\r\n                                            update nnm1 set NextNumber=@DocNum+1 WHERE ObjectCode=@ObjCode AND ISNULL(Series,-1)=@Series", tableDocName, "#APZ_DOC", "#APZ_DOCLINE", tableLineName);
			ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@DocEntry", objId), new SqlParameter("@ObjCode", Function.ToString((int)objtype)), new SqlParameter("@DocNum", objNum), new SqlParameter("@Series", series));
			AfterAction(doc, objtype, objId, objNum, transtype, cancelEntry);
			BatchAndSerialNumberProcess(doc, objId, objtype, transtype, objNum);
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	public void BatchAndSerialNumberProcess(DataSet ds, string objId, ApzObjectTypes objType, Enums.TransactionType transtype, string objNum)
	{
		if (ds == null || ds.Tables.Count < 3 || ds.Tables[2] == null)
		{
			string query = string.Format("Select top 0 ItemCode, DistNumber, MnfSerial, LotNumber, 0 BaseLineNumber, 0 LogQty, ObjType, AbsEntry, SysNumber INTO {0}\r\n                                        from OBTN\r\n                                    Select top 0 * INTO {1} from OBTN ", "#APZ_Doc_Batch", "#APZ_Batch_Detail");
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
		DataTable dataTable2 = ExecuteDataTable("APZ_Document_Batch_Process", CommandType.StoredProcedure, new SqlParameter("@obj_id", Function.ParseInt(objId)), new SqlParameter("@objtype", (int)objType), new SqlParameter("@transtype", transtype.ToString()), new SqlParameter("@objNum", Function.ParseInt(objNum)));
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

	private void InitData(DataSet doc, ApzObjectTypes objtype, ref string objId, ref string objNum, ref string tableDocName, ref string tableLineName, ref int series, Enums.TransactionType transtype, ref int cancelEntry)
	{
		DataTable dataTable = doc.Tables[0];
		List<string> tableName = Function.GetTableName(objtype);
		tableDocName = tableName[0];
		DataTable tableStruct = GetTableStruct(tableDocName);
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
			num = Function.ParseInt(GetObjectId(objtype));
			dataTable.Rows[0]["DocEntry"] = num;
			dataTable.Rows[0]["Ref1"] = num;
			int num3 = Function.ParseInt(dataTable.Rows[0]["BPLId"]);
			num3 = ((num3 == 0) ? (-1) : num3);
			series = Function.ParseInt(dataTable.Rows[0]["Series"]);
			int usersign = Function.ParseInt(dataTable.Rows[0]["UserSign"]);
			num2 = Function.ParseInt(GetObjectNum(objtype, num3, series, usersign));
			dataTable.Rows[0]["DocNum"] = num2;
			dataTable.Rows[0]["CreateDate"] = DateTime.Now.Date;
			dataTable.Rows[0]["DocTime"] = DateTime.Now.ToString("hhmm");
			break;
		}
		case Enums.TransactionType.U:
			num = Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			num2 = Function.ParseInt(dataTable.Rows[0]["DocNum"]);
			dataTable.Rows[0]["UpdateDate"] = DateTime.Now.Date;
			break;
		}
		if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["TaxDate"])))
		{
			dataTable.Rows[0]["TaxDate"] = dataTable.Rows[0]["DocDate"];
		}
		CreateTempTable(dataTable, "#APZ_DOC");
		BulkCopy(dataTable, "#APZ_DOC");
		DataTable dataTable2 = doc.Tables[1];
		tableLineName = tableName[1];
		DataTable tableStruct2 = GetTableStruct(tableLineName);
		if (!tableStruct2.IsNotNull())
		{
			throw new Exception("Could not get data line structure");
		}
		source = from DataColumn t in tableStruct2.Columns
			select t.ColumnName;
		dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source.ToArray());
		CreateTempTable(dataTable2, "#APZ_DOCLINE");
		BulkCopy(dataTable2, "#APZ_DOCLINE");
		List<string> list = new List<string>();
		if (transtype == Enums.TransactionType.U)
		{
			DataTable dataTable3 = ExecuteDataTable($"SELECT * FROM {tableDocName} WHERE DocEntry=@DocEntry", CommandType.Text, new SqlParameter("@DocEntry", num));
			if (!dataTable3.IsNotNull())
			{
				throw new Exception($"No matching record found [{objtype} - {num}]");
			}
			for (int num4 = 0; num4 < list.Count; num4++)
			{
				if (!dataTable.Rows[0][list[num4]].Equals(dataTable3.Rows[0][list[num4]]))
				{
					throw new Exception($"Fields cannot be updated [{objtype} - {list[num4]}]");
				}
			}
		}
		objId = num.ToString();
		objNum = num2.ToString();
	}

	private void BeforeAction(DataSet doc, ApzObjectTypes objtype, string objId, string objNum, Enums.TransactionType transtype, int cancelEntry = 0)
	{
		string query = $"apz_document_validate";
		SqlParameter[] parameters = new SqlParameter[3]
		{
			new SqlParameter("@obj_id", objId),
			new SqlParameter("@objtype", Function.ToString((int)objtype)),
			new SqlParameter("@transtype", transtype.ToString())
		};
		DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, parameters);
		if (dataTable.IsNotNull())
		{
			int num = Function.ParseInt(dataTable.Rows[0][0]);
			if (num > 0)
			{
				string text = Function.ToString(dataTable.Rows[0][1]);
				throw new Exception(num + "-" + text);
			}
		}
		DataTable dataTable2 = ExecuteDataTable("apz_document_calculate", CommandType.StoredProcedure, parameters);
		if (dataTable2.IsNotNull())
		{
			int num2 = Function.ParseInt(dataTable2.Rows[0][0]);
			if (num2 > 0)
			{
				string text2 = Function.ToString(dataTable2.Rows[0][1]);
				throw new Exception(num2 + "-" + text2);
			}
		}
		if (cancelEntry != 0)
		{
			return;
		}
		DataTable dataTable3 = ExecuteDataTable("apz_InventoryTransfer_item_whs", CommandType.StoredProcedure, parameters);
		if (dataTable3.IsNotNull())
		{
			int num3 = Function.ParseInt(dataTable3.Rows[0][0]);
			if (num3 > 0)
			{
				string text3 = Function.ToString(dataTable3.Rows[0][1]);
				throw new Exception(num3 + "-" + text3);
			}
		}
	}

	private void AfterAction(DataSet doc, ApzObjectTypes objtype, string objId, string objNum, Enums.TransactionType transtype, int cancelEntry = 0)
	{
		SqlParameter[] parameters = new SqlParameter[3]
		{
			new SqlParameter("@obj_id", objId),
			new SqlParameter("@objtype", Function.ToString((int)objtype)),
			new SqlParameter("@transtype", transtype.ToString())
		};
		DataTable dataTable = ExecuteDataTable("apz_document_installments", CommandType.StoredProcedure, parameters);
		if (dataTable.IsNotNull())
		{
			int num = Function.ParseInt(dataTable.Rows[0][0]);
			if (num > 0)
			{
				string text = Function.ToString(dataTable.Rows[0][1]);
				throw new Exception(num + "-" + text);
			}
		}
		DataTable dataTable2 = ExecuteDataTable("apz_InventoryTransfer_itemlogmsg", CommandType.StoredProcedure, parameters);
		if (dataTable2.IsNotNull())
		{
			int num2 = Function.ParseInt(dataTable2.Rows[0][0]);
			if (num2 > 0)
			{
				string text2 = Function.ToString(dataTable2.Rows[0][1]);
				throw new Exception(num2 + "-" + text2);
			}
		}
	}

	private HttpResult UpdateData(DataSet doc, ApzObjectTypes objtype, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		try
		{
			if (doc.IsNotNull())
			{
				transtype = Enums.TransactionType.U;
				string tableDocName = "";
				string tableLineName = "";
				int series = -1;
				int cancelEntry = 0;
				InitData(doc, objtype, ref objId, ref objNum, ref tableDocName, ref tableLineName, ref series, transtype, ref cancelEntry);
				BeforeAction(doc, objtype, objId, objNum, transtype);
				switch (objtype)
				{
				case ApzObjectTypes.oOrders:
				case ApzObjectTypes.oQuotations:
				{
					string query2 = string.Format("delete {0} where DocEntry=@DocEntry\r\n                                                    delete {3} where DocEntry=@DocEntry\r\n                                                    insert into {0} select * from {1} \r\n                                                    insert into {3} select * from {2} where {2}.ItemCode is not null and {2}.ItemCode <>'' and {2}.Quantity>0", tableDocName, "#APZ_DOC", "#APZ_DOCLINE", tableLineName);
					ExecuteNonQuery(query2, CommandType.Text, new SqlParameter("@DocEntry", objId));
					break;
				}
				case ApzObjectTypes.oDeliveryNotes:
				{
					string query = string.Format("delete {0} where DocEntry=@DocEntry\r\n                                                    insert into {0} select * from {1}", tableDocName, "#APZ_DOC");
					ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@DocEntry", objId));
					break;
				}
				}
				AfterAction(doc, objtype, objId, objNum, transtype);
				return new HttpResult();
			}
			return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool Exists(DataSet doc, ApzObjectTypes type)
	{
		try
		{
			string text = Function.GetTableName(type)[0];
			string query = "SELECT 1 FROM " + text + " WHERE DocEntry=" + Function.ParseInt(doc.Tables[0].Rows[0]["DocEntry"]);
			if (ExecuteDataTable(query, CommandType.Text).IsNotNull())
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public string DeleteLine(int docEntry, int lineNum)
	{
		try
		{
			string query = "delete from WTQ1 where DocEntry = '" + docEntry + "' and LineNum = '" + lineNum + "'";
			if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public string CancelLine(string docentry, string lineNum, string quatity, string itemcode, string whscode, string towhscode, string linetotal)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				string text = "Update owtr set DocTotal = DocTotal - @linetotal where DocEntry = @docentry  ";
				text += "Update wtr1 set OpenQty=0,LineStatus='C' where DocEntry = @docentry and LineNum = @lineNum    ";
				text += "Update OITW set OnHand = OnHand + @quatity where ItemCode = @itemcode and WhsCode = @whscode   ";
				text += "Update OITW set OnHand = OnHand - @quatity where ItemCode = @itemcode and WhsCode = @towhscode   ";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@linetotal", Function.ParseInt(linetotal)));
				list.Add(new SqlParameter("@docentry", Function.ParseInt(docentry)));
				list.Add(new SqlParameter("@lineNum", Function.ParseInt(lineNum)));
				list.Add(new SqlParameter("@quatity", Function.ParseInt(quatity)));
				list.Add(new SqlParameter("@itemcode", itemcode));
				list.Add(new SqlParameter("@whscode", whscode));
				list.Add(new SqlParameter("@towhscode", towhscode));
				if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "error!";
			}
			catch (Exception)
			{
				dbTransaction.Dispose();
				return "Error!";
			}
		}
	}

	public string CloseDoc(int docEntry)
	{
		try
		{
			return Process(ApzObjectTypes.oInventoryTransferRequest, -1, Enums.TransactionType.L, null, docEntry.ToString()).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetDataDoc(string cardCode, int docNum, int docEntry, string docStatus, int prilist, string fromDate, string toDate, Enums.NavigateType type)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@cardCode", cardCode),
				new SqlParameter("@docNum", docNum),
				new SqlParameter("@docEntry", docEntry),
				new SqlParameter("@docStatus", docStatus),
				new SqlParameter("@priceList", prilist),
				new SqlParameter("@fromDate", Function.ParseDateTime(fromDate, "ddMMyyyy")),
				new SqlParameter("@toDate", Function.ParseDateTime(toDate, "ddMMyyyy")),
				new SqlParameter("@type", (int)type)
			};
			DataTable dataTable = ExecuteDataTable("APZ_InventoryTranferRequest_GetDataDoc", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				List<string> tableName = Function.GetTableName(ApzObjectTypes.oInventoryTransferRequest);
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

	public DataSet GetDataLine(int docEntry)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@docEntry", docEntry);
			DataSet dataSet = ExecuteData("APZ_InventoryTransferRequest_GetDataLine", CommandType.StoredProcedure, sqlParameter);
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				List<string> tableName = Function.GetTableName(ApzObjectTypes.oInventoryTransferRequest);
				SetDefaultValueToTable(tableName[1], dataSet.Tables[0]);
			}
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataCopyFrom(string docEntry, ApzObjectTypes fromObj)
	{
		try
		{
			DataSet dataSet = ExecuteData("APZ_TranferRequest_GetDataCopyFrom", CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry), new SqlParameter("@fromObj", (int)fromObj));
			if (dataSet.Tables.Count > 2)
			{
				if (dataSet.Tables[1] != null)
				{
					List<string> tableName = Function.GetTableName(ApzObjectTypes.oStockTransfer);
					SetDefaultValueToTable(tableName[0], dataSet.Tables[1]);
				}
				if (dataSet.Tables[2] != null)
				{
					List<string> tableName2 = Function.GetTableName(ApzObjectTypes.oStockTransfer);
					SetDefaultValueToTable(tableName2[1], dataSet.Tables[2]);
				}
			}
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetCopyLines(List<int> docEntries)
	{
		try
		{
			string text = "";
			foreach (int docEntry in docEntries)
			{
				text = text + docEntry + ",";
			}
			if (text.Length > 0)
			{
				text = text.Remove(text.Length - 1);
			}
			string query = "SELECT distinct  A.CodeBars,\r\n                                        A.ItemCode ,\r\n\t\t\t\t\t\t\t\t\t\tA.Dscription, \r\n\t\t                               ISNULL( A.Quantity,0) as  Quantity,\r\n\t\t\t\t\t\t\t\t\tISNULL(\tA.OpenQty,0) as OpenQty,\r\n\t\t                              ISNULL(  A.LineTotal,0 ) as LineTotal,\r\n\t\t\t\t\t\t\t\t\tISNULL(\tA.Price,0) as Price,\r\n\t\t\t\t\t\t\t\t\t\tA.VatPrcnt,\r\n\t\t\t\t\t\t\t\t\t\tA.VatGroup,\r\n\t\t\t\t\t\t\t\t\tISNULL(\tA.Factor1,0) as Factor1, ISNULL(A.Factor2,0) as Factor2,ISNULL(\tA.Factor3,0) as Factor3,ISNULL(\tA.Factor4,0) as Factor4\t,\t\t\t\t\t\t\t\t\r\n\t\t                               ISNULL( A.UseBaseUn,'') as UseBaseUn,\r\n\t\t\t\t\t\t\t\t\tISNULL( A.unitMsr,'') as unitMsr,\r\n\t\t\t\t\t\t\t\t\tISNULL( A.NumPerMsr,0) as NumPerMsr,\r\n\t\t\t\t\t\t\t\t\tISNULL( A.DocEntry,0) as DocEntry,\r\n\t\t\t\t\t\t\t\t\tISNULL( A.LineNum,0) as LineNum,\r\n\t\t\t\t\t\t\t\t\tISNULL( A.LineStatus,'O') as LineStatus,\r\n\t\t\t\t\t\t\t\t\tISNULL( A.WhsCode,'') as WhsCode,\r\n\t\t\t\t\t\t\t\t\tISNULL(b.OnHand,0)\tas QuantityOnStock,\r\n\t\t\t\t\t\t\t\t\tISNULL(a.unitMsr,'') as UoMName,\r\n\t\t\t\t\t\t\t\t\t'Manual' as UoMCode,\r\n\t\t\t\t\t\t\t\t\tISNULL(a.FromWhsCod,'') as FromWhsCod\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t                                                                                                                   \r\n                                FROM WTQ1 A\r\n\t\t\t\t\t\t\t\tleft join oitw c on a.ItemCode=c.ItemCode\r\n\t\t\t\t\t\t\t\tleft join (select itemcode,sum(onhand) as onhand from oitw group by itemcode) b on b.ItemCode=a.ItemCode\r\n\t\t\t\t\t\t\t\tleft join oitm d on d.ItemCode=a.ItemCode where a.docentry in (" + text + ")";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable SearchLine(Document_LinesImpl line)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "DECLARE @UDF NVARCHAR(MAX) DECLARE @sql nvarchar(max) SET @UDF=(SELECT 'A.'+COLUMN_NAME+',' FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'WTR1' AND LEFT(COLUMN_NAME,2) ='U_' for xml path('')) set @sql = 'SELECT distinct ' + @UDF +'A.CodeBars,A.ItemCode ,ISNULL (A.Dscription,'''') as ItemDescription, ISNULL(A.Quantity,0) as Quantity,ISNULL(A.OpenQty,0) as OpenQty,ISNULL(A.LineTotal,0 ) as LineTotal, ISNULL (a.PriceBefDi , 0) as PriceBefDi,ISNULL(A.Price,0) as Price,ISNULL (a.PQTReqdate, '''') as PQTReqdate,ISNULL (a.DiscPrcnt, 0) as DiscountPercent, ISNULL (a.FreeTxt, '''') as Text,ISNULL (a.OpenQty, 0) as RemainingOpenQuantity,ISNULL (a.Currency, '''') as Currency,ISNULL (a.Rate, 0) as RateCurrency,ISNULL (a.LineVat, 0) as LineVat,ISNULL (a.BaseType, -1) as BaseType, ISNULL (a.BaseOpnQty,0) as BaseOpenQuantity,ISNULL (a.BaseEntry, '''') as BaseEntry,ISNULL (a.BaseLine, '''') as BaseLine,ISNULL (a.AcctCode, '''') as AcctCode,ISNULL (a.ShipDate, '''') as ShipDate,A.VatPrcnt,A.VatGroup,ISNULL(A.Factor1,0) as Factor1, ISNULL(A.Factor2,0) as Factor2,ISNULL(A.Factor3,0) as Factor3,ISNULL(A.Factor4,0) as Factor4,ISNULL(A.UseBaseUn,'''') as UseBaseUn,ISNULL(A.unitMsr,'''') as unitMsr,ISNULL( A.NumPerMsr,0) as NumPerMsr,ISNULL( A.DocEntry,0) as DocEntry,ISNULL(A.LineNum,0) as LineNum,ISNULL(A.LineStatus,''O'') as LineStatus,ISNULL(A.WhsCode,'''') as WhsCode,ISNULL(b.OnHand,0) as QuantityOnStock,ISNULL(a.unitMsr,'''') as UoMName,ISNULL (a.UoMCode, ''Manual'') as UoMCode,ISNULL(a.FromWhsCod,'''') as FromWhsCod into #term FROM WTQ1 A left join oitw c on a.ItemCode=c.ItemCode  left join (select itemcode,sum(onhand) as onhand from oitw group by itemcode) b on b.ItemCode=a.ItemCode left join oitm d on d.ItemCode=a.ItemCode WHERE 1= 1 AND A.DocEntry = ''" + line.DocEntry + "'' select * from #term  Drop table #term ' exec(@sql)";
			result = ExecuteData(query, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Line ", ex.Message);
		}
		return result;
	}

	public DataTableResult GetDataByBarcode(string barcode, string type)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@barcode", barcode),
				new SqlParameter("@type", type)
			};
			return new DataTableResult("", ExecuteDataTable("Apz_TransferRequest_GetDataByBarcode", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTableResult(ex.Message);
		}
	}

	public void LogBinAllocate(DataTable dt)
	{
		try
		{
			BulkCopy(dt, "APZ_LOG_ALLOCATE_BIN");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}
}
