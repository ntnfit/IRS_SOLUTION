using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Items;
using Apzon.Services.Interfaces;

namespace Apzon.Services.Repository.Inventory;

public class SpecialBp_LineSPP1Respository : BaseService<SpecialBP_LineSPP1Impl>, ISpecialBP_LineSPP1
{
	public SpecialBp_LineSPP1Respository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public bool CheckLineSPP1(string itemCode, string cardCode, int lineNum)
	{
		try
		{
			string text = "SELECT a.\"ItemCode\", a.\"CardCode\", a.\"LINENUM\"\r\n                            FROM SPP1 a\r\n                            WHERE a.\"CardCode\" = '" + cardCode + "'";
			text = text + " AND a.\"ItemCode\" = '" + itemCode + "'";
			text = text + " AND a.\"LINENUM\" = '" + lineNum + "'";
			text += ";";
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

	public DataTable SearchLineSPP1(string itemCode, string cardCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = SearchLineSPP1Query(itemCode, cardCode);
			result = ExecuteData(query, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Search - LineSPP1 ", ex.Message);
		}
		return result;
	}

	protected virtual string SearchLineSPP1Query(string itemCode, string cardCode)
	{
		string text = "select distinct a.ItemCode, a.CardCode,a.LINENUM,\r\n\t                          \r\n\t                                      a.Price,  Isnull (a.Currency,'VND') as Currency, a.Discount,a.ListNum,\r\n                                          a.FromDate,a.ToDate,a.AutoUpdt,a.Expand                                                               \r\n                                from SPP1 a                                                                                                        \r\n                                    WHERE 1=1 ";
		if (!string.IsNullOrEmpty(itemCode))
		{
			text = text + "AND a.ItemCode = '" + itemCode + "' ";
		}
		if (!string.IsNullOrEmpty(cardCode))
		{
			text = text + "AND a.CardCode = '" + cardCode + "' ";
		}
		return text;
	}

	public string SaveDoc(List<SpecialBP_LineSPP1Impl> docs)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string text = "";
				for (int i = 0; i < docs.Count; i++)
				{
					string text2 = SaveADoc(docs[i]);
					if (!string.IsNullOrWhiteSpace(text2))
					{
						text = text2;
						break;
					}
				}
				if (string.IsNullOrWhiteSpace(text))
				{
					dbTransaction.Commit();
				}
				else
				{
					dbTransaction.Rollback();
				}
				return text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string SaveADoc(SpecialBP_LineSPP1Impl LineSPP1)
	{
		string empty = string.Empty;
		List<SqlParameter> list = new List<SqlParameter>();
		try
		{
			if (string.IsNullOrWhiteSpace(LineSPP1.CardCode))
			{
				LineSPP1.CardCode = "*" + LineSPP1.ListNum;
			}
			bool flag = CheckLineSPP1(LineSPP1.ItemCode, LineSPP1.CardCode, LineSPP1.LINENUM);
			string empty2 = string.Empty;
			if (!flag)
			{
				empty2 = "INSERT INTO SPP1  (\"ItemCode\" , \"CardCode\" ,\"LINENUM\", \"Price\", \"Currency\", \"Discount\", \"ListNum\", \"FromDate\",\"ToDate\",\"AutoUpdt\" ,\"Expand\"  )   \r\n                               VALUES \r\n                                     ( ";
				empty2 += " @itemCode ";
				list.Add(new SqlParameter("@itemCode", Function.ToString(LineSPP1.ItemCode)));
				empty2 += ", @cardCode ";
				list.Add(new SqlParameter("@cardCode", Function.ToString(LineSPP1.CardCode)));
				empty2 += ", @lineNum ";
				list.Add(new SqlParameter("@lineNum", Function.ParseInt(LineSPP1.LINENUM)));
				empty2 += ", @price ";
				list.Add(new SqlParameter("@price", Function.ParseDouble(LineSPP1.Price)));
				empty2 += ", @currency ";
				list.Add(new SqlParameter("@currency", Function.ToString(LineSPP1.Currency)));
				empty2 += ", @discount ";
				list.Add(new SqlParameter("@discount", Function.ParseDouble(LineSPP1.Discount)));
				empty2 += ", @listNum ";
				list.Add(new SqlParameter("@listNum", Function.ParseInt(LineSPP1.ListNum)));
				empty2 += ", @fromDate";
				list.Add(new SqlParameter("@fromDate", Function.ParseDateTime(LineSPP1.FromDate)));
				empty2 += ", @toDate";
				list.Add(new SqlParameter("@toDate", Function.ParseDateTime(LineSPP1.ToDate)));
				empty2 += ", @autoUpdt ";
				if (LineSPP1.AutoUpdt == "No")
				{
					LineSPP1.AutoUpdt = "N";
				}
				else if (LineSPP1.AutoUpdt == "Yes")
				{
					LineSPP1.AutoUpdt = "Y";
				}
				list.Add(new SqlParameter("@autoUpdt", LineSPP1.AutoUpdt));
				empty2 += ", @expand ";
				if (LineSPP1.Expand == "No")
				{
					LineSPP1.Expand = "N";
				}
				else if (LineSPP1.Expand == "Yes")
				{
					LineSPP1.Expand = "Y";
				}
				list.Add(new SqlParameter("@expand", Function.ToString(LineSPP1.Expand)));
				empty2 += " )";
			}
			else
			{
				list = new List<SqlParameter>();
				empty2 = "UPDATE SPP1  \r\n                            SET\r\n                                \"Price\"= ";
				empty2 += " @price ";
				list.Add(new SqlParameter("@price", Function.ParseDouble(LineSPP1.Price)));
				empty2 += ",\"Currency\"= @currency ";
				list.Add(new SqlParameter("@currency", Function.ToString(LineSPP1.Currency)));
				empty2 += ",\"Discount\"= @discount ";
				list.Add(new SqlParameter("@discount", Function.ParseDouble(LineSPP1.Discount)));
				empty2 += ",\"ListNum\"= @listNum ";
				list.Add(new SqlParameter("@listNum", Function.ParseInt(LineSPP1.ListNum)));
				empty2 += ",\"FromDate\"= @fromDate";
				list.Add(new SqlParameter("@fromDate", Function.ParseDateTime(LineSPP1.FromDate)));
				empty2 += ",\"ToDate\"= @toDate";
				list.Add(new SqlParameter("@toDate", Function.ParseDateTime(LineSPP1.ToDate)));
				empty2 += ",\"AutoUpdt\"= @autoUpdt ";
				if (LineSPP1.AutoUpdt == "No")
				{
					LineSPP1.AutoUpdt = "N";
				}
				else if (LineSPP1.AutoUpdt == "Yes")
				{
					LineSPP1.AutoUpdt = "Y";
				}
				list.Add(new SqlParameter("@autoUpdt", LineSPP1.AutoUpdt));
				empty2 += ",\"Expand\"= @expand ";
				if (LineSPP1.Expand == "No")
				{
					LineSPP1.Expand = "N";
				}
				else if (LineSPP1.Expand == "Yes")
				{
					LineSPP1.Expand = "Y";
				}
				list.Add(new SqlParameter("@expand", Function.ToString(LineSPP1.Expand)));
				empty2 += " WHERE \"CardCode\" =@cardCode";
				list.Add(new SqlParameter("@cardCode", LineSPP1.CardCode));
				empty2 += " AND \"ItemCode\" = @itemCode";
				list.Add(new SqlParameter("@itemCode", LineSPP1.ItemCode));
				empty2 += " AND LINENUM = @lineNum";
				list.Add(new SqlParameter("@lineNum", LineSPP1.LINENUM));
			}
			empty2 += ";";
			if (ExecuteNonQuery(empty2, CommandType.Text, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE LineSPP1  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteLineSPP1(List<Tuple<string, string>> obj)
	{
		try
		{
			string query = "delete from SPP1 where \"CardCode\" = '" + obj.Find((Tuple<string, string> t) => t.Item1 == "cardCode").Item2 + "' and \"ItemCode\" = '" + obj.Find((Tuple<string, string> t) => t.Item1 == "itemCode").Item2 + "' and  LINENUM ='" + obj.Find((Tuple<string, string> t) => t.Item1 == "lineNum").Item2 + "' ;";
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

	public DataTable GetMaxLineNum(string itemCode, string cardCode)
	{
		DataTable dataTable = new DataTable();
		try
		{
			string maxLineNumQuery = GetMaxLineNumQuery(itemCode, cardCode);
			dataTable = ExecuteDataTable(maxLineNumQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	protected virtual string GetMaxLineNumQuery(string itemCode, string cardCode)
	{
		return "SELECT ISNULL( MAX(LINENUM ),0) as LINENUM\r\n                            FROM SPP1 Where ItemCode = '" + itemCode + "'  and  CardCode= '" + cardCode + "'";
	}
}
