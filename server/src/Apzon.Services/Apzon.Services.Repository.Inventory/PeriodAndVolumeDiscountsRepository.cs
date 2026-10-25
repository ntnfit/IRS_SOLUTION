using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Inventory;

namespace Apzon.Services.Repository.Inventory;

public class PeriodAndVolumeDiscountsRepository : BaseService<DataTable>, IPeriodAndVolumeDiscounts
{
	public PeriodAndVolumeDiscountsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	private string GetInsertUDFFieldString(DataTable dt)
	{
		string text = "";
		try
		{
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				if (dt.Columns[i].ColumnName.StartsWith("U_"))
				{
					text = text + "," + dt.Columns[i].ColumnName;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public DataSet LoadData(string cardCode)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "APZ_PeriodAndVolumeDiscounts_LoadData";
			result = ExecuteData(query, CommandType.StoredProcedure, new SqlParameter("@cardCode", cardCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public string SaveDoc(DataSet ds)
	{
		try
		{
			if (ds == null || ds.Tables.Count != 3)
			{
				return "Not enough data";
			}
			DataTable dataTable = ds.Tables[0];
			DataTable tableStruct = GetTableStruct("OSPP");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			DataTable dataTable2 = ds.Tables[1];
			DataTable tableStruct2 = GetTableStruct("SPP1");
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			source = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source.ToArray());
			DataTable dataTable3 = ds.Tables[2];
			DataTable tableStruct3 = GetTableStruct("SPP2");
			if (!tableStruct3.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			source = from DataColumn t in tableStruct3.Columns
				select t.ColumnName;
			dataTable3 = dataTable3.DefaultView.ToTable(distinct: false, source.ToArray());
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dataTable, "#OSPP");
				BulkCopy(dataTable, "#OSPP");
				CreateTempTable(dataTable2, "#SPP1");
				BulkCopy(dataTable2, "#SPP1");
				CreateTempTable(dataTable3, "#SPP2");
				BulkCopy(dataTable3, "#SPP2");
				ExecuteNonQuery("APZ_PeriodAndVolumeDiscounts_Save", CommandType.StoredProcedure);
				dbTransaction.Commit();
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string SaveADoc(DataRow row)
	{
		string text = "ERROR";
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string empty = string.Empty;
			if (!CheckDoc(Function.ParseInt(row["ListNum"]), Function.ToString(row["ItemCode"])))
			{
				empty = "INSERT INTO OSPP\r\n                                   (\"ItemCode\"\r\n                                   ,\"CardCode\"\r\n                                   ,\"Price\"\r\n                                   ,\"Currency\"\r\n                                   ,\"Discount\"\r\n                                   ,\"ListNum\"\r\n                                   ,\"AutoUpdt\"\r\n                                   ,\"EXPAND\"\r\n                                   ,\"UserSign\"\r\n                                   ,\"SrcPrice\"\r\n                                   ,\"LogInstanc\"\r\n                                   ,\"UserSign2\"\r\n                                   ,\"CreateDate\"\r\n                                   ,\"UpdateDate\") VALUES (";
				empty += "@ItemCode ";
				list.Add(new SqlParameter("@ItemCode", Function.ToString(row["ItemCode"])));
				empty += ",@CardCode ";
				list.Add(new SqlParameter("@CardCode", "*" + Function.ToString(row["ListNum"])));
				empty += ",0 ";
				empty += ",'VND' ";
				empty += ",0 ";
				empty += ",@ListNum ";
				list.Add(new SqlParameter("@ListNum", Function.ParseInt(row["ListNum"])));
				empty += ",@AutoUpdt ";
				list.Add(new SqlParameter("@AutoUpdt", "N"));
				empty += ",@EXPAND ";
				list.Add(new SqlParameter("@EXPAND", "N"));
				empty += ",@UserSign ";
				list.Add(new SqlParameter("@UserSign", 1));
				empty += ",@SrcPrice ";
				list.Add(new SqlParameter("@SrcPrice", Function.ParseInt(row["SrcPrice"])));
				empty += ",0 ";
				empty += ",1 ";
				empty += ",@CreateDate ";
				list.Add(new SqlParameter("@CreateDate", DateTime.Today));
				empty += ",@UpdateDate ";
				list.Add(new SqlParameter("@UpdateDate", DateTime.Today));
				empty += " );";
			}
			else
			{
				list = new List<SqlParameter>();
				empty = "Update OSPP SET ";
				empty += " \"SrcPrice\"=@SrcPrice ";
				list.Add(new SqlParameter("@SrcPrice", Function.ParseInt(row["SrcPrice"])));
				empty += " WHERE \"ItemCode\" =@ItemCode";
				list.Add(new SqlParameter("@ItemCode", Function.ToString(row["ItemCode"])));
				empty += " and \"ListNum\"=@ListNum ;";
				list.Add(new SqlParameter("@ListNum", Function.ParseInt(row["ListNum"])));
			}
			if (ExecuteNonQuery(empty, CommandType.Text, list.ToArray()) != 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE A DOC ITM1  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public bool CheckDoc(int listNum, string itemCode)
	{
		try
		{
			string query = "SELECT \"ListNum\" FROM OSPP  WHERE \"ListNum\" = @ListNum and \"ItemCode\"=@ItemCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ListNum", listNum));
			list.Add(new SqlParameter("@ItemCode", itemCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
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

	public string GetDeleteDocs(DataTable dt)
	{
		try
		{
			string text = GetDeleteDocsQuery();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				text = text + "@item" + Function.ToString(i) + ",";
			}
			if (text[text.Length - 1] == ',')
			{
				text = text.Remove(text.Length - 1);
			}
			text += GetDeleteDocsAndQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ListNum", Function.ToString(dt.Rows[0]["ListNum"])));
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				list.Add(new SqlParameter("@item" + Function.ToString(j), Function.ToString(dt.Rows[j]["ItemCode"])));
			}
			DataTable dataTable = ExecuteDataTable(text, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				string text2 = Function.ToString(dataTable.Rows[0][0]);
				if (text2.Length > 0)
				{
					text2 = text2.Remove(text2.Length - 1);
				}
				return text2;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return "";
		}
	}

	protected virtual string GetDeleteDocsQuery()
	{
		return "select ItemCode+',' from OSPP a where listnum=@ListNum and itemcode not in (";
	}

	protected virtual string GetDeleteDocsAndQuery()
	{
		return ") and (select count(cardcode) from ocrd where cardcode=a.cardcode)=0 FOR  XML PATH('')";
	}

	public string DeleteDocs(string deleteDocs, string listNum)
	{
		string text = "";
		try
		{
			string query = "delete from OSPP where \"ItemCode\" in (@deleteDocs) and \"ListNum\"=@ListNum;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@deleteDocs", deleteDocs));
			list.Add(new SqlParameter("@ListNum", listNum));
			if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public DataTable GetItemByItemCode(string itemCode, int listNum)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_PeriodAndVolumeDiscounts_GetItemByItemCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@ListNum", listNum));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetItems(string fCode, string tCode, string fVendor, string tVendor)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_PeriodAndVolumeDiscounts_GetItems";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@fCode", fCode));
			list.Add(new SqlParameter("@tCode", tCode));
			list.Add(new SqlParameter("@fVendor", fVendor));
			list.Add(new SqlParameter("@tVendor", tVendor));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}
}
