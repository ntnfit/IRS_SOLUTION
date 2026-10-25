using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Inventory;
using Apzon.Services.Interfaces.Inventory.InventoryTransaction;

namespace Apzon.Services.Repository.Inventory.InventoryTransaction;

public class InventoryRevaluationRespository : BaseService<InventoryRevaluationImpl>, IInventoryRevaluation
{
	public InventoryRevaluationRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public int GetMaxDocEntry()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(DocEntry ),0) as MaxDocEntry\r\n                            FROM OMRV ";
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

	public int GetMaxLineNum()
	{
		throw new NotImplementedException();
	}

	public bool CheckDoc(int docEntry)
	{
		try
		{
			string query = "SELECT   A.DocEntry \r\n                            FROM OMRV A \r\n                            WHERE A.DocEntry = '" + docEntry + "'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
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

	public bool CheckDocLineMRV1(int docEntry, int lineNum)
	{
		try
		{
			string text = "SELECT *\r\n                            FROM MRV1\r\n                            WHERE DocEntry = '" + docEntry + "'";
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

	public bool CheckDocLineMRV2(int docEntry, int lineNum)
	{
		try
		{
			string text = "SELECT *\r\n                            FROM MRV2\r\n                            WHERE DocEntry = '" + docEntry + "'";
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

	public DataTable Search(InventoryRevaluationImpl doc)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "\r\n                                select a.DocEntry,a.TaxDate,a.DocDate ,\r\n                                case when a.RevalType='P' then 'Price Change'\r\n\t                                 when a.RevalType='M' then N'Inventory Debit/Credit'\t\r\n                                end RevalType,\r\n\t                                isnull (a.Comments,'')as Comments ,a.JrnlMemo\r\n\t                            from OMRV a    \r\n                                WHERE 1=1 ";
			if (doc.DocEntry == 0)
			{
				if (doc.DocDate.Date.ToString("dd/MM/yyyy") != "01/01/0001")
				{
					text = text + " AND a.DocDate >=convert(datetime, '" + doc.DocDate.Date.ToString("dd/MM/yyyy") + "', 103)";
				}
				if (doc.TaxDate.Date.ToString("dd/MM/yyyy") != "01/01/0001")
				{
					text = text + " AND a.TaxDate <=convert(datetime, '" + doc.TaxDate.Date.ToString("dd/MM/yyyy") + "', 103)";
				}
			}
			else
			{
				text = text + "AND a.DocEntry = '" + doc.DocEntry + "' ";
			}
			result = ExecuteData(text, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Return Search - Doc ", ex.Message);
		}
		return result;
	}

	public DataTable SearchLineMRV1(int idDocEntry)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "  select isnull(a.DocEntry,0) as DocEntry, isnull (a.LineNum,0) as LineNum, a.ItemCode,a.Dscription,a.RActPrice,a.Price,\r\n\t\t\t                             a.ROnHand,a.Quantity,a.RIncmAcct,a.RDcrmAcct,a.WhsCode,a.EvalSystem\r\n\t\t\t\t                  from mrv1 a\r\n                                  WHERE 1=1 ";
			text = text + "AND a.DocEntry = '" + idDocEntry + "' ";
			result = ExecuteData(text, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Return Search - Doc ", ex.Message);
		}
		return result;
	}

	public DataTable SearchLineMRV2(int docEntry)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "select ISNULL( a.DocEntry,0) as DocEntry, isnull(a.LineNum,0) as LineNum, isnull(a.INMCreatBy,'') as INMCreatBy,\r\n\t\t                                case when  a.INMDocDate IS NULL then GETDATE()\r\n\t\t                                else a.INMDocDate \r\n\t\t                                end INMDocDate,\r\n                                     ISNULL( a.RActPrice,0)as RActPrice,  ISNULL( a.Price,0) as Price , isnull(a.Quantity,0) as Quantity , ISNULL( a.INMOpenQty,0) as INMOpenQty\r\n                                 from MRV2 a \r\n                                 WHERE 1=1 ";
			text = text + "AND a.DocEntry = '" + docEntry + "' ";
			result = ExecuteData(text, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Return Search - Doc ", ex.Message);
		}
		return result;
	}

	public string SaveLineMRV1(InventoryRevaluation_LineMRV1Impl line)
	{
		string empty = string.Empty;
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			bool flag = CheckDocLineMRV1(line.DocEntry, line.LineNum);
			string empty2 = string.Empty;
			if (!flag)
			{
				empty2 = "INSERT INTO MRV1 ( DocEntry,LineNum,ItemCode,Dscription, RActPrice, Price,ROnHand,Quantity,RIncmAcct,RDcrmAcct,WhsCode,EvalSystem)\r\n                             VALUES (";
				empty2 += " @docEntry ";
				list.Add(new SqlParameter("@docEntry", Function.ParseInt(line.DocEntry)));
				empty2 += ", @lineNum ";
				list.Add(new SqlParameter("@lineNum", Function.ParseInt(line.LineNum)));
				empty2 += ", @itemCode ";
				list.Add(new SqlParameter("@itemCode", Function.ToString(line.ItemCode)));
				empty2 += ", @dscription ";
				list.Add(new SqlParameter("@dscription", Function.ToString(line.Dscription)));
				empty2 += ", @rActPrice ";
				list.Add(new SqlParameter("@rActPrice", Function.ParseDouble(line.RActPrice)));
				empty2 += ", @price ";
				list.Add(new SqlParameter("@price", Function.ParseDouble(line.Price)));
				empty2 += ", @rOnHand ";
				list.Add(new SqlParameter("@rOnHand", Function.ParseDouble(line.ROnHand)));
				empty2 += ", @quantity ";
				list.Add(new SqlParameter("@quantity", Function.ParseDouble(line.Quantity)));
				empty2 += ",@rIncmAcct ";
				list.Add(new SqlParameter("@rIncmAcct", Function.ToString(line.RIncmAcct)));
				empty2 += ", @rDcrmAcct ";
				list.Add(new SqlParameter("@rDcrmAcct", Function.ToString(line.RDcrmAcct)));
				empty2 += ", @whsCode ";
				list.Add(new SqlParameter("@whsCode", Function.ToString(line.WhsCode)));
				empty2 += ", @evalSystem ";
				list.Add(new SqlParameter("@evalSystem", Function.ToString(line.EvalSystem)));
				empty2 += " )";
			}
			else
			{
				list = new List<SqlParameter>();
				empty2 = "Update MRV1 SET ";
				empty2 += "ItemCode= @itemCode ";
				list.Add(new SqlParameter("@itemCode", Function.ToString(line.ItemCode)));
				empty2 += ",Dscription= @dscription ";
				list.Add(new SqlParameter("@dscription", Function.ToString(line.Dscription)));
				empty2 += ",RActPrice= @rActPrice ";
				list.Add(new SqlParameter("@rActPrice", Function.ParseDouble(line.RActPrice)));
				empty2 += ", Price=@price ";
				list.Add(new SqlParameter("@price", Function.ParseDouble(line.Price)));
				empty2 += ",ROnHand= @rOnHand ";
				list.Add(new SqlParameter("@rOnHand", Function.ParseDouble(line.ROnHand)));
				empty2 += ",Quantity= @quantity ";
				list.Add(new SqlParameter("@quantity", Function.ParseDouble(line.Quantity)));
				empty2 += ",RIncmAcct=@rIncmAcct ";
				list.Add(new SqlParameter("@rIncmAcct", Function.ToString(line.RIncmAcct)));
				empty2 += ",RDcrmAcct= @rDcrmAcct ";
				list.Add(new SqlParameter("@rDcrmAcct", Function.ToString(line.RDcrmAcct)));
				empty2 += ",WhsCode= @whsCode ";
				list.Add(new SqlParameter("@whsCode", Function.ToString(line.WhsCode)));
				empty2 += ",EvalSystem= @evalSystem ";
				list.Add(new SqlParameter("@evalSystem", Function.ToString(line.EvalSystem)));
				empty2 += " WHERE DocEntry =@docEntry";
				list.Add(new SqlParameter("@docEntry", line.DocEntry));
				empty2 += " AND LineNum = @lineNum";
				list.Add(new SqlParameter("@lineNum", line.LineNum));
			}
			if (ExecuteNonQuery(empty2, CommandType.Text, list.ToArray()) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC LINE  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveLineMRV2(InventoryRevaluation_LineMRV2Impl line)
	{
		string empty = string.Empty;
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			bool flag = CheckDocLineMRV2(line.DocEntry, line.LineNum);
			string empty2 = string.Empty;
			if (!flag)
			{
				empty2 = "INSERT INTO MRV2 ( DocEntry,LineNum,INMCreatBy,INMDocDate, RActPrice, Price, Quantity,INMOpenQty)\r\n                             VALUES (";
				empty2 += " @docEntry ";
				list.Add(new SqlParameter("@docEntry", Function.ParseInt(line.DocEntry)));
				empty2 += ", @lineNum ";
				list.Add(new SqlParameter("@lineNum", Function.ParseInt(line.LineNum)));
				empty2 += ", @iNMCreatBy ";
				list.Add(new SqlParameter("@iNMCreatBy", Function.ToString(line.INMCreatBy)));
				empty2 += ", @iNMDocDate ";
				list.Add(new SqlParameter("@iNMDocDate", Function.ToString(line.INMDocDate)));
				empty2 += ", @rActPrice ";
				list.Add(new SqlParameter("@rActPrice", Function.ParseDouble(line.RActPrice)));
				empty2 += ", @price ";
				list.Add(new SqlParameter("@price", Function.ParseDouble(line.Price)));
				empty2 += ", @quantity ";
				list.Add(new SqlParameter("@quantity", Function.ParseDouble(line.Quantity)));
				empty2 += ",@iNMOpenQty ";
				list.Add(new SqlParameter("@iNMOpenQty", Function.ParseDouble(line.INMOpenQty)));
				empty2 += " )";
			}
			else
			{
				list = new List<SqlParameter>();
				empty2 = "Update MRV2 SET ";
				empty2 += "INMCreatBy= @iNMCreatBy ";
				list.Add(new SqlParameter("@iNMCreatBy", Function.ToString(line.INMCreatBy)));
				empty2 += ",INMDocDate= @iNMDocDate ";
				list.Add(new SqlParameter("@iNMDocDate", Function.ToString(line.INMDocDate)));
				empty2 += ",RActPrice= @rActPrice ";
				list.Add(new SqlParameter("@rActPrice", Function.ParseDouble(line.RActPrice)));
				empty2 += ",Price= @price ";
				list.Add(new SqlParameter("@price", Function.ParseDouble(line.Price)));
				empty2 += ",Quantity= @quantity ";
				list.Add(new SqlParameter("@quantity", Function.ParseDouble(line.Quantity)));
				empty2 += ",INMOpenQty=@iNMOpenQty ";
				list.Add(new SqlParameter("@iNMOpenQty", Function.ParseDouble(line.INMOpenQty)));
				empty2 += " WHERE DocEntry =@docEntry";
				list.Add(new SqlParameter("@docEntry", line.DocEntry));
				empty2 += " AND LineNum = @lineNum";
				list.Add(new SqlParameter("@lineNum", line.LineNum));
			}
			if (ExecuteNonQuery(empty2, CommandType.Text, list.ToArray()) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC LINE  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteLineMRV2(int docEntry, int lineNum)
	{
		throw new NotImplementedException();
	}

	public string SaveDoc(InventoryRevaluationImpl doc)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			string empty = string.Empty;
			List<SqlParameter> list = new List<SqlParameter>();
			try
			{
				bool flag = CheckDoc(doc.DocNum);
				string empty2 = string.Empty;
				if (!flag)
				{
					empty2 = "INSERT INTO OMRV  (DocEntry , DocNum ,RevalType,DocDate,TaxDate,Comments,JrnlMemo,\r\n                                                  DataSource, Series,UserSign,ObjType )\r\n                                                     VALUES \r\n                                        ( ";
					empty2 += " @docEntry ";
					list.Add(new SqlParameter("@docEntry", Function.ParseInt(doc.DocEntry)));
					empty2 += ", @docNum ";
					list.Add(new SqlParameter("@docNum", Function.ParseInt(doc.DocNum)));
					empty2 += ", @revalType ";
					if (doc.RevalType == "Price Change")
					{
						doc.RevalType = "P";
					}
					else if (doc.RevalType == "Inventory Debit/Credit")
					{
						doc.RevalType = "M";
					}
					list.Add(new SqlParameter("@revalType", Function.ToString(doc.RevalType)));
					empty2 += ", @docDate ";
					list.Add(new SqlParameter("@docDate", doc.DocDate.Date));
					empty2 += ", @taxDate ";
					list.Add(new SqlParameter("@taxDate", doc.TaxDate.Date));
					empty2 += ", @comments ";
					list.Add(new SqlParameter("@comments", Function.ToString(doc.Comments)));
					empty2 += ", @jrnlMemo ";
					list.Add(new SqlParameter("@jrnlMemo", Function.ToString(doc.JrnlMemo)));
					empty2 += ", @dataSource";
					list.Add(new SqlParameter("@dataSource", Function.ToString(doc.DataSource)));
					empty2 += ", @series ";
					list.Add(new SqlParameter("@series", Function.ParseInt(doc.Series)));
					empty2 += ", @userSign ";
					list.Add(new SqlParameter("@userSign", Function.ParseInt(doc.UserSign)));
					empty2 += ", @objType ";
					list.Add(new SqlParameter("@objType", Function.ToString(doc.ObjType)));
					empty2 += " )";
				}
				else
				{
					list = new List<SqlParameter>();
					empty2 = "UPDATE OMRV  \r\n                            SET\r\n                                DocNum= ";
					empty2 += " @docNum ";
					list.Add(new SqlParameter("@docNum", Function.ParseInt(doc.DocNum)));
					empty2 += ",RevalType= @revalType ";
					if (doc.RevalType == "Price Change")
					{
						doc.RevalType = "P";
					}
					else if (doc.RevalType == "Inventory Debit/Credit")
					{
						doc.RevalType = "M";
					}
					list.Add(new SqlParameter("@revalType", Function.ToString(doc.RevalType)));
					empty2 += ",DocDate= @docDate ";
					list.Add(new SqlParameter("@docDate", doc.DocDate.Date));
					empty2 += ",TaxDate= @taxDate ";
					list.Add(new SqlParameter("@taxDate", doc.TaxDate.Date));
					empty2 += ",Comments= @comments ";
					list.Add(new SqlParameter("@comments", Function.ToString(doc.Comments)));
					empty2 += ",JrnlMemo= @jrnlMemo ";
					list.Add(new SqlParameter("@jrnlMemo", Function.ToString(doc.JrnlMemo)));
					empty2 += ",DataSource= @dataSource";
					list.Add(new SqlParameter("@dataSource", Function.ToString(doc.DataSource)));
					empty2 += ",Series= @series ";
					list.Add(new SqlParameter("@series", Function.ParseInt(doc.Series)));
					empty2 += ",UserSign= @userSign ";
					list.Add(new SqlParameter("@userSign", Function.ParseInt(doc.UserSign)));
					empty2 += ",ObjType= @objType ";
					list.Add(new SqlParameter("@objType", Function.ToString(doc.ObjType)));
					empty2 += " WHERE DocEntry =@docEntry";
					list.Add(new SqlParameter("@docEntry", doc.DocEntry));
				}
				int num = ExecuteNonQuery(empty2, CommandType.Text, list.ToArray());
				if (num > 0)
				{
					if (doc.LinesMRV1 != null)
					{
						foreach (InventoryRevaluation_LineMRV1Impl item in doc.LinesMRV1)
						{
							if (!string.IsNullOrEmpty(SaveLineMRV1(item)))
							{
								dbTransaction.Rollback();
							}
						}
					}
					if (doc.LinesMRV2 != null)
					{
						foreach (InventoryRevaluation_LineMRV2Impl item2 in doc.LinesMRV2)
						{
							if (!string.IsNullOrEmpty(SaveLineMRV2(item2)))
							{
								dbTransaction.Rollback();
							}
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
				empty = ex.Message;
			}
			return empty;
		}
	}

	public string DeleteLineMRV1(int docEntry, int lineNum)
	{
		try
		{
			string query = "delete from MRV1 where DocEntry =  '" + docEntry + "' and LineNum = '" + lineNum + "' ";
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
}
