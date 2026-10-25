using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Items;
using Apzon.Services.Interfaces.Inventory;

namespace Apzon.Services.Repository.Inventory;

public class SpecialBp_LineSPP2Respository : BaseService<SpecialBP_LineSPP2Impl>, ISpecialBP_LineSPP2
{
	public SpecialBp_LineSPP2Respository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public bool CheckLineSPP2(string itemCode, string cardCode, int lineNum, int spp2LNum)
	{
		try
		{
			string text = "SELECT *\r\n                            FROM SPP2 \r\n                            WHERE \"CardCode\" = '" + cardCode + "'";
			text = text + " AND \"ItemCode\" = '" + itemCode + "'";
			text = text + " AND \"SPP1LNum\" = '" + lineNum + "'";
			text = text + " AND \"SPP2LNum\" = '" + spp2LNum + "'";
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

	public string DeleteLineSPP2(List<Tuple<string, string>> obj)
	{
		try
		{
			string query = "delete from SPP2 where \"CardCode\" = '" + obj.Find((Tuple<string, string> t) => t.Item1 == "cardCode").Item2 + "' and \"ItemCode\" = '" + obj.Find((Tuple<string, string> t) => t.Item1 == "itemCode").Item2 + "' and  \"SPP1LNum\" ='" + obj.Find((Tuple<string, string> t) => t.Item1 == "lineNum").Item2 + "' and \"SPP2LNum\" ='" + obj.Find((Tuple<string, string> t) => t.Item1 == "spp2LNum").Item2 + "'; ";
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

	public string SaveDoc(List<SpecialBP_LineSPP2Impl> docs)
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

	public string SaveADoc(SpecialBP_LineSPP2Impl LineSPP2)
	{
		string empty = string.Empty;
		List<SqlParameter> list = new List<SqlParameter>();
		try
		{
			bool flag = CheckLineSPP2(LineSPP2.ItemCode, LineSPP2.CardCode, LineSPP2.LINENUM, LineSPP2.SPP2LNum);
			string empty2 = string.Empty;
			if (!flag)
			{
				empty2 = "INSERT INTO SPP2  (\"ItemCode\" , \"CardCode\" ,\"SPP1LNum\",\"SPP2LNum\", \"Amount\", \"Discount\",  \"Price\", \"Currency\")   \r\n                               VALUES \r\n                                     ( ";
				empty2 += " @itemCode ";
				list.Add(new SqlParameter("@itemCode", Function.ToString(LineSPP2.ItemCode)));
				empty2 += ", @cardCode ";
				list.Add(new SqlParameter("@cardCode", Function.ToString(LineSPP2.CardCode)));
				empty2 += ", @lineNum ";
				list.Add(new SqlParameter("@lineNum", Function.ParseInt(LineSPP2.LINENUM)));
				empty2 += ", @spp2lineNum ";
				list.Add(new SqlParameter("@spp2lineNum", Function.ParseInt(LineSPP2.SPP2LNum)));
				empty2 += ", @amount ";
				list.Add(new SqlParameter("@amount", Function.ParseDouble(LineSPP2.Amount)));
				empty2 += ", @discount ";
				list.Add(new SqlParameter("@discount", Function.ParseDouble(LineSPP2.Discount)));
				empty2 += ", @price ";
				list.Add(new SqlParameter("@price", Function.ParseDouble(LineSPP2.Price)));
				empty2 += ", @currency ";
				list.Add(new SqlParameter("@currency", Function.ToString(LineSPP2.Currency)));
				empty2 += " )";
			}
			else
			{
				list = new List<SqlParameter>();
				empty2 = "UPDATE SPP2  \r\n                            SET \r\n                                \"Amount\"= ";
				empty2 += " @amount ";
				list.Add(new SqlParameter("@amount", Function.ParseDouble(LineSPP2.Amount)));
				empty2 += ",\"Discoun\"t= @discount ";
				list.Add(new SqlParameter("@discount", Function.ParseDouble(LineSPP2.Discount)));
				empty2 += ",\"Price\"= @price ";
				list.Add(new SqlParameter("@price", Function.ParseDouble(LineSPP2.Price)));
				empty2 += ",\"Currency\"= @currency ";
				list.Add(new SqlParameter("@currency", Function.ToString(LineSPP2.Currency)));
				empty2 += " WHERE \"CardCode\" =@cardCode";
				list.Add(new SqlParameter("@cardCode", LineSPP2.CardCode));
				empty2 += " AND \"ItemCode\" = @itemCode";
				list.Add(new SqlParameter("@itemCode", LineSPP2.ItemCode));
				empty2 += " AND \"SPP1LNum\" = @lineNum";
				list.Add(new SqlParameter("@lineNum", LineSPP2.LINENUM));
				empty2 += " AND \"SPP2LNum\" = @spp2lineNum";
				list.Add(new SqlParameter("@spp2lineNum", LineSPP2.SPP2LNum));
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE LineSPP2  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetMaxSPP2LNum(string itemCode, string cardCode, int lineNum)
	{
		try
		{
			string maxSPP2LNumQuery = GetMaxSPP2LNumQuery(itemCode, cardCode, lineNum);
			DataTable dataTable = ExecuteDataTable(maxSPP2LNumQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetMaxSPP2LNumQuery(string itemCode, string cardCode, int lineNum)
	{
		return "SELECT ISNULL( MAX(SPP2LNum ),0) as SPP2LNum\r\n                            FROM SPP2 Where ItemCode = '" + itemCode + "'  and  CardCode= '" + cardCode + "' and  SPP1LNum = '" + lineNum + "' ";
	}

	public DataTable SearchLineSPP2(string itemCode, string cardCode, int SPP1LineNum)
	{
		DataTable result = new DataTable();
		try
		{
			string query = SearchLineSPP2Query(itemCode, cardCode, SPP1LineNum);
			result = ExecuteData(query, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Search - Doc ", ex.Message);
		}
		return result;
	}

	protected virtual string SearchLineSPP2Query(string itemCode, string cardCode, int SPP1LineNum)
	{
		string text = "select distinct a.ItemCode, a.CardCode,a.SPP1LNum as LINENUM,a.SPP2LNum,\r\n\t                                            a.Amount, a.Discount,a.Price,\r\n\t                                       Isnull (a.Currency,'VND') as Currency                                                          \r\n                                from SPP2 a                                                                                                        \r\n                                    WHERE 1=1 ";
		if (!string.IsNullOrEmpty(itemCode))
		{
			text = text + "AND a.ItemCode = '" + itemCode + "' ";
		}
		if (!string.IsNullOrEmpty(cardCode))
		{
			text = text + "AND a.CardCode = '" + cardCode + "' ";
		}
		if (SPP1LineNum != -1)
		{
			text = text + "AND a.SPP1LNum = '" + SPP1LineNum + "' ";
		}
		return text;
	}
}
