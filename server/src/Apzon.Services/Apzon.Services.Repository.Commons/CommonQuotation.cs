using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class CommonQuotation : BaseService<DocumentsImpl>, ICommonQuotation
{
	public CommonQuotation(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDocuments(DocumentsImpl doc, string tblDoc)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "SELECT A.DocEntry,  A.Docnum,\r\n                                    A.CardCode,\r\n                                    A.CardName,\r\n                                    A.CntctCode as ContactPersonCode, B.Name as ContactPersonName,\r\n                                    A.DocStatus as DocumentStatus, \r\n                                    CASE\r\n                                            WHEN A.DocStatus ='O' then 'Open' \r\n                                            WHEN  A.DocStatus  = 'C' AND (A.Canceled ='N' OR A.Canceled ='' OR A.Canceled IS NULL) THEN 'Close'\r\n                                            WHEN A.DocStatus  = 'C' AND A.Canceled ='Y' then 'Cancel' \r\n                                        END AS DocStatusName,\r\n                                    A.DocDate,\r\n                                    A.DocDueDate,\r\n                                    A.TaxDate,\r\n                                    A.Comments,\r\n                                    A.NumAtCard,\r\n                                    A.DiscPrcnt as DiscountPercent,\r\n                                    A.VatSum,\r\n                                    A.DocTotal,\r\n                                    A.DocCur as DocCurrency,\r\n                                    A.DocRate,\r\n                                    A.Instance,\r\n                                    A.Segment,\r\n                                    A.PIndicator,\r\n                                    A.DocSubType,\r\n                                    A.Canceled,\r\n                                    A.ReqDate,\r\n                                ISNULL (SUB_Query.TotalBeforeDis,0) AS TotalBeforeDis,\r\n                                  isnull( A.DocTotalFc,0) as DocTotalFc, isnull(A.VatSumFc,0) as VatSumFc\r\n                                , Case when isnull(A.DocTotalFc,0)>0 then isnull(A.DocTotalFc,0) else isnull(A.DocTotal,0) end Total\r\n                                ,case when isnull(A.VatSumFC,0)>0 then isnull(A.VatSumFC,0) else isnull(A.VatSum,0) end TaxTotal\r\n                                FROM " + tblDoc + " A LEFT JOIN OCPR B ON A.CardCode = B.CardCode AND A.CntctCode = B.CntctCode LEFT JOIN (SELECT SUM(A.LineTotal) AS TotalBeforeDis,B.DocEntry FROM QUT1 A INNER JOIN " + tblDoc + " B ON A.DocEntry = B.DocEntry GROUP BY B.DocEntry )SUB_Query ON A.DocEntry = SUB_Query.DocEntry  WHERE 1 = 1";
			text += "       ORDER BY A.DocEntry DESC ";
			result = ExecuteData(text, CommandType.Text, (SqlParameter[])null).Tables[0];
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " error Doc ", ex.Message);
			return result;
		}
	}

	public DataTable GetDocLines(int docEntry, string tblLine)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "SELECT A.DocEntry, \r\n                                            A.LineNum, \r\n                                            A.ItemCode, \r\n                                            A.Dscription as ItemDescription,\r\n                                            A.Quantity, \r\n                                            A. Price,\r\n                                            A.DiscPrcnt as DiscountPercent, \r\n                                            A.LineTotal,\r\n                                            A.TaxCode,\r\n                                            A.UomCode,\r\n                                            ISNULL(A.PQTReqdate,''),\r\n                                            A.WhsCode as WarehouseCode,\r\n                                            A.FreeTxt as Text,\r\n                                            ISNULL( A.OpenQty,0) as RemainingOpenQuantity,\r\n                                            A.Currency,\r\n                                            ISNULL( A.Rate,0)   Rate,\r\n                                            ISNULL( A.LineVat,0)   LineVat,\r\n                                            ISNULL( B.Rate,0) as TaxPercentagePerRow,\r\n                                            LineStatus as LineStatusName,\r\n                                            isnull ( A.TotalFrgn,0) as TotalFrgn ,isnull ( A.VatSumFrgn,0) as VatSumFrgn                                                                                                                      \r\n                            FROM " + tblLine + " A\r\n                            LEFT JOIN OVTG B\r\n                            ON B.Code = A.TaxCode\r\n                            WHERE 1 = 1 and  docentry='" + docEntry + "'";
			result = ExecuteData(query, CommandType.Text, (SqlParameter[])null).Tables[0];
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " error Lines ", ex.Message);
			return result;
		}
	}

	public bool CheckDoc(int docEntry, string tblDoc)
	{
		try
		{
			string query = "SELECT   A.DocEntry \r\n                            FROM " + tblDoc + " A WHERE A.DocEntry = '" + docEntry + "'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public bool CheckDocLine(int docEntry, int lineNum, string tblLine)
	{
		try
		{
			string text = "SELECT *\r\n                            FROM " + tblLine + " WHERE DocEntry = '" + docEntry + "'";
			text = text + " AND LineNum = '" + lineNum + "'";
			DataTable dataTable = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public string SaveDoc(DocumentsImpl doc, string tblDoc, string tblLine)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			string result = string.Empty;
			if (doc.CardCode.Length > 0 && doc.CardName.Length > 0)
			{
				try
				{
					bool flag = CheckDoc(doc.DocNum, tblDoc);
					string empty = string.Empty;
					List<SqlParameter> list = new List<SqlParameter>();
					if (!flag)
					{
						list = new List<SqlParameter>();
						empty = "INSERT INTO " + tblDoc + " (DocEntry,  Docnum,\r\n                                    CardCode,\r\n                                    CardName,\r\n                                    CntctCode,\r\n                                    DocStatus,\r\n                                    TaxDate,\r\n                                    DocDate,\r\n                                    DocDueDate,\r\n                                    NumAtCard,\r\n                                    ReqDate,\r\n                                    Comments,\r\n                                    DiscPrcnt,\r\n                                    VatSum,\r\n                                    DocTotal,\r\n                                    DocCur,\r\n                                    DocRate,\r\n                                    Instance,\r\n                                    Segment,\r\n                                    PIndicator,\r\n                                    DocSubType,Canceled,DocTotalFc,VatSumFc ) VALUES (";
						empty += " @docEntry ";
						list.Add(new SqlParameter("@docEntry", doc.DocEntry));
						empty += ", @docNum";
						list.Add(new SqlParameter("@docNum", doc.DocNum));
						empty += ", @cardCode";
						list.Add(new SqlParameter("@cardCode", doc.CardCode));
						empty += ", @cardName";
						list.Add(new SqlParameter("@cardName", doc.CardName));
						empty += ", @cntctCode";
						list.Add(new SqlParameter("@cntctCode", doc.CntctCode));
						empty += ", @docStatus";
						list.Add(new SqlParameter("@docStatus", doc.DocStatus));
						empty += ", @taxDate";
						list.Add(new SqlParameter("@taxDate", doc.TaxDate.Date));
						empty += ", @docDate";
						list.Add(new SqlParameter("@docDate", doc.DocDate.Date));
						empty += ", @docDueDate";
						list.Add(new SqlParameter("@docDueDate", doc.DocDueDate.Date));
						empty += ", @numAtCard";
						list.Add(new SqlParameter("@numAtCard", Function.ToString(doc.NumAtCard)));
						empty += ", @reqDate";
						list.Add(new SqlParameter("@reqDate", Function.ParseDateTimes(doc.ReqDate.Date)));
						empty += ", @comments";
						list.Add(new SqlParameter("@comments", Function.ToString(doc.Comments)));
						empty += ", @discPrcnt";
						list.Add(new SqlParameter("@discPrcnt", Function.ToString(doc.DiscPrcnt)));
						empty += ", @vatSum";
						list.Add(new SqlParameter("@vatSum", doc.VatSum));
						empty += ", @docTotal";
						list.Add(new SqlParameter("@docTotal", Function.ParseDouble(doc.DocTotal)));
						empty += ", @docCur";
						list.Add(new SqlParameter("@docCur", doc.DocCur));
						empty += ", @docRate";
						list.Add(new SqlParameter("@docRate", doc.DocRate));
						empty += ", 0";
						empty += ", 0";
						empty += ", 'Default'";
						empty += ", '--'";
						empty += ",@canceled";
						list.Add(new SqlParameter("@canceled", Function.ToString(doc.Canceled)));
						empty += ", @DocTotalFc ";
						list.Add(new SqlParameter("@DocTotalFc", Function.ParseDouble(doc.DocTotalFc)));
						empty += ", @VatSumFc ";
						list.Add(new SqlParameter("@VatSumFc", Function.ParseDouble(doc.VatSumFc)));
						empty += " ) ";
					}
					else
					{
						list = new List<SqlParameter>();
						empty = "UPDATE " + tblDoc + " SET ";
						empty += "CardCode = @cardCode";
						list.Add(new SqlParameter("@cardCode", doc.CardCode));
						empty += ", CardName = @cardName";
						list.Add(new SqlParameter("@cardName", doc.CardName));
						empty += ",CntctCode = @cntctCode";
						list.Add(new SqlParameter("@cntctCode", doc.CntctCode));
						empty += ", DocStatus = @docStatus";
						list.Add(new SqlParameter("@docStatus", doc.DocStatus));
						empty += ", TaxDate = @taxDate ";
						list.Add(new SqlParameter("@taxDate", doc.TaxDate));
						empty += ", DocDueDate= @docDueDate";
						list.Add(new SqlParameter("@docDueDate", doc.DocDueDate));
						empty += ",NumAtCard= @numAtCard ";
						list.Add(new SqlParameter("@numAtCard", Function.ParseInt(doc.NumAtCard)));
						empty += ", ReqDate= @reqDate ";
						list.Add(new SqlParameter("@reqDate", doc.ReqDate.Date));
						empty += ", Comments =@comments";
						list.Add(new SqlParameter("@comments", Function.ToString(doc.Comments)));
						empty += ", DocDate=  @docDate";
						list.Add(new SqlParameter("@docDate", doc.DocDate));
						empty += ",DiscPrcnt = @discPrcnt";
						list.Add(new SqlParameter("@discPrcnt", Function.ParseDouble(doc.DiscPrcnt)));
						empty += ", VatSum=@vatSum";
						list.Add(new SqlParameter("@vatSum", Function.ParseDouble(doc.VatSum)));
						empty += ",DocTotal = @docTotal";
						list.Add(new SqlParameter("@docTotal", Function.ParseDouble(doc.DocTotal)));
						empty += ",DocCur = @docCur";
						list.Add(new SqlParameter("@docCur", Function.ToString(doc.DocCur)));
						empty += ",DocRate = @docRate";
						list.Add(new SqlParameter("@docRate", Function.ParseDouble(doc.DocRate)));
						empty += ", instance = 0";
						empty += ",segment = 0";
						empty += ",pIndicator ='Default'";
						empty += ",docSubType = '--'";
						empty += ",Canceled = @canceled";
						list.Add(new SqlParameter("@canceled", Function.ToString(doc.Canceled)));
						empty += ",  DocTotalFc=@DocTotalFc ";
						list.Add(new SqlParameter("@DocTotalFc", Function.ParseDouble(doc.DocTotalFc)));
						empty += ", VatSumFc=@VatSumFc ";
						list.Add(new SqlParameter("@VatSumFc", Function.ParseDouble(doc.VatSumFc)));
						empty += " where DocNum = @docNum";
						list.Add(new SqlParameter("@docNum", doc.DocNum));
					}
					int num = ExecuteNonQuery(empty, CommandType.Text, list.ToArray());
					int num2 = 0;
					int num3 = 0;
					if (num > 0 && doc.Lines != null)
					{
						foreach (Document_LinesImpl line in doc.Lines)
						{
							if (string.IsNullOrEmpty(SaveDocLine(line, doc, tblLine)))
							{
								if (doc.ObjTypeFrom > 0)
								{
									num2 = line.BaseEntry;
									num3 = DealLineOrgin(doc.TypeTo, doc.TypeFrom, num2, line.BaseLine);
									if (num3 < 0)
									{
										Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Close Line Base On " + line.BaseEntry + " FAILED " + line.BaseLine, "Vượt quá Open Qty Line : " + num3);
										dbTransaction.Rollback();
										return $"Vượt quá Open Qty Line {line.BaseLine}: {Math.Abs(num3)} ";
									}
								}
								if (doc.ObjTypeFrom > 0 && num2 > 0 && num3 >= 0)
								{
									int num4 = DealDocOrgin(doc.TypeFrom, num2);
									if (num4 < 0)
									{
										Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Close Doc From " + num2 + ". FAILED ", "Vượt quá Open Qty Doc" + num4);
										dbTransaction.Rollback();
										return "Vượt quá Open Qty Doc: " + Math.Abs(num4);
									}
								}
							}
							else
							{
								dbTransaction.Rollback();
							}
						}
					}
					if (num > 0)
					{
						dbTransaction.Commit();
						return null;
					}
					return "ERROR";
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
					result = ex.Message;
				}
			}
			return result;
		}
	}

	public string SaveDocLine(Document_LinesImpl line, DocumentsImpl doc, string tblLine)
	{
		_ = string.Empty;
		try
		{
			bool flag = CheckDocLine(doc.DocEntry, line.LineNum, tblLine);
			if (line.ItemCode.Length > 0 && line.Quantity > 0.0)
			{
				string empty = string.Empty;
				List<SqlParameter> list = new List<SqlParameter>();
				if (!flag)
				{
					empty = "INSERT INTO " + tblLine + "( DocEntry, \r\n\t\t                                LineNum , \r\n                                        ItemCode ,\r\n                                        Dscription,\r\n\t\t                                Quantity, \r\n\t\t                                Price,\r\n                                        UomCode,\r\n                                        PQTReqdate,\r\n\t\t                                DiscPrcnt, \r\n                                        TaxCode,\r\n                                        LineTotal,\r\n                                        WhsCode,\r\n                                        FreeTxt,\r\n                                        OpenQty,\r\n                                        Currency,\r\n                                        Rate,\r\n                                        LineVat, LineStatus,TotalFrgn,VatSumFrgn)   VALUES ( ";
					empty += " @docEntry ";
					list.Add(new SqlParameter("@docEntry", Function.ToString(doc.DocEntry)));
					empty += ", @lineNum";
					list.Add(new SqlParameter("@lineNum", Function.ToString(line.LineNum)));
					empty += ", @itemCode";
					list.Add(new SqlParameter("@itemCode", Function.ToString(line.ItemCode)));
					empty += ", @itemName";
					list.Add(new SqlParameter("@itemName", Function.ToString(line.Dscription)));
					empty += ", @quantity";
					list.Add(new SqlParameter("@quantity", Function.ToString(line.Quantity)));
					empty += ", @price";
					list.Add(new SqlParameter("@price", Function.ToString(line.Price)));
					empty += ", @uomCode";
					list.Add(new SqlParameter("@uomCode", Function.ToString(line.UomCode)));
					empty += ", @pQTReqdate";
					list.Add(new SqlParameter("@pQTReqdate", Function.ParseDateTimes(line.PQTReqDate)));
					empty += ", @discPrcnt";
					list.Add(new SqlParameter("@discPrcnt", Function.ToString(line.DiscPrcnt)));
					empty += ", @taxCode";
					list.Add(new SqlParameter("@taxCode", Function.ToString(line.TaxCode)));
					empty += ", @lineTotal";
					list.Add(new SqlParameter("@lineTotal", Function.ToString(line.LineTotal)));
					empty += ", @whsCode";
					list.Add(new SqlParameter("@whsCode", Function.ToString(line.WhsCode)));
					empty += ", @freeTxt";
					list.Add(new SqlParameter("@freeTxt", Function.ToString(line.Text)));
					empty += ", @openQty";
					list.Add(new SqlParameter("@openQty", Function.ToString(line.RemainingOpenQuantity)));
					empty += ", @currency";
					list.Add(new SqlParameter("@currency", Function.ToString(line.Currency)));
					empty += ", @rate";
					list.Add(new SqlParameter("@rate", Function.ToString(line.Rate)));
					empty += ",@lineVat";
					list.Add(new SqlParameter("@lineVat", Function.ToString(line.LineVat)));
					empty += ",@lineStatus";
					list.Add(new SqlParameter("@lineStatus", Function.ToString(line.LineStatus)));
					empty += ", @TotalFrgn ";
					list.Add(new SqlParameter("@TotalFrgn", Function.ParseDouble(line.TotalFrgn)));
					empty += ",@VatSumFrgn ";
					list.Add(new SqlParameter("@VatSumFrgn", Function.ParseDecimal(line.VatSumFrgn)));
					empty += " ) ";
				}
				else
				{
					list = new List<SqlParameter>();
					empty = "UPDATE " + tblLine + " SET ";
					empty += " ItemCode = @itemCode";
					list.Add(new SqlParameter("@itemCode", Function.ToString(line.ItemCode)));
					empty += ", Dscription = @dscription";
					list.Add(new SqlParameter("@dscription", Function.ToString(line.Dscription)));
					empty += ", Quantity = @quantity";
					list.Add(new SqlParameter("@quantity", Function.ToString(line.Quantity)));
					empty += ", Price = @price";
					list.Add(new SqlParameter("@price", Function.ToString(line.Price)));
					empty += ", DiscPrcnt  = @discPrcnt ";
					list.Add(new SqlParameter("@discPrcnt ", Function.ToString(line.DiscPrcnt)));
					empty += ", TaxCode = @taxCode";
					list.Add(new SqlParameter("@taxCode", Function.ToString(line.TaxCode)));
					empty += ", LineTotal = @lineTotal";
					list.Add(new SqlParameter("@lineTotal", Function.ToString(line.LineTotal)));
					empty += ", WhsCode = @whsCode";
					list.Add(new SqlParameter("@whsCode", Function.ToString(line.WhsCode)));
					empty += ", FreeTxt = @freeTxt";
					list.Add(new SqlParameter("@freeTxt", Function.ToString(line.Text)));
					empty += ", OpenQty = @openQty";
					list.Add(new SqlParameter("@openQty", Function.ToString(line.RemainingOpenQuantity)));
					empty += ", Currency = @currency";
					list.Add(new SqlParameter("@currency", Function.ToString(line.Currency)));
					empty += ", Rate = @rate";
					list.Add(new SqlParameter("@rate", Function.ToString(line.Rate)));
					empty += ", LineVat = @lineVat";
					list.Add(new SqlParameter("@lineVat", Function.ToString(line.LineVat)));
					empty += ", LineStatus = @lineStatus";
					list.Add(new SqlParameter("@lineStatus", Function.ToString(line.LineStatus)));
					empty += ", TotalFrgn= @TotalFrgn ";
					list.Add(new SqlParameter("@TotalFrgn", Function.ParseDouble(line.TotalFrgn)));
					empty += ", VatSumFrgn=@VatSumFrgn ";
					list.Add(new SqlParameter("@VatSumFrgn", Function.ParseDecimal(line.VatSumFrgn)));
					empty += " where LineNum = @lineNum and DocEntry = @docEntry";
					list.Add(new SqlParameter("@lineNum", line.LineNum));
					list.Add(new SqlParameter("@docEntry", doc.DocEntry));
				}
				if (ExecuteNonQuery(empty, CommandType.Text, list.ToArray()) > 0)
				{
					return null;
				}
				return "ERROR";
			}
			return null;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public DataTable Search(DocumentsImpl doc, string tblDoc, string tblLine)
	{
		new DataTable();
		try
		{
			new List<SqlParameter>();
			string text = "SELECT A.DocEntry,  A.Docnum,\r\n                                    A.CardCode,\r\n                                    A.CardName,\r\n                                    A.CntctCode as ContactPersonCode, B.Name as ContactPersonName,\r\n                                    A.DocStatus as DocumentStatus, \r\n                                    CASE\r\n                                            WHEN A.DocStatus ='O' then 'Open' \r\n                                            WHEN  A.DocStatus  = 'C' AND (A.Canceled ='N' OR A.Canceled ='' OR A.Canceled IS NULL) THEN 'Close'\r\n                                            WHEN A.DocStatus  = 'C' AND A.Canceled ='Y' then 'Cancel' \r\n                                        END AS DocStatusName,\r\n                                    A.DocDate,\r\n                                    A.DocDueDate,\r\n                                    A.TaxDate,\r\n                                    ISNULL(A.Comments,'') AS Comments,\r\n                                    A.DiscPrcnt as DiscountPercent,\r\n                                    A.VatSum,\r\n                                    A.DocTotal,\r\n                                    A.DocCur as DocCurrency,\r\n                                    A.DocRate,\r\n                                    A.NumAtCard,\r\n                                    A.Instance,\r\n                                    A.Segment,\r\n                                    A.PIndicator,\r\n                                    A.DocSubType,\r\n                                    A.Canceled,\r\n                                   ISNULL( A.ReqDate, getDate()) AS RequriedDate,\r\n                                ISNULL (SUB_Query.TotalBeforeDis,0) AS TotalBeforeDis,\r\n                                      isnull( A.DocTotalFc,0) as DocTotalFc, isnull(A.VatSumFc,0) as VatSumFc\r\n                                FROM " + tblDoc + " A LEFT JOIN OCPR B ON A.CardCode = B.CardCode AND A.CntctCode = B.CntctCode LEFT JOIN (SELECT SUM(A.LineTotal) AS TotalBeforeDis,B.DocEntry FROM " + tblLine + " A INNER JOIN " + tblDoc + " B ON A.DocEntry = B.DocEntry GROUP BY B.DocEntry )SUB_Query ON A.DocEntry = SUB_Query.DocEntry  WHERE 1 = 1";
			if (doc.DocEntry == 0)
			{
				_ = doc.DocDate;
				if (doc.DocDate != DateTime.MinValue)
				{
					text = text + " AND A.DocDate >= '" + doc.DocDate.ToString() + "'";
				}
				_ = doc.DocDueDate;
				if (doc.DocDueDate != DateTime.MinValue)
				{
					text = text + "AND A.DocDate <= '" + doc.DocDueDate.ToString() + "' ";
				}
				if (!string.IsNullOrEmpty(doc.DocStatus) && doc.Canceled != "Y")
				{
					text = text + " AND A.DocStatus like '%" + doc.DocStatus + "%' ";
					text += " AND A.Canceled !='Y' ";
				}
				if (doc.Canceled == "Y")
				{
					text += " AND A.Canceled like 'Y' ";
				}
				if (!string.IsNullOrEmpty(doc.CardCode))
				{
					text = text + "AND A.CardCode like '%" + doc.CardCode + "%' ";
				}
			}
			else
			{
				text = text + " AND A.DocEntry = '" + doc.DocEntry + "' ";
			}
			text += "  ORDER BY A.DocEntry DESC  ";
			return ExecuteData(text, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception)
		{
			return null;
		}
	}

	public int GetMaxDocEntry(string tblDoc)
	{
		try
		{
			string query = "SELECT   MAX(DocEntry ) as MaxDocEntry\r\n                            FROM " + tblDoc + " ";
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

	public string DeleteDoc(int docEntry, string tblDoc, string tblLine)
	{
		try
		{
			string query = " delete " + tblDoc + " where DocEntry = '" + docEntry + "' delete " + tblLine + " where DocEntry = '" + docEntry + "'";
			if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public string DeleteLine(int docEntry, int lineNum, string tblLine)
	{
		try
		{
			string query = "delete from " + tblLine + " where DocEntry = '" + docEntry + "' and LineNum = '" + lineNum + "'";
			if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public DataTable ContactPerson(string cardCode)
	{
		try
		{
			new DataTable();
			string text = "SELECT   CntctCode as InternalCode,\r\n\t\t                                Name as Name ,\r\n                                        CardCode \r\n                             FROM OCPR\r\n                                UNION ALL\r\n                                SELECT -1,'',''  \r\n                                WHERE 1=1\r\n                                order by  CntctCode ";
			if (!string.IsNullOrEmpty(cardCode))
			{
				text = text + " AND CardCode ='" + cardCode + "'";
			}
			return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	public DataTable TaxCode()
	{
		try
		{
			new DataTable();
			string query = "select Code, \r\n                                    Name, \r\n                                    Rate \r\n                                from OVTG where 1 = 1";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	public string Canncel(int docEntry, string tblDoc, string tblLine)
	{
		string empty = string.Empty;
		try
		{
			string query = string.Format("UPDATE {0} SET Canceled='{1}',DocStatus='{2}' where DocEntry={3}", tblDoc, "Y", "C", docEntry);
			if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) >= 0)
			{
				string.Format("UPDATE {0} SET LineStatus='{1}' where DocEntry={2}", tblLine, "C", docEntry);
				if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
				{
					return null;
				}
				return "Error cancel line";
			}
			return "Error cancel";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public string Close(int docEntry, string tblDoc, string tblLine)
	{
		string empty = string.Empty;
		try
		{
			string query = string.Format("UPDATE {0} SET  DocStatus='{1}' where DocEntry={2}", tblDoc, "C", docEntry);
			if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) >= 0)
			{
				string.Format("UPDATE {0} SET LineStatus='{1}' where DocEntry={2}", tblLine, "C", docEntry);
				if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
				{
					return null;
				}
				return "Error Close line";
			}
			return "Error Close";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public string CloseLine(int lineNum, int docEntry, string tblLine)
	{
		string empty = string.Empty;
		try
		{
			string query = string.Format("UPDATE {0} SET  LineStatus = '{1}' WHERE LineNum = {2} AND DocEntry = {3}", tblLine, "C", lineNum, docEntry);
			if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
			{
				return null;
			}
			return "Error Close Lines";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public int GetQuantityBase(string tbl, string tblLine, int baseEntry, int baseLine)
	{
		try
		{
			string empty = string.Empty;
			empty = "SELECT B.BaseEntry ";
			if (baseLine != int.MinValue)
			{
				empty += " , B.BaseLine";
			}
			empty = empty + ", SUM( B.Quantity) AS Quantity\r\n                    FROM " + tbl + " A";
			empty = empty + " LEFT JOIN  " + tblLine + " B ON A.DocEntry = B.DocEntry";
			empty += " WHERE 1=1 AND B.LineStatus != 'C'";
			if (baseEntry != 0)
			{
				empty = empty + " AND B.BaseEntry='" + baseEntry + "'";
			}
			if (baseLine != int.MinValue)
			{
				empty = empty + " AND  B.BaseLine =N'" + baseLine + "'";
			}
			empty += " GROUP BY B.BaseEntry";
			if (baseLine != int.MinValue)
			{
				empty += " , B.BaseLine";
			}
			return Convert.ToInt32(ExecuteDataTable(empty, CommandType.Text, (SqlParameter[])null).Rows[0]["Quantity"]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetQuantityBase  FAILED ", ex.Message);
			return 0;
		}
	}

	public int GetOpenQuantity(string tbl, string tblLine, int docEntry, int lineNum)
	{
		try
		{
			string empty = string.Empty;
			empty = "SELECT a.DocEntry ";
			if (lineNum != int.MinValue)
			{
				empty += " , B.LineNum";
			}
			empty = empty + ", SUM( B.OpenQty) AS OpenQty\r\n                    FROM " + tbl + " A";
			empty = empty + " LEFT JOIN  " + tblLine + " B ON A.DocEntry = B.DocEntry";
			empty += " WHERE 1=1 ";
			empty += " AND B.LineStatus != 'C'";
			if (docEntry != 0)
			{
				empty = empty + " AND A.DocEntry='" + docEntry + "'";
			}
			if (lineNum != int.MinValue)
			{
				empty = empty + " AND  B.LineNum ='" + lineNum + "'";
			}
			empty += " GROUP BY A.DocEntry";
			if (lineNum != int.MinValue)
			{
				empty += " , B.LineNum";
			}
			return Convert.ToInt32(ExecuteDataTable(empty, CommandType.Text, (SqlParameter[])null).Rows[0]["OpenQty"]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetOpenQuantity  FAILED ", ex.Message);
			return 0;
		}
	}

	public int DealDocOrgin(ApzObjectTypes typeFrom, int baseEntry)
	{
		try
		{
			List<string> list = new List<string>();
			list = GetTableName(typeFrom);
			int openQuantity = GetOpenQuantity(list[0], list[1], baseEntry, int.MinValue);
			string empty = string.Empty;
			if (openQuantity == 0)
			{
				empty = $" UPDATE {list[0]} \r\n                                        SET DocStatus = 'C'\r\n                                        WHERE DocEntry = '{baseEntry}'\r\n                                        ";
				ExecuteNonQuery(empty, CommandType.Text, (SqlParameter[])null);
			}
			return openQuantity;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CloseDocOrgin  FAILED ", ex.Message);
			return int.MinValue;
		}
	}

	public int DealLineOrgin(ApzObjectTypes typeTo, ApzObjectTypes typeFrom, int baseEntry, int baseLine)
	{
		try
		{
			string text = string.Empty;
			List<string> list = new List<string>();
			list = GetTableName(typeTo);
			int quantityBase = GetQuantityBase(list[0], list[1], baseEntry, baseLine);
			List<string> list2 = new List<string>();
			list2 = GetTableName(typeFrom);
			int num = GetOpenQuantity(list2[0], list2[1], baseEntry, baseLine) - quantityBase;
			if (num <= 0)
			{
				text = $" UPDATE {list2[1]} \r\n                                        SET LineStatus = 'C'\r\n                                        WHERE DocEntry = '{baseEntry}' AND LineNum ='{baseLine}'\r\n                                        ";
			}
			text += string.Format(" UPDATE {0} \r\n                                SET OpenQty = '" + num + "'\r\n                                WHERE DocEntry = '{1}' AND LineNum ='{2}'\r\n                                ", list2[1], baseEntry, baseLine);
			ExecuteNonQuery(text, CommandType.Text, (SqlParameter[])null);
			return num;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CloseLineOrgin  FAILED ", ex.Message);
			return int.MinValue;
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
			item = "OPOR";
			item2 = "POR1";
			break;
		case ApzObjectTypes.oPurchaseRequest:
			item = "OPRQ";
			item2 = "PRQ1";
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
		case ApzObjectTypes.oDrafts:
			item = "ODRF";
			item2 = "DRF1";
			break;
		case ApzObjectTypes.oInventoryGenEntry:
			item = "OIGN";
			item2 = "IGN1";
			break;
		case ApzObjectTypes.oInventoryGenExit:
			item = "OIGE";
			item2 = "IGE1";
			break;
		}
		list.Add(item);
		list.Add(item2);
		return list;
	}
}
