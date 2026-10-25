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
using Apzon.Entities.Models.DMS;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Sales;
using Apzon.Services.Repository.Commons;
using SAPbobsCOM;

namespace Apzon.Services.Repository.Document;

public class DocumentRepository : BaseService<DataSet>, IDocument, IService<DocumentsImpl>
{
	private const string TempDocument = "#APZ_DOC";

	private const string TempDocumentLine = "#APZ_DOCLINE";

	private const string TempBatchDetail = "#APZ_Batch_Detail";

	private const string TempDocumentBatch = "#APZ_Doc_Batch";

	private const string TempDocumentFile = "#APZ_Doc_File";

	public DocumentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDocuments(DocumentsImpl doc, string tbl, string tblLine)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@tbl", tbl));
			list.Add(new SqlParameter("@tblLine", tblLine));
			list.Add(new SqlParameter("@DocEntry", doc.DocEntry));
			if (doc.DocDate != DateTime.MinValue)
			{
				list.Add(new SqlParameter("@DocDate", doc.DocDate.Date));
			}
			else
			{
				list.Add(new SqlParameter("@DocDate", null));
			}
			if (doc.DocDueDate != DateTime.MinValue)
			{
				list.Add(new SqlParameter("@EndDate", doc.DocDueDate));
			}
			else
			{
				list.Add(new SqlParameter("@EndDate", null));
			}
			list.Add(new SqlParameter("@DocStatus", doc.DocStatus));
			if (doc.Cancelled == BoYesNoEnum.tYES)
			{
				list.Add(new SqlParameter("@Canceled", "Y"));
			}
			else
			{
				list.Add(new SqlParameter("@Canceled", "N"));
			}
			list.Add(new SqlParameter("@CardCode", doc.CardCode));
			list.Add(new SqlParameter("@UserName", doc.UserName));
			list.Add(new SqlParameter("@isIns", doc.isIns ?? "N"));
			if (doc.BPLId > 0 || string.IsNullOrEmpty(doc.BPLId.ToString()))
			{
				list.Add(new SqlParameter("@Branch", doc.BPLId));
			}
			else
			{
				list.Add(new SqlParameter("@Branch", -1));
			}
			list.Add(new SqlParameter("@DocNum", doc.DocNum));
			list.Add(new SqlParameter("@Requester", doc.Requester));
			if (doc.ReqType > 0)
			{
				list.Add(new SqlParameter("@ReqType", doc.ReqType));
			}
			else
			{
				list.Add(new SqlParameter("@ReqType", null));
			}
			DataTable dataTable = ExecuteDataTable("apz_document_get", tbl, CommandType.StoredProcedure, list.ToArray());
			if (!string.IsNullOrEmpty(doc.FilterString))
			{
				string filterText = GetUnitOfWork().FilterSetting.GetFilterText(doc.FilterString);
				DataRow[] array = dataTable.Select(filterText);
				if (array.Length != 0)
				{
					return array.CopyToDataTable();
				}
				return new DataTable();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search data table " + tbl, ex.Message);
			return new DataTable();
		}
	}

	public DataTable DMSGetDocuments(DMSSaleImpl data, string tbl, string tblLine)
	{
		try
		{
			DocumentsImpl document = data.Document;
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@tbl", tbl));
			list.Add(new SqlParameter("@tblLine", tblLine));
			list.Add(new SqlParameter("@DocEntry", document.DocEntry));
			list.Add(new SqlParameter("@pageIndex", data.PageIndex));
			list.Add(new SqlParameter("@searchText", data.SearchText));
			list.Add(new SqlParameter("@skipAuthorization", data.SkipAuthorization));
			list.Add(new SqlParameter("@DMSSubType", data.DMSSubType));
			if (document.DocDate != DateTime.MinValue)
			{
				list.Add(new SqlParameter("@DocDate", document.DocDate.Date));
			}
			else
			{
				list.Add(new SqlParameter("@DocDate", null));
			}
			if (document.DocDueDate != DateTime.MinValue)
			{
				list.Add(new SqlParameter("@EndDate", document.DocDueDate));
			}
			else
			{
				list.Add(new SqlParameter("@EndDate", null));
			}
			list.Add(new SqlParameter("@DocStatus", document.DocStatus));
			if (document.Cancelled == BoYesNoEnum.tYES)
			{
				list.Add(new SqlParameter("@Canceled", "Y"));
			}
			else
			{
				list.Add(new SqlParameter("@Canceled", "N"));
			}
			list.Add(new SqlParameter("@CardCode", document.CardCode));
			list.Add(new SqlParameter("@UserName", document.UserName));
			list.Add(new SqlParameter("@isIns", document.isIns ?? "N"));
			if (document.BPLId > 0 || string.IsNullOrEmpty(document.BPLId.ToString()))
			{
				list.Add(new SqlParameter("@Branch", document.BPLId));
			}
			else
			{
				list.Add(new SqlParameter("@Branch", -1));
			}
			list.Add(new SqlParameter("@DocNum", document.DocNum));
			list.Add(new SqlParameter("@Requester", document.Requester));
			if (document.ReqType > 0)
			{
				list.Add(new SqlParameter("@ReqType", document.ReqType));
			}
			else
			{
				list.Add(new SqlParameter("@ReqType", null));
			}
			DataTable dataTable = ExecuteDataTable("apz_d_document_get", tbl, CommandType.StoredProcedure, list.ToArray());
			if (!string.IsNullOrEmpty(document.FilterString))
			{
				string filterText = GetUnitOfWork().FilterSetting.GetFilterText(document.FilterString);
				DataRow[] array = dataTable.Select(filterText);
				if (array.Length != 0)
				{
					return array.CopyToDataTable();
				}
				return new DataTable();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search data table " + tbl, ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDocLines(int docEntry, string tbl, string tblLine, string filterLine = "")
	{
		try
		{
			DataTable dt = ExecuteDataTable("apz_Document_GetDocLine", "RDR1", CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry), new SqlParameter("@tbl", tbl), new SqlParameter("@tblLine", tblLine));
			if (!string.IsNullOrEmpty(filterLine))
			{
				string filterText = GetUnitOfWork().FilterSetting.GetFilterText(filterLine);
				DataRow[] array = dt.Select(filterText);
				if (array.Length != 0)
				{
					return array.CopyToDataTable();
				}
				return new DataTable();
			}
			MakeSourceView(ref dt);
			return dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Read line data sales FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable DMSGetDocLines(string docEntry, string tbl, string tblLine, string filterLine = "")
	{
		try
		{
			DataTable dt = ExecuteDataTable("apz_D_Document_GetDocLine", "RDR1", CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry), new SqlParameter("@tbl", tbl), new SqlParameter("@tblLine", tblLine));
			if (!string.IsNullOrEmpty(filterLine))
			{
				string filterText = GetUnitOfWork().FilterSetting.GetFilterText(filterLine);
				DataRow[] array = dt.Select(filterText);
				if (array.Length != 0)
				{
					return array.CopyToDataTable();
				}
				return new DataTable();
			}
			MakeSourceView(ref dt);
			return dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Read line data sales FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDocLinesByLineStatus(int docEntry, string tbl, string tblLine)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "SELECT A.DocEntry, \r\n\t\t                                A.LineNum , \r\n                                        A.ItemCode ,\r\n                                        A.dscription as ItemDescription,\r\n\t\t                                A.Quantity, \r\n\t\t                                A.Price,\r\n                                        A.LineStatus as LineStatusName,\r\n\t\t                                A.DiscPrcnt as DiscountPercent, \r\n                                        A.WhsCode as WarehouseCode,\r\n\t\t                                A.VatGroup ,\r\n                                        ISNULL (C.Rate,0) as TaxPercentagePerRow,\r\n\t\t                                A.LineTotal,\r\n                                        ISNULL( A.LineVat,0) LineVat,\r\n                                        A.ShipDate,\r\n                                        ISNULL ( A.OpenQty, 0 ) as RemainingOpenQuantity,\r\n                                        A.Currency,\r\n                                        A.Rate, A.BaseType\r\n                                        , ISNULL ( A.BaseEntry, 0 ) as BaseEntry\r\n                                        ,ISNULL ( A.BaseLine, 0 )  AS BaseLine\r\n                                        ,A.FreeTxT as Text\r\n                                        , ISNULL(A.UomCode,'') as UoMCode       \r\n                                FROM " + tblLine + " A\r\n                                INNER JOIN " + tbl + " B\r\n                                ON A.DocEntry = B.DocEntry\r\n                                LEFT JOIN OVTG C\r\n                                ON C.Code= A.VatGroup\r\n                                WHERE 1= 1 ";
			text += " AND A.DocEntry =@DocEntry ";
			text += " AND A.LineStatus ='O'";
			text += " ORDER BY A.LineNum DESC";
			result = ExecuteDataTable(text, CommandType.Text, new SqlParameter("@DocEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetDocLinesByLineStatusPurchaseOrder(int docEntry, string tbl, string tblLine)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "SELECT A.DocEntry, \r\n\t\t                                A.LineNum , \r\n                                        A.ItemCode ,\r\n                                        A.dscription as ItemDescription,\r\n\t\t                                A.Quantity, \r\n\t\t                                A.Price,\r\n                                        A.LineStatus as LineStatusName,\r\n\t\t                                A.DiscPrcnt as DiscountPercent, \r\n                                        A.WhsCode as WarehouseCode,\r\n\t\t                                A.VatGroup ,\r\n                                        ISNULL (C.Rate,0) as TaxPercentagePerRow,\r\n\t\t                                A.LineTotal,\r\n                                        A.ShipDate,\r\n                                        ISNULL ( A.OpenQty, 0 ) as RemainingOpenQuantity,\r\n                                        A.Currency,\r\n                                        A.Rate, A.BaseType\r\n                                        , ISNULL ( A.BaseEntry, 0 ) as BaseEntry\r\n                                        ,ISNULL ( A.BaseLine, 0 )  AS BaseLine\r\n                                        ,A.FreeTxT as Text\r\n                                        , ISNULL(A.UomCode,'') as UoMCode       \r\n                                FROM " + tblLine + " A\r\n                                INNER JOIN " + tbl + " B\r\n                                ON A.DocEntry = B.DocEntry\r\n                                LEFT JOIN OVTG C\r\n                                ON C.Code= A.VatGroup\r\n                                WHERE 1= 1 ";
			text = text + " AND A.DocEntry ='" + docEntry + "' ";
			text += " AND A.LineStatus ='O'";
			text += " ORDER BY A.LineNum DESC";
			result = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Read line data sales  FAILED ", ex.Message);
		}
		return result;
	}

	public DataTable GetDocLinesByListDocEntry(List<int> docEntry, ApzObjectTypes objtype)
	{
		DataTable result = new DataTable();
		try
		{
			string text = string.Join(",", docEntry.ToArray());
			string docTableByObjectType = GetDocTableByObjectType(objtype);
			string docLineTableByObjectType = GetDocLineTableByObjectType(objtype);
			string text2 = "SELECT A.DocEntry, \r\n\t\t                                A.LineNum , \r\n                                        A.ItemCode ,\r\n                                        A.dscription as ItemDescription,\r\n\t\t                                A.Quantity, \r\n\t\t                                A.Price,\r\n                                        A.LineStatus as LineStatusName,\r\n\t\t                                A.DiscPrcnt as DiscountPercent, \r\n                                        A.WhsCode as WarehouseCode,\r\n\t\t                                A.VatGroup ,\r\n                                        ISNULL (C.Rate,0) as TaxPercentagePerRow,\r\n\t\t                                A.LineTotal,\r\n                                        ISNULL( A.LineVat,0) LineVat,\r\n                                        A.ShipDate,\r\n                                        ISNULL ( A.OpenQty, 0 ) as RemainingOpenQuantity,\r\n                                        A.Currency,\r\n                                        A.Rate, A.BaseType  \r\n                                       ,A.FreeTxT as FreeTxT       \r\n                                FROM " + docLineTableByObjectType + " A\r\n                                INNER JOIN " + docTableByObjectType + " B\r\n                                ON A.DocEntry = B.DocEntry\r\n                                LEFT JOIN OVTG C\r\n                                ON C.Code= A.VatGroup\r\n                                WHERE 1= 1 ";
			text2 = text2 + " AND A.DocEntry in (' '" + text + ") ";
			text2 += " ORDER BY A.LineNum DESC";
			result = ExecuteDataTable(text2, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Read line data sales  FAILED ", ex.Message);
		}
		return result;
	}

	public int GetMaxDocEntry(ApzObjectTypes objtype)
	{
		try
		{
			string docTableByObjectType = GetDocTableByObjectType(objtype);
			string query = "SELECT   ISNULL(MAX(DocEntry ), 0) as MaxDocEntry FROM " + docTableByObjectType;
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["MaxDocEntry"]);
			}
			return 0;
		}
		catch (Exception)
		{
			return 0;
		}
	}

	public int GetMaxDocNum(ApzObjectTypes objtype)
	{
		try
		{
			string docTableByObjectType = GetDocTableByObjectType(objtype);
			string query = "SELECT   ISNULL(MAX(DocNum ), 0) as MaxDocNum FROM " + docTableByObjectType;
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["MaxDocNum"]);
			}
			return 0;
		}
		catch (Exception)
		{
			return 0;
		}
	}

	public int GetMinDocEntry(ApzObjectTypes objtype)
	{
		try
		{
			string query = "SELECT   ISNULL(Min(DocEntry ), 0) as MinDocEntry FROM " + GetDocTableByObjectType(objtype);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["MinDocEntry"]);
			}
			return 0;
		}
		catch (Exception)
		{
			return 0;
		}
	}

	public bool CheckDoc(int docEntry, string tbl)
	{
		try
		{
			string query = "SELECT   A.DocEntry \r\n                            FROM " + tbl + " A WHERE A.DocEntry = '" + docEntry + "'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public bool CheckDocLine(int docEntry, int lineNum, string tbl)
	{
		try
		{
			string text = "SELECT * FROM " + tbl + " WHERE DocEntry = '" + docEntry + "'";
			text = text + " AND LineNum = '" + lineNum + "'";
			DataTable dataTable = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
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

	public HttpResult SaveDataSet(DataSet ds, ApzObjectTypes objtype)
	{
		try
		{
			if (objtype != ApzObjectTypes.oDrafts && ds.Tables.Count > 1)
			{
				DataTable dataTable = ds.Tables[1];
				if (dataTable.IsNotNull())
				{
					DataRow dataRow = dataTable.Rows[0];
					if (dataRow != null && Function.ToString(dataRow["BaseType"]) == 202.ToString())
					{
						using (DatabaseService.OpenConnection())
						{
							using DbTransaction dbTransaction = DatabaseService.BeginTransaction(IsolationLevel.RepeatableRead);
							HttpResult httpResult = Process(objtype, -1, Enums.TransactionType.A, ds);
							if (!string.IsNullOrEmpty(httpResult.Message))
							{
								dbTransaction.Rollback();
								return httpResult;
							}
							DataTable dataTable2 = ds.Tables[0];
							string docTableByObjectType = GetDocTableByObjectType(objtype);
							DataTable tableStruct = GetTableStruct(docTableByObjectType);
							if (!tableStruct.IsNotNull())
							{
								throw new Exception("Could not get data structure");
							}
							IEnumerable<string> source = from DataColumn t in tableStruct.Columns
								select t.ColumnName;
							dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source.ToArray());
							int num = 0;
							num = Function.ParseInt(GetObjectId(objtype)) - 1;
							dataTable2.Rows[0]["DocEntry"] = num;
							CreateTempTable(dataTable2, "#APZ_DOC");
							BulkCopy(dataTable2, "#APZ_DOC");
							DataTable dataTable3 = ds.Tables[1];
							string docLineTableByObjectType = GetDocLineTableByObjectType(objtype);
							DataTable tableStruct2 = GetTableStruct(docLineTableByObjectType);
							if (!tableStruct2.IsNotNull())
							{
								throw new Exception("Could not get data line structure");
							}
							source = from DataColumn t in tableStruct2.Columns
								select t.ColumnName;
							dataTable3 = dataTable3.DefaultView.ToTable(distinct: false, source.ToArray());
							CreateTempTable(dataTable3, "#APZ_DOCLINE");
							BulkCopy(dataTable3, "#APZ_DOCLINE");
							DataSet dataBackflush = GetDataBackflush(objtype);
							if (dataBackflush.Tables.Count > 1)
							{
								dataTable2 = dataBackflush.Tables[0];
								dataTable3 = dataBackflush.Tables[1];
								DataSet dataSet = new DataSet();
								dataSet.Tables.AddRange(new DataTable[2]
								{
									dataTable2.Copy(),
									dataTable3.Copy()
								});
								httpResult = Process((objtype == ApzObjectTypes.oInventoryGenEntry) ? ApzObjectTypes.oInventoryGenExit : ApzObjectTypes.oInventoryGenEntry, -1, Enums.TransactionType.A, dataSet);
								if (!string.IsNullOrEmpty(httpResult.Message))
								{
									dbTransaction.Rollback();
									return httpResult;
								}
							}
							dbTransaction.Commit();
							return httpResult;
						}
					}
				}
			}
			return Process(objtype, -1, Enums.TransactionType.U, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataSet GetDataBackflush(ApzObjectTypes type)
	{
		return ExecuteData("apz_document_getBackflush", CommandType.StoredProcedure, new SqlParameter("@ObjType", type));
	}

	public HttpResult SaveAsDraftDataSet(DataSet doc)
	{
		try
		{
			return Process(ApzObjectTypes.oDrafts, -1, Enums.TransactionType.A, doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		if (objtype == ApzObjectTypes.oDrafts)
		{
			switch (transtype)
			{
			case Enums.TransactionType.R:
				if (objtype == ApzObjectTypes.oDrafts)
				{
					string query = "  DELETE ODRF WHERE DocEntry=@DocEntry\r\n                                            DELETE DRF1 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF2 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF3 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF4 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF5 WHERE AbsEntry = @DocEntry\r\n                                            DELETE DRF6 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF7 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF8 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF9 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF10 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF11 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF12 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF13 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF14 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF15 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF16 WHERE AbsEntry = @DocEntry\r\n                                            DELETE DRF17 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF18 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF19 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF20 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF21 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF22 WHERE DocEntry = @DocEntry\r\n                                            DELETE DRF23 WHERE DocEntry = @DocEntry\r\n                                            ";
					if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@DocEntry", objId)) > 0)
					{
						return new HttpResult();
					}
					return new HttpResult(Enums.MessageCode.Error, "No record affected");
				}
				return new HttpResult(Enums.MessageCode.Error, "The requested action is not supported for this object");
			case Enums.TransactionType.L:
				if (ExecuteNonQuery("UPDATE ODRF SET DocStatus='C' WHERE DocEntry=@DocEntry", CommandType.Text, new SqlParameter("@DocEntry", objId)) > 0)
				{
					return new HttpResult();
				}
				return new HttpResult(Enums.MessageCode.Error, "No record affected");
			default:
				return SaveAsDraft(document);
			}
		}
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			if (Exists(document, objtype))
			{
				transtype = Enums.TransactionType.U;
				return UpdateData(document, objtype, ref objId, ref objNum, ref transtype);
			}
			transtype = Enums.TransactionType.A;
			return AddNew(document, objtype, ref objId, ref objNum, ref transtype);
		case Enums.TransactionType.L:
		{
			DataTable dataTable = ExecuteDataTable("apz_document_close", CommandType.StoredProcedure, new SqlParameter("@obj_id", Function.ParseInt(objId)), new SqlParameter("@objtype", Function.ToString((int)objtype)));
			if (dataTable.IsNotNull())
			{
				int num2 = Function.ParseInt(dataTable.Rows[0][0]);
				if (num2 > 0)
				{
					string text2 = Function.ToString(dataTable.Rows[0][1]);
					throw new Exception(num2 + "-" + text2);
				}
			}
			AfterAction(document, objtype, objId, objNum, Enums.TransactionType.L);
			break;
		}
		case Enums.TransactionType.C:
		{
			DataSet dataSet = ExecuteData("apz_document_cancel", CommandType.StoredProcedure, new SqlParameter("@obj_id", Function.ParseInt(objId)), new SqlParameter("@objtype", Function.ToString((int)objtype)));
			if (dataSet.IsNotNull())
			{
				int num = Function.ParseInt(dataSet.Tables[dataSet.Tables.Count - 1].Rows[0][0]);
				if (num > 0)
				{
					string text = Function.ToString(dataSet.Tables[dataSet.Tables.Count - 1].Rows[0][1]);
					throw new Exception(num + "-" + text);
				}
			}
			AfterAction(document, objtype, objId, objNum, Enums.TransactionType.L);
			break;
		}
		}
		BatchAndSerialNumberProcess(document, objId, objtype, transtype, objNum);
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
			ApzObjectTypes docObjType = objtype;
			InitData(doc, objtype, ref objId, ref objNum, ref tableDocName, ref tableLineName, ref series, transtype, ref cancelEntry, ref docObjType);
			BeforeAction(doc, objtype, objId, objNum, transtype, cancelEntry);
			string query = string.Format("update {1} set ObjType=@ObjType\r\n                                            insert into {0} select * from {1} \r\n                                            update {2} set DocEntry=@DocEntry, LineStatus='O',ObjType=@ObjType\r\n                                            insert into {3} select * from {2} where {2}.ItemCode is not null and {2}.ItemCode <>'' and {2}.Quantity>0\r\n                                            update onnm set AutoKey=@DocEntry+1 where ObjectCode=@ObjCode\r\n                                            update nnm1 set NextNumber=@DocNum+1 WHERE ObjectCode=@ObjCode AND ISNULL(Series,-1)=@Series", tableDocName, "#APZ_DOC", "#APZ_DOCLINE", tableLineName);
			ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@DocEntry", objId), new SqlParameter("@ObjCode", Function.ToString((int)objtype)), new SqlParameter("@DocNum", objNum), new SqlParameter("@Series", series), new SqlParameter("@ObjType", Function.ToString((int)docObjType)));
			AfterAction(doc, objtype, objId, objNum, transtype, cancelEntry);
			BatchAndSerialNumberProcess(doc, objId, objtype, transtype, objNum);
			FileAttachmentProcess(doc, objtype, Function.ParseInt(objId));
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	private void InitData(DataSet doc, ApzObjectTypes objtype, ref string objId, ref string objNum, ref string tableDocName, ref string tableLineName, ref int series, Enums.TransactionType transtype, ref int cancelEntry, ref ApzObjectTypes docObjType)
	{
		DataTable dataTable = doc.Tables[0];
		if (objtype == ApzObjectTypes.oDrafts)
		{
			docObjType = (ApzObjectTypes)Function.ParseInt(dataTable.Rows[0]["ObjType"]);
		}
		else
		{
			docObjType = objtype;
		}
		tableDocName = GetDocTableByObjectType(objtype);
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
			if (objtype == ApzObjectTypes.oDrafts)
			{
				num2 = Function.ParseInt(dataTable.Rows[0]["DocNum"]);
				break;
			}
			num2 = Function.ParseInt(GetObjectNum(objtype, num3, series, usersign));
			dataTable.Rows[0]["DocNum"] = num2;
			break;
		}
		case Enums.TransactionType.U:
			num = Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			num2 = Function.ParseInt(dataTable.Rows[0]["DocNum"]);
			break;
		}
		CreateTempTable(dataTable, "#APZ_DOC");
		BulkCopy(dataTable, "#APZ_DOC");
		DataTable dataTable2 = doc.Tables[1];
		tableLineName = GetDocLineTableByObjectType(objtype);
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
			list = ((!new ApzObjectTypes[5]
			{
				ApzObjectTypes.oQuotations,
				ApzObjectTypes.oOrders,
				ApzObjectTypes.oPurchaseRequest,
				ApzObjectTypes.oPurchaseQuotations,
				ApzObjectTypes.oPurchaseOrders
			}.Contains(objtype)) ? (from DataColumn t in tableStruct.Columns
				where !new string[16]
				{
					"CntctCode", "NumAtCard", "DocDueDate", "PayToCode", "Address", "TrnspCode", "TrackNo", "StampNo", "SlpCode", "OwnerCode",
					"Comments", "JrnlMemo", "UserSign", "UserSign2", "CreateDate", "DataSource"
				}.Contains(t.ColumnName) && !t.ColumnName.StartsWith("U_")
				select t.ColumnName).ToList() : new string[4] { "CardCode", "CardName", "Series", "DocNum" }.ToList());
			for (int num4 = 0; num4 < list.Count; num4++)
			{
				if (!dataTable.Rows[0][list[num4]].Equals(dataTable3.Rows[0][list[num4]]))
				{
					throw new Exception($"Fields cannot be updated [{objtype} - {list[num4]}]");
				}
			}
		}
		string query = "SELECT T1.DocEntry FROM #APZ_DOCLINE T0 INNER JOIN " + GetDocLineTableByObjectType(objtype) + " T1 ON T0.BaseEntry=T1.DocEntry and T0.BaseType=T1.ObjType WHERE T1.DocEntry <>@DocEntry";
		DataTable dataTable4 = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@DocEntry", Function.ParseInt(objId)));
		if (dataTable4.IsNotNull())
		{
			cancelEntry = Function.ParseInt(dataTable4.Rows[0]["DocEntry"]);
		}
		objId = num.ToString();
		objNum = num2.ToString();
	}

	public void BatchAndSerialNumberProcess(DataSet ds, string objId, ApzObjectTypes objType, Enums.TransactionType transtype, string objNum)
	{
		if (ds == null || ds.Tables.Count < 3 || ds.Tables[2] == null || ds.Tables[2].Columns.Count == 0)
		{
			string query = string.Format("Select top 0 ItemCode, DistNumber, MnfSerial, LotNumber, 0 BaseLineNumber, 0 LogQty, ObjType, AbsEntry, SysNumber INTO {0}\r\n                                        from OBTN\r\n\r\n                                    Select top 0 * INTO {1} from OBTN ", "#APZ_Doc_Batch", "#APZ_Batch_Detail");
			ExecuteNonQuery(query, CommandType.Text);
		}
		else
		{
			DataTable dataTable = ds.Tables[2];
			DataTable tableStruct = GetTableStruct("OBTN");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data batch/serial structure");
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

	private void FileAttachmentProcess(DataSet ds, ApzObjectTypes objType, int docEntry)
	{
		if (!ds.Tables.Contains("ATTFILE"))
		{
			return;
		}
		DataTable dataTable = ds.Tables["ATTFILE"];
		if (dataTable != null && dataTable.Rows.Count != 0)
		{
			int absEntry = GetAbsEntry(objType, docEntry);
			string text = new CommonsRepository(DatabaseService).FileAttachmentProcess(dataTable, absEntry);
			if (text.StartsWith("ERR:"))
			{
				throw new Exception(text.Remove(0, 4));
			}
			int num = Function.ParseInt(text);
			if (num == 0 && dataTable.Rows.Count > 0)
			{
				throw new Exception("1001 - Save file Error (AbsEntry return must greater than zero)");
			}
			if (num > 0)
			{
				UpdateDocumentFileAttachment(objType, num, docEntry);
			}
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
		DataTable dataTable3 = ExecuteDataTable("apz_document_item_whs", CommandType.StoredProcedure, parameters);
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
		DataTable dataTable2 = ExecuteDataTable("apz_document_itemlogmsg", CommandType.StoredProcedure, parameters);
		if (dataTable2.IsNotNull())
		{
			int num2 = Function.ParseInt(dataTable2.Rows[0][0]);
			if (num2 > 0)
			{
				string text2 = Function.ToString(dataTable2.Rows[0][1]);
				throw new Exception(num2 + "-" + text2);
			}
		}
		DataTable dataTable3 = ExecuteDataTable("apz_document_unreported_tax", CommandType.StoredProcedure, parameters);
		if (dataTable3.IsNotNull())
		{
			int num3 = Function.ParseInt(dataTable3.Rows[0][0]);
			if (num3 > 0)
			{
				string text3 = Function.ToString(dataTable3.Rows[0][1]);
				throw new Exception(num3 + "-" + text3);
			}
		}
		DataTable dataTable4 = ExecuteDataTable("apz_document_whs_journal", CommandType.StoredProcedure, parameters);
		if (dataTable4.IsNotNull())
		{
			int num4 = Function.ParseInt(dataTable4.Rows[0][0]);
			if (num4 > 0)
			{
				string text4 = Function.ToString(dataTable4.Rows[0][1]);
				throw new Exception(num4 + "-" + text4);
			}
		}
		if (cancelEntry <= 0)
		{
			return;
		}
		DataTable dataTable5 = ExecuteDataTable("apz_document_cancel", CommandType.StoredProcedure, new SqlParameter("@obj_id", cancelEntry), new SqlParameter("@objtype", Function.ToString((int)objtype)));
		if (dataTable5.IsNotNull())
		{
			int num5 = Function.ParseInt(dataTable5.Rows[0][0]);
			if (num5 > 0)
			{
				string text5 = Function.ToString(dataTable5.Rows[0][1]);
				throw new Exception(num5 + "-" + text5);
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
				ApzObjectTypes docObjType = objtype;
				InitData(doc, objtype, ref objId, ref objNum, ref tableDocName, ref tableLineName, ref series, transtype, ref cancelEntry, ref docObjType);
				BeforeAction(doc, objtype, objId, objNum, transtype);
				if (objtype == ApzObjectTypes.oOrders || objtype == ApzObjectTypes.oQuotations || objtype == ApzObjectTypes.oPurchaseRequest || objtype == ApzObjectTypes.oPurchaseQuotations || objtype == ApzObjectTypes.oPurchaseOrders || objtype == ApzObjectTypes.oDrafts)
				{
					string query = string.Format("delete {0} where DocEntry=@DocEntry\r\n                                                    delete {3} where DocEntry=@DocEntry\r\n                                                    update {1} set ObjType=@ObjType\r\n                                                    update {2} set DocEntry=@DocEntry,ObjType=@ObjType\r\n                                                    insert into {0} select * from {1} \r\n                                                    insert into {3} select * from {2} where {2}.ItemCode is not null and {2}.ItemCode <>'' and {2}.Quantity>0", tableDocName, "#APZ_DOC", "#APZ_DOCLINE", tableLineName);
					SqlParameter[] array = new SqlParameter[2]
					{
						new SqlParameter("@DocEntry", objId),
						null
					};
					int num = (int)docObjType;
					array[1] = new SqlParameter("@ObjType", num.ToString());
					ExecuteNonQuery(query, CommandType.Text, array);
				}
				else
				{
					string query2 = string.Format("delete {0} where DocEntry=@DocEntry\r\n                                                    insert into {0} select * from {1}", tableDocName, "#APZ_DOC");
					ExecuteNonQuery(query2, CommandType.Text, new SqlParameter("@DocEntry", objId));
				}
				AfterAction(doc, objtype, objId, objNum, transtype);
				BatchAndSerialNumberProcess(doc, objId, objtype, transtype, objNum);
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

	private string GetDocTableFreightByObjectType(ApzObjectTypes objtype)
	{
		try
		{
			switch (objtype)
			{
			case ApzObjectTypes.oChartOfAccounts:
				return "ACT3";
			case ApzObjectTypes.oBusinessPartners:
				return "CRD3";
			case ApzObjectTypes.oItems:
				return "ITM3";
			case ApzObjectTypes.oVatGroups:
				return "VTG3";
			case ApzObjectTypes.oInvoices:
				return "INV3";
			case ApzObjectTypes.oCreditNotes:
				return "RIN3";
			case ApzObjectTypes.oDeliveryNotes:
				return "DLN3";
			case ApzObjectTypes.oReturns:
				return "RDN3";
			case ApzObjectTypes.oOrders:
				return "RDR3";
			case ApzObjectTypes.oQuotations:
				return "QUT3";
			case ApzObjectTypes.oPurchaseInvoices:
				return "PCH3";
			case ApzObjectTypes.oPurchaseCreditNotes:
				return "RPC3";
			case ApzObjectTypes.oAPReserveInvoice:
				return "PCH3";
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				return "PDN3";
			case ApzObjectTypes.oPurchaseReturns:
				return "RPD3";
			case ApzObjectTypes.oPurchaseOrders:
				return "POR3";
			case ApzObjectTypes.oIncomingPayments:
				return "RCT3";
			case ApzObjectTypes.oJournalVouchers:
				return "BTD3";
			case ApzObjectTypes.oJournalEntries:
				return "JDT3";
			case ApzObjectTypes.oContacts:
				return "CLG3";
			case ApzObjectTypes.oVendorPayments:
				return "VPM3";
			case ApzObjectTypes.oSalesPersons:
				return "SLP3";
			case ApzObjectTypes.oInventoryGenEntry:
				return "IGN3";
			case ApzObjectTypes.oInventoryGenExit:
				return "IGE3";
			case ApzObjectTypes.oWarehouses:
				return "WHS3";
			case ApzObjectTypes.oProductTrees:
				return "ITT3";
			case ApzObjectTypes.oStockTransfer:
				return "WTR3";
			case ApzObjectTypes.oAlternateCatNum:
				return "SCN3";
			case ApzObjectTypes.oSalesOpportunities:
				return "OPR3";
			case ApzObjectTypes.oDrafts:
				return "DRF3";
			case ApzObjectTypes.oPaymentsDrafts:
				return "PDF3";
			case ApzObjectTypes.oKnowledgeBaseSolutions:
				return "SLT3";
			case ApzObjectTypes.oServiceContracts:
				return "CTR3";
			case ApzObjectTypes.oServiceCalls:
				return "SCL3";
			case ApzObjectTypes.oUserKeys:
				return "UKD3";
			case ApzObjectTypes.oQueue:
				return "QUE3";
			case ApzObjectTypes.oSalesForecast:
				return "FCT3";
			case ApzObjectTypes.oProductionOrders:
				return "WOR3";
			case ApzObjectTypes.oStockTransferDraft:
				return "DRF3";
			case ApzObjectTypes.oPurchaseQuotations:
				return "PQT3";
			case ApzObjectTypes.oInventoryTransferRequest:
				return "WTQ3";
			case ApzObjectTypes.oPurchaseRequest:
				return "PRQ3";
			case ApzObjectTypes.oBlanketAgreement:
				return "OAT3";
			case ApzObjectTypes.oResources:
				return "RSC3";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public HttpResult SaveAsDraft(DataSet doc)
	{
		try
		{
			DataTable dataTable = doc.Tables[0];
			List<string> tableName = Function.GetTableName(ApzObjectTypes.oDrafts);
			if (tableName.Count < 2)
			{
				return new HttpResult(Enums.MessageCode.Error, "Could not get data structure");
			}
			DataTable tableStruct = GetTableStruct(tableName[0]);
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			int num = Function.ParseInt(dataTable.Rows[0]["DraftKey"]);
			int num2 = Function.ParseInt(dataTable.Rows[0]["DocNum"]);
			if (num <= 0)
			{
				num = Function.ParseInt(GetObjectId(ApzObjectTypes.oDrafts));
			}
			dataTable.Rows[0]["DocEntry"] = num;
			CreateTempTable(dataTable, "#APZ_DOC");
			BulkCopy(dataTable, "#APZ_DOC");
			DataTable dataTable2 = doc.Tables[1];
			DataTable tableStruct2 = GetTableStruct(tableName[1]);
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data line structure");
			}
			source = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable2, "#APZ_DOCLINE");
			BulkCopy(dataTable2, "#APZ_DOCLINE");
			int num3 = Function.ParseInt(dataTable.Rows[0]["Series"]);
			string query = string.Format("Delete from {0} where DocEntry = @DocEntry\r\n                                            DELETE from {3} where DocEntry = @DocEntry\r\n                                            insert into {0} select * from {1} \r\n                                            update {2} set DocEntry=@DocEntry\r\n                                            insert into {3} select * from {2} where {2}.ItemCode is not null and {2}.ItemCode <>''\r\n                                            update onnm set AutoKey=(select max(DocEntry) from {0}) + 1 where ObjectCode=@ObjCode\r\n                                            update nnm1 set NextNumber=(select max(DocNum) from {0})+1 where ObjectCode=@ObjCode and Series=@Series", tableName[0], "#APZ_DOC", "#APZ_DOCLINE", tableName[1]);
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@DocEntry", num), new SqlParameter("@Series", num3), new SqlParameter("@ObjCode", Function.ToString(112)), new SqlParameter("@DocNum", Function.ParseInt(num2))) <= 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Could not save as draft!");
			}
			BatchSerialDraftProcess(doc, Function.ToString(num), Function.ParseInt(dataTable.Rows[0]["ObjType"]), Enums.TransactionType.A, Function.ToString(num2));
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void BatchSerialDraftProcess(DataSet ds, string objId, int objType, Enums.TransactionType transtype, string objNum)
	{
		if (ds == null || ds.Tables.Count < 3 || ds.Tables[2] == null || ds.Tables[2].Columns.Count == 0)
		{
			return;
		}
		DataTable dataTable = ds.Tables[2];
		DataTable tableStruct = GetTableStruct("ODBN");
		if (!tableStruct.IsNotNull())
		{
			throw new Exception("Could not get data batch/serial draft structure");
		}
		IEnumerable<string> source = from DataColumn t in tableStruct.Columns
			select t.ColumnName;
		DataTable dt = dataTable.DefaultView.ToTable(false, "ItemCode", "DistNumber", "MnfSerial", "LotNumber", "BaseLineNumber", "LogQty", "ObjType", "AbsEntry", "SysNumber");
		dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
		CreateTempTable(dataTable, "#APZ_Batch_Detail");
		BulkCopy(dataTable, "#APZ_Batch_Detail");
		CreateTempTable(dt, "#APZ_Doc_Batch");
		BulkCopy(dt, "#APZ_Doc_Batch");
		DataTable dataTable2 = ExecuteDataTable("APZ_DocumentDraft_Batch_Process", CommandType.StoredProcedure, new SqlParameter("@obj_id", Function.ParseInt(objId)), new SqlParameter("@objtype", objType), new SqlParameter("@transtype", transtype.ToString()), new SqlParameter("@objNum", Function.ParseInt(objNum)));
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

	private bool Exists(DataSet doc, ApzObjectTypes type)
	{
		try
		{
			string docTableByObjectType = GetDocTableByObjectType(type);
			string query = "SELECT 1 FROM " + docTableByObjectType + " WHERE DocEntry=" + Function.ParseInt(doc.Tables[0].Rows[0]["DocEntry"]);
			return ExecuteDataTable(query, CommandType.Text).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
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

	public int GetMaxDocEntrySaveAsDraf()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select isnull (MAX(DocEntry),0) as DocEntry  from ODRF ", CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["DocEntry"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxDocEntrySaveAsDraft FAILED ", ex.Message);
		}
		return 0;
	}

	public DataTable GetPayment(int docEntry, ApzObjectTypes objType)
	{
		try
		{
			return ExecuteDataTable("Apz_Document_GetPaymentTerm", CommandType.StoredProcedure, new SqlParameter("@table", GetDocTableByObjectType(objType)), new SqlParameter("@DocEntry", docEntry), new SqlParameter("@ObjType", (int)objType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	internal DataTable GetFreight(string tblName, int docEntry)
	{
		try
		{
			string query = "Apz_Document_GetFreight";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@Tbl", tblName), new SqlParameter("@DocEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	internal DataTable GetFreightDefault()
	{
		try
		{
			string query = "Apz_Document_GetFreightDefault";
			return ExecuteDataTable(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDocLines(int docEntry, string tbl, string tblLine)
	{
		throw new NotImplementedException();
	}

	internal string Close(int docEntry, ApzObjectTypes apzObjectTypes)
	{
		try
		{
			return Process(apzObjectTypes, -1, Enums.TransactionType.L, new DataSet(), docEntry.ToString()).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable LoadBatchNumberStruct(ApzObjectTypes apzObjectTypes)
	{
		try
		{
			DataTable tableStruct = GetTableStruct("OBTN");
			if (tableStruct.Columns.Count > 0)
			{
				if (!tableStruct.Columns.Contains("SelectedQty"))
				{
					tableStruct.Columns.Add("SelectedQty", typeof(double));
				}
				if (!tableStruct.Columns.Contains("BaseLineNumber"))
				{
					tableStruct.Columns.Add("BaseLineNumber", typeof(int));
				}
				if (!tableStruct.Columns.Contains("WhsCode"))
				{
					tableStruct.Columns.Add("WhsCode", typeof(string));
				}
				if (!tableStruct.Columns.Contains("BinAbs"))
				{
					tableStruct.Columns.Add("BinAbs", typeof(int));
				}
				if (!tableStruct.Columns.Contains("BinCode"))
				{
					tableStruct.Columns.Add("BinCode", typeof(string));
				}
				if (!tableStruct.Columns.Contains("ManBatchSerial"))
				{
					tableStruct.Columns.Add("ManBatchSerial", typeof(string));
				}
				if (!tableStruct.Columns.Contains("LogQty"))
				{
					tableStruct.Columns.Add("LogQty", typeof(double));
				}
				if (!tableStruct.Columns.Contains("IsExits"))
				{
					tableStruct.Columns.Add("IsExits", typeof(string));
				}
				if (!tableStruct.Columns.Contains("TranType"))
				{
					tableStruct.Columns.Add("TranType", typeof(int));
				}
				if (!tableStruct.Columns.Contains("IsBase"))
				{
					tableStruct.Columns.Add("IsBase", typeof(string));
				}
				if (!tableStruct.Columns.Contains("BinDescription"))
				{
					tableStruct.Columns.Add("BinDescription", typeof(string));
				}
			}
			return tableStruct;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBatchSerialByCode(string itemcode, string whscode, int lineNum, string barcode, string objType)
	{
		try
		{
			return ExecuteDataTable("apz_document_getbatchserial_bybarcode", CommandType.StoredProcedure, new SqlParameter("@ItemCode", itemcode), new SqlParameter("@WhsCode", whscode), new SqlParameter("@LineNum", lineNum), new SqlParameter("@BarCode", barcode), new SqlParameter("@ObjType", objType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBatchSerialByDocEntry(int docEntry, ApzObjectTypes objType)
	{
		try
		{
			return ExecuteDataTable("apz_document_getbatchserial_byDocEntry", CommandType.StoredProcedure, new SqlParameter("@DocEntry", Function.ToString(docEntry)), new SqlParameter("@ObjType", (Function.ToString((int)objType) == "13000") ? "13" : Function.ToString((int)objType)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable CopyBatchSerial(string fromentry, string fromtype)
	{
		try
		{
			return ExecuteDataTable("apz_document_copy_batch_serial", CommandType.StoredProcedure, new SqlParameter("@DocEntry", fromentry), new SqlParameter("@ObjType", (fromtype == "13000") ? "13" : ((fromtype == "25") ? "18" : fromtype)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadSeries(string usercode, ApzObjectTypes apzObjectTypes)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_Series_LoadAllByObject", CommandType.StoredProcedure, new SqlParameter("@ObjectCode", Function.ToString((int)apzObjectTypes)), new SqlParameter("@UserCode", Function.ToString(usercode)));
			if (dataTable.Columns.Count > 0 && apzObjectTypes != ApzObjectTypes.oInventoryGenEntry && apzObjectTypes != ApzObjectTypes.oInventoryGenExit && apzObjectTypes != ApzObjectTypes.oStockTransfer && apzObjectTypes != ApzObjectTypes.oInventoryTransferRequest)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Series"] = -1;
				dataRow["SeriesName"] = "Manual";
				dataRow["BPLId"] = 0;
				dataTable.Rows.Add(dataRow);
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public int GetDocumentNumber(int series, ApzObjectTypes apzObjectTypes)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetDocumentNumberQuery(), CommandType.Text, new SqlParameter("@Series", series));
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	protected virtual string GetDocumentNumberQuery()
	{
		return "select NextNumber from NNM1 WHERE Series=@Series";
	}

	public DataTable TableLine(int docentry, ApzObjectTypes objtype, string filter = "")
	{
		try
		{
			string docTableByObjectType = GetDocTableByObjectType(objtype);
			string docLineTableByObjectType = GetDocLineTableByObjectType(objtype);
			return GetDocLines(docentry, docTableByObjectType, docLineTableByObjectType, filter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable DMSTableLine(string docentry, ApzObjectTypes objtype, string filter = "")
	{
		try
		{
			string docTableByObjectType = GetDocTableByObjectType(objtype);
			string docLineTableByObjectType = GetDocLineTableByObjectType(objtype);
			return DMSGetDocLines(docentry, docTableByObjectType, docLineTableByObjectType, filter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable Search(DocumentsImpl doc, ApzObjectTypes objtype)
	{
		try
		{
			string docTableByObjectType = GetDocTableByObjectType(objtype);
			string docLineTableByObjectType = GetDocLineTableByObjectType(objtype);
			return GetDocuments(doc, docTableByObjectType, docLineTableByObjectType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable DMSSearch(DMSSaleImpl doc, ApzObjectTypes objtype)
	{
		try
		{
			string docTableByObjectType = GetDocTableByObjectType(objtype);
			string docLineTableByObjectType = GetDocLineTableByObjectType(objtype);
			return DMSGetDocuments(doc, docTableByObjectType, docLineTableByObjectType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetListBpCatalog(ApzObjectTypes objtype)
	{
		try
		{
			return ExecuteDataTable("APZ_SalesOderGetBPCatalog", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable Duplicate(int docEntry, ApzObjectTypes objtype)
	{
		try
		{
			string docTableByObjectType = GetDocTableByObjectType(objtype);
			string docLineTableByObjectType = GetDocLineTableByObjectType(objtype);
			DataTable dt = ExecuteDataTable("apz_Document_duplicate", docLineTableByObjectType, CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry), new SqlParameter("@tbl", docTableByObjectType), new SqlParameter("@tblLine", docLineTableByObjectType));
			MakeSourceView(ref dt);
			return dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetFieldByItemCode(string itemCode, ApzObjectTypes objtype)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@ItemCode", itemCode)
			};
			DataTable dataTable = ExecuteDataTable("APZ_SalesOder_GetFieldByItemCode", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult CloseDoc(int docEntry, ApzObjectTypes objtype)
	{
		try
		{
			return Process(objtype, -1, Enums.TransactionType.L, null, docEntry.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult CancelDoc(int docEntry, ApzObjectTypes objtype)
	{
		try
		{
			return Process(objtype, -1, Enums.TransactionType.C, null, docEntry.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetFreight(int docEntry, ApzObjectTypes objtype)
	{
		return GetFreight(GetDocTableFreightByObjectType(objtype), docEntry);
	}

	public DataTable GetFreightDefault(ApzObjectTypes objtype)
	{
		return GetFreightDefault();
	}

	public DataTable GetTaxGroup(ApzObjectTypes objtype)
	{
		try
		{
			string query = "SELECT Code,Name FROM dbo.OVTG WHERE Category='I'";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetTaxGroup  FAILED ", ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetFreightCode(ApzObjectTypes objtype)
	{
		try
		{
			return ExecuteDataTable(GetFreightCodeQuery(), CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetFreightCodeQuery()
	{
		return "SELECT ExpnsCode AS Code,ExpnsName AS Name FROM OEXD UNION ALL SELECT 0,''";
	}

	public DataTable GetRateByVatGroup(ApzObjectTypes objtype)
	{
		try
		{
			string query = "SELECT Code AS VatGroup,Rate AS DocRate FROM OVTG";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable CopyFromLine(string type, string entry, string to)
	{
		try
		{
			DataTable dt = ExecuteDataTable("apz_document_copy", "RDR1", CommandType.StoredProcedure, new SqlParameter("@fromtype", (type == "13000") ? "13" : ((type == "25") ? "18" : type)), new SqlParameter("@docentry", entry), new SqlParameter("@totype", to));
			MakeSourceView(ref dt);
			return dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	protected virtual void MakeSourceView(ref DataTable dt)
	{
	}

	public DataTable CopyFrom(string type, string entry, string to, int bplId, string username)
	{
		try
		{
			return ExecuteDataTable("apz_document_copy_header", CommandType.StoredProcedure, new SqlParameter("@fromtype", (type == "13000") ? "13" : ((type == "25") ? "18" : type)), new SqlParameter("@docentry", entry), new SqlParameter("@totype", to), new SqlParameter("@bplId", bplId), new SqlParameter("@username", username));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public DataTable GetExpnsByCode(ApzObjectTypes objtype, string expnsCode)
	{
		try
		{
			string text = "VatGroupI";
			if (new ApzObjectTypes[7]
			{
				ApzObjectTypes.oQuotations,
				ApzObjectTypes.oOrders,
				ApzObjectTypes.oDeliveryNotes,
				ApzObjectTypes.oReturns,
				ApzObjectTypes.oInvoices,
				ApzObjectTypes.oReserveInvoices,
				ApzObjectTypes.oCreditNotes
			}.Contains(objtype))
			{
				text = "VatGroupO";
			}
			string query = "SELECT T0.*,T1.Code VatGroup,T1.Rate VatRate FROM OEXD T0 INNER JOIN OVTG T1 ON T0." + text + "=T1.Code WHERE T0.ExpnsCode=@ExpnsCode";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@ExpnsCode", expnsCode ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public HttpResult Remove(int docEntry, ApzObjectTypes objtype)
	{
		return Process(objtype, -1, Enums.TransactionType.R, new DataSet(), docEntry.ToString());
	}

	public DataTable GetLineShipTo(string cardCode)
	{
		try
		{
			return ExecuteDataTable(GetLineShipToQuery(), CommandType.Text, new SqlParameter("@cardCode", cardCode ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetLineShipToQuery()
	{
		return "select *, ISNULL(Street,'') +CHAR(13)+ISNULL(ZipCode,'')+' '+ISNULL(City,'')+CHAR(13) + isnull((Select UPPER(name) from OCRY t where t.code=a.country), '') FullAddress \r\n                    from crd1 a where CardCode = @cardCode and AdresType='S'";
	}

	public int GetDocNumByDocEntryType(int baseKey, int baseType)
	{
		try
		{
			List<string> tableName = Function.GetTableName((ApzObjectTypes)baseType);
			if (tableName != null && tableName.Count > 0)
			{
				DataTable dataTable = ExecuteDataTable("SELECT \"DocNum\" FROM " + tableName[0] + " WHERE \"DocEntry\"=@DocEntry;", CommandType.Text, new SqlParameter("@DocEntry", baseKey));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return Function.ParseInt(dataTable.Rows[0][0]);
				}
				return 0;
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	public DataTable GetOwnerByBpUser(string cardCode, int userID)
	{
		try
		{
			return ExecuteDataTable("APZ_Document_GetOwnerByBpUser", CommandType.StoredProcedure, new SqlParameter("@cardCode", cardCode), new SqlParameter("@userID", userID));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public int GetDocTypeByDraftKey(int draftKey)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT \"ObjType\" FROM ODRF WHERE \"DocEntry\"=@DraftKey;", CommandType.Text, new SqlParameter("@DraftKey", draftKey));
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return 0;
	}

	public int InsertDocumentApprovalRequest(SqlParameter[] paramater)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "USP_InsertDocumentApprovalRequest";
				DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, paramater);
				if (dataTable.IsNotNull())
				{
					int num = Function.ParseInt(dataTable.Rows[0][0]);
					if (num > 0)
					{
						dbTransaction.Commit();
						return num;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return -1;
	}

	public bool SaveDraftToDocument(ApzObjectTypes objectTypes, int docEntry, int draftKey)
	{
		try
		{
			if (!ExecuteDataTable("apz_document_savedraft", CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry), new SqlParameter("@DraftKey", draftKey), new SqlParameter("@ObjType", Function.ToString((int)objectTypes))).IsNotNull())
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return false;
	}

	public string GetWddStatus(ApzObjectTypes objType, int docEntry)
	{
		try
		{
			string docTableByObjectType = GetDocTableByObjectType(objType);
			string query = "SELECT \"WddStatus\" FROM " + docTableByObjectType + " WHERE \"DocEntry\"=@DocEntry;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@DocEntry", docEntry));
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return "";
	}

	public int GetDocEntryByDocNumType(int baseKey, int baseType)
	{
		try
		{
			List<string> tableName = Function.GetTableName((ApzObjectTypes)baseType);
			if (tableName != null && tableName.Count > 0)
			{
				DataTable dataTable = ExecuteDataTable("SELECT \"DocEntry\" FROM " + tableName[0] + " WHERE \"DocNum\"=@DocNum;", CommandType.Text, new SqlParameter("@DocNum", baseKey));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return Function.ParseInt(dataTable.Rows[0][0]);
				}
				return 0;
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	public DataTable GetDownPaymentToDraw(ApzObjectTypes objType, string cardCode, string docCur)
	{
		try
		{
			return ExecuteDataTable("APZ_Document_DownPaymentToDraw", CommandType.StoredProcedure, new SqlParameter("@objType", (int)objType), new SqlParameter("@cardCode", cardCode), new SqlParameter("@docCur", docCur));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetDownPaymentDrawed(ApzObjectTypes objType, int docEntry)
	{
		try
		{
			return ExecuteDataTable("APZ_Document_ViewDownPaymentDrawed", CommandType.StoredProcedure, new SqlParameter("@objType", (int)objType), new SqlParameter("@docEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetReferencedDocument(ApzObjectTypes objType, int docEntry)
	{
		try
		{
			string text = GetDocLineTableByObjectType(objType).Substring(0, 3) + "21";
			string query = "select * from " + text + " where \"DocEntry\"=@docEntry;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@docEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable ListChooseDocuments(ApzObjectTypes objType, string from, string to)
	{
		try
		{
			string docTableByObjectType = GetDocTableByObjectType(objType);
			string query = "apz_document_lst_choose";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@TableName", docTableByObjectType), new SqlParameter("@FromDate", from), new SqlParameter("@ToDate", to));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetReferencedDocumentBy(ApzObjectTypes objType, int docEntry)
	{
		try
		{
			string text = GetDocLineTableByObjectType(objType).Substring(0, 3) + "21";
			string query = "select * from " + text + " where \"RefDocEntr\"=@docEntry;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@docEntry", docEntry));
			dataTable.Columns.Add("DocNum", typeof(int));
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					ApzObjectTypes objtype = (ApzObjectTypes)Function.ParseInt(dataTable.Rows[i]["ObjectType"]);
					string docTableByObjectType = GetDocTableByObjectType(objtype);
					string query2 = "SELECT \"DocNum\" from " + docTableByObjectType + " WHERE \"DocEntry\"=@docEntry;";
					DataTable dataTable2 = ExecuteDataTable(query2, CommandType.Text, new SqlParameter("@docEntry", Function.ParseInt(dataTable.Rows[i]["DocEntry"])));
					if (dataTable2.IsNotNull())
					{
						int num = Function.ParseInt(dataTable2.Rows[0][0]);
						dataTable.Rows[i]["DocNum"] = num;
					}
				}
			}
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}
}
