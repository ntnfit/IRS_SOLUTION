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

public class InventoryTranferRespository : BaseService<DataSet>, IInventoryTranfer
{
	private const string TempDocument = "#APZ_DOC";

	private const string TempDocumentLine = "#APZ_DOCLINE";

	private const string TempBatchDetail = "#APZ_Batch_Detail";

	private const string TempDocumentBatch = "#APZ_Doc_Batch";

	public InventoryTranferRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public int GetMaxDocEntry()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(DocEntry ),0) as MaxDocEntry\r\n                            FROM OWTR ";
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
			string query = "SELECT ISNULL( MAX(DocNum),0) as MaxDocNum\r\n                            FROM OWTR where DocType = @type ";
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
			string text = "DECLARE @UDF NVARCHAR(MAX) DECLARE @sql nvarchar(max) SET @UDF=(SELECT 'A.'+COLUMN_NAME+',' FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'OWTR' AND LEFT(COLUMN_NAME,2) ='U_' for xml path('')) set @sql = 'SELECT ' + @UDF +'ISNULL  (a.DocEntry,0) as DocEntry, ISNULL  (a.DocNum,0) as DocNum,ISNULL( a.Series ,0) as Series,a.DocDate,a.UpdateDate,a.CreateDate,Isnull(a.CardCode,'''') as CardCode,ISNULL (a.CardName,'''') as CardName,ISNULL (a.CntctCode, 0) as ContactPersonCode,ISNULL (a.VatSum, 0) as VatSum,ISNULL (a.ShipToCode, '''') as ShipToCode,ISNULL (a.DocRate, 0) as DocRate,ISNULL (a.NumAtCard, '''') as NumAtCard,ISNULL (a.Rounding, ''N'') as Rounding,ISNULL (a.SlpCode, 0) as SlpCode,ISNULL (a.Requester, '''') as Requester,ISNULL (a.ReqName, '''') as ReqName,ISNULL (a.Branch, '''') as Branch,ISNULL (a.Department, '''') as Department,ISNULL (b.ListNum, -1) as ListNum,ISNULL (a.Canceled, ''N'') as Canceled ,ISNULL (a.ObjType, '''') as ObjType,a.TaxDate ,Isnull(a.DocTotal,0) as DocTotal,ISNULL(a.Ref2,'''') as Ref2,ISNULL(a.Filler,'''') as Filler,ISNULL(a.Comments,'''') as Comments,ISNULL(a.JrnlMemo,'''') as JrnlMemo,ISNULL(a.GroupNum,-1) as GroupNum,isnull(b.ListName,'''') as PriceList,isnull(a.DocCur,'''') as DocCurrency,isnull(a.DocStatus,'''') as DocStatus,ISNULL(c.SeriesName ,''Primary'') AS SeriesName,isnull(a.ToWhsCode,'''') as ToWhsCode,isnull(a.DocDueDate,getdate()) as DocDueDate,isnull(a.DiscPrcnt,0) as DiscountPercent FROM OWTR a left join OPLN  b on a.GroupNum=b.ListNum left join NNM1 c on a.Series=c.Series WHERE 1=1";
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
			return Process(ApzObjectTypes.oStockTransfer, -1, Enums.TransactionType.U, doc).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "save data ", ex.Message);
			return "Error save data into sql";
		}
	}

	public string CancelDoc(int docEntry)
	{
		try
		{
			return Process(ApzObjectTypes.oStockTransfer, -1, Enums.TransactionType.C, null, docEntry.ToString()).Message;
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
		case Enums.TransactionType.C:
		{
			DataSet dataSet = ExecuteData("apz_InventoryTransfer_cancel", CommandType.StoredProcedure, new SqlParameter("@obj_id", Function.ParseInt(objId)));
			if (!dataSet.IsNotNull() || dataSet.Tables.Count < 2)
			{
				return new HttpResult(Enums.MessageCode.Error, "Can not get data to close doc!");
			}
			Enums.TransactionType transtype2 = Enums.TransactionType.A;
			return AddNew(dataSet, objtype, ref objId, ref objNum, ref transtype2);
		}
		default:
			return new HttpResult();
		}
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
			dataTable.Rows[0]["DocDueDate"] = dataTable.Rows[0]["DocDate"];
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
		string query = string.Format("SELECT T1.DocEntry FROM {1} T0 INNER JOIN {0} T1 ON T0.BaseEntry=T1.DocEntry and T0.BaseType=T1.ObjType WHERE T1.DocEntry <>@DocEntry", tableLineName, "#APZ_DOCLINE");
		DataTable dataTable4 = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@DocEntry", Function.ParseInt(objId)));
		if (dataTable4.IsNotNull())
		{
			cancelEntry = Function.ParseInt(dataTable4.Rows[0]["DocEntry"]);
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
		DataTable dataTable3 = ExecuteDataTable("apz_InventoryTransfer_whs_journal", CommandType.StoredProcedure, parameters);
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
					string query2 = string.Format("delete {0} where DocEntry=@DocEntry\r\n                                                    insert into {0} select * from {1}", tableDocName, "#APZ_DOC");
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
				FileAttachmentProcess(doc, objtype, Function.ParseInt(objId));
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

	private void FileAttachmentProcess(DataSet ds, ApzObjectTypes objType, int docEntry)
	{
		if (ds.Tables.Contains("ATTFILE"))
		{
			DataTable dataTable = ds.Tables["ATTFILE"];
			int absEntry = GetAbsEntry(objType, docEntry);
			string text = GetUnitOfWork().Commons.FileAttachmentProcess(dataTable, absEntry);
			if (text.StartsWith("ERR:"))
			{
				throw new Exception(text.Remove(0, 4));
			}
			int num = Function.ParseInt(text);
			if (num <= 0 && dataTable.Rows.Count > 0)
			{
				throw new Exception("1001 - Save file Error (AbsEntry return must greater than zero)");
			}
			UpdateDocumentFileAttachment(objType, num, docEntry);
		}
	}

	private void UpdateDocumentFileAttachment(ApzObjectTypes objType, int absEntry, int docEntry)
	{
		string tableByApzObjectType = Function.GetTableByApzObjectType(objType);
		string query = "Update " + tableByApzObjectType + " SET AtcEntry=@AbsEntry WHERE docEntry=@DocEntry";
		ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@AbsEntry", Function.ParseInt(absEntry)), new SqlParameter("@DocEntry", Function.ParseInt(docEntry)));
	}

	private int GetAbsEntry(ApzObjectTypes objType, int docEntry)
	{
		string tableByApzObjectType = Function.GetTableByApzObjectType(objType);
		string query = "declare @AbsEntry int = isnull((SELECT isnull(AtcEntry,0) FROM " + tableByApzObjectType + "  WHERE  DocEntry= @DocEntry),0)\r\n                                select @AbsEntry";
		DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@DocEntry", Function.ParseInt(docEntry)));
		if (dataTable.IsNotNull())
		{
			return Function.ParseInt(dataTable.Rows[0][0]);
		}
		return 0;
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
			string query = "delete from WTR1 where DocEntry = '" + docEntry + "' and LineNum = '" + lineNum + "'";
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

	public DataTable SearchLine(Document_LinesImpl line)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "DECLARE @UDF NVARCHAR(MAX) \r\n                DECLARE @sql nvarchar(max) \r\n\r\n                SET @UDF=(SELECT 'A.'+COLUMN_NAME+',' \r\n                FROM INFORMATION_SCHEMA.COLUMNS\r\n                WHERE TABLE_NAME = N'WTR1' AND LEFT(COLUMN_NAME,2) ='U_' for xml path('')) \r\n\r\n                set @sql = 'SELECT distinct ' + @UDF +'\r\n                A.CodeBars,A.ItemCode ,ISNULL (A.Dscription,'''') as Dscription, ISNULL(A.Quantity,0) as Quantity,ISNULL(A.OpenQty,0) as OpenQty,ISNULL(A.LineTotal,0 ) as LineTotal, ISNULL (a.PriceBefDi , 0) as PriceBefDi,\r\n                ISNULL(A.Price,0) as Price,ISNULL (a.PQTReqdate, '''') as PQTReqdate,ISNULL (a.DiscPrcnt, 0) as DiscountPercent, ISNULL (a.FreeTxt, '''') as Text,ISNULL (a.OpenQty, 0) as RemainingOpenQuantity,\r\n                ISNULL (a.Currency, '''') as Currency,ISNULL (a.Rate, 0) as RateCurrency,ISNULL (a.LineVat, 0) as LineVat,ISNULL (a.BaseType, -1) as BaseType, ISNULL (a.BaseOpnQty,0) as BaseOpenQuantity,\r\n                ISNULL (a.BaseEntry, '''') as BaseEntry,ISNULL (a.BaseLine, '''') as BaseLine,ISNULL (a.AcctCode, '''') as AcctCode,ISNULL (a.ShipDate, '''') as ShipDate,A.VatPrcnt,\r\n                A.VatGroup,ISNULL(A.Factor1,0) as Factor1, ISNULL(A.Factor2,0) as Factor2,ISNULL(A.Factor3,0) as Factor3,ISNULL(A.Factor4,0) as Factor4,ISNULL(A.UseBaseUn,'''') as UseBaseUn,\r\n                ISNULL(A.unitMsr,'''') as unitMsr,ISNULL( A.NumPerMsr,0) as NumPerMsr,ISNULL( A.DocEntry,0) as DocEntry,ISNULL(A.LineNum,0) as LineNum,ISNULL(A.LineStatus,''O'') as LineStatus,\r\n                ISNULL(A.WhsCode,'''') as WhsCode,ISNULL(b.OnHand,0) as QuantityOnStock,ISNULL(a.unitMsr,'''') as UoMName,ISNULL (a.UoMCode, ''Manual'') as UoMCode,\r\n                ISNULL(a.FromWhsCod,'''') as FromWhsCod into #term \r\n                FROM WTR1 A left join oitw c on a.ItemCode=c.ItemCode  \r\n                left join (select itemcode,sum(onhand) as onhand from oitw group by itemcode) b on b.ItemCode=a.ItemCode left join oitm d on d.ItemCode=a.ItemCode \r\n                WHERE 1= 1 AND A.DocEntry = " + line.DocEntry + "\r\n\r\n                select * from #term  \r\n                Drop table #term ' \r\n                exec(@sql)";
			result = ExecuteData(query, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Line ", ex.Message);
		}
		return result;
	}

	public string SaveInventoryTransferRequestDetail(string docs)
	{
		try
		{
			string[] array = docs.Split('-');
			string query = "UPDATE wtq1  SET\r\n                                linestatus= 'C' where docentry=" + array[0] + " and linenum in (" + array[1] + ")";
			if (ExecuteNonQueryNoParam(query) > 0)
			{
				if (IsInventoryTransferRequestClosed(array[0]))
				{
					query = "UPDATE Owtq  SET docstatus= 'C' where docentry=" + array[0];
					ExecuteNonQueryNoParam(query);
					_ = 0;
					return null;
				}
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public bool IsInventoryTransferRequestClosed(string docEntry)
	{
		try
		{
			string query = "select docentry from trr1 where docentry=" + docEntry + " and linestatus= 'O'";
			if (ExecuteDataTable(query, CommandType.Text).Rows.Count > 0)
			{
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return true;
		}
	}

	public string CloseDoc(int docEntry)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				string query = "update owtr set docstatus='C' where docentry=" + docEntry;
				if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
				{
					query = "update wtr1 set linestatus='C' where docentry=" + docEntry;
					if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) >= 0)
					{
						dbTransaction.Commit();
						return null;
					}
					dbTransaction.Rollback();
					return "No row closed";
				}
				return "No row closed";
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
				dbTransaction.Rollback();
				return ex.Message;
			}
		}
	}

	public DataSet GetDataCopyFrom(string lstDocEntry, ApzObjectTypes fromObj)
	{
		try
		{
			DataSet dataSet = ExecuteData("APZ_InventoryTranfer_GetDataCopyFrom", CommandType.StoredProcedure, new SqlParameter("@docEntry", lstDocEntry), new SqlParameter("@fromObj", (int)fromObj));
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

	public DataTable GetDataDoc(string cardCode, int docNum, int docEntry, string docStatus, int prilist, string fromDate, string toDate, Enums.NavigateType type)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@cardCode", cardCode),
				new SqlParameter("@docNum", docNum),
				new SqlParameter("@docENtry", docEntry),
				new SqlParameter("@docStatus", docStatus),
				new SqlParameter("@priceList", prilist),
				new SqlParameter("@fromDate", Function.ParseDateTime(fromDate, "ddMMyyyy")),
				new SqlParameter("@toDate", Function.ParseDateTime(toDate, "ddMMyyyy")),
				new SqlParameter("@type", (int)type)
			};
			DataTable dataTable = ExecuteDataTable("APZ_InventoryTranfer_GetDataDoc", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				List<string> tableName = Function.GetTableName(ApzObjectTypes.oStockTransfer);
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

	public DataSet GetDataLine(int docEntry)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@docEntry", docEntry);
			DataSet dataSet = ExecuteData("APZ_InventoryTransfer_GetDataLine", CommandType.StoredProcedure, sqlParameter);
			if (dataSet != null && dataSet.Tables.Count >= 1)
			{
				List<string> tableName = Function.GetTableName(ApzObjectTypes.oStockTransfer);
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

	public int GetDocNumByDocEntry(int docEntry)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select DocNum from OWTR where DocEntry = @docEntry", CommandType.Text, new SqlParameter("@docEntry", docEntry));
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return 0;
			}
			return Function.ParseInt(dataTable.Rows[0][0]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public DataSet GetItemByBarcode(string barcode, string type)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@barcode", barcode),
				new SqlParameter("@type", type)
			};
			return ExecuteData("Apz_InventoryTransfer_GetItemByBarcode", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetWhsByBin(string binCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@binCode", binCode);
			return ExecuteDataTable("Apz_InventoryTransfer_GetWhsByBin", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string CreatePalletNo(int binId)
	{
		try
		{
			if (ExecuteNonQuery("Apz_InventoryTransfer_CreatePalletNo", CommandType.StoredProcedure, new SqlParameter("@binId", binId)) <= 0)
			{
				return "Lỗi khi tạo pallet ở bin: " + binId;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual HttpResult AddTransferMobileAppLog(DataTable dtTransfer, int transferEntry)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				new HttpResult();
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dtTransfer, "#tempTransferLog");
				BulkCopy(dtTransfer, "#tempTransferLog");
				string query = $"INSERT INTO APZ_TransferLog (\"DocEntry\", \"TempKey\", \"BoxCode\", \"DocTransfer\", \"Quantity\")\r\n                                                        select \"DocEntry\", \"TempKey\", \"BoxCode\", @docEntry, \"Quantity\" from \"#tempTransferLog\";";
				ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@docEntry", transferEntry));
				dbTransaction.Commit();
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
