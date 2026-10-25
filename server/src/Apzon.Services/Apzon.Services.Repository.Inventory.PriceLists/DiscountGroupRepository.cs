using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Inventory.PriceLists;

namespace Apzon.Services.Repository.Inventory.PriceLists;

public class DiscountGroupRepository : BaseService<DataSet>, IDiscountGroup
{
	private const string docTemp = "#OEDG";

	private const string itemTemp = "#EDG1";

	public DiscountGroupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetTableColumns(string table)
	{
		DataTable dataTable = new DataTable();
		try
		{
			if (ExistsTable(table))
			{
				string query = "select top 0 * from " + table + ";";
				dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return dataTable;
	}

	public HttpResult SaveDoc(DataSet ds)
	{
		return Process(ApzObjectTypes.oDiscountGroups, -1, Enums.TransactionType.A, ds);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			string text = "";
			if (Function.ParseInt(ds.Tables[0].Rows[0]["AbsEntry"]) < 1)
			{
				if (Function.ToString(ds.Tables[0].Rows[0]["Type"]) == "A")
				{
					int entryTypeAll = GetEntryTypeAll();
					if (entryTypeAll > 0)
					{
						ds.Tables[0].Rows[0]["AbsEntry"] = entryTypeAll;
						for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
						{
							ds.Tables[1].Rows[i]["AbsEntry"] = entryTypeAll;
						}
						text = "U";
					}
					else
					{
						entryTypeAll = GetMaxAbsEntry() + 1;
						ds.Tables[0].Rows[0]["AbsEntry"] = entryTypeAll;
						for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
						{
							ds.Tables[1].Rows[j]["AbsEntry"] = entryTypeAll;
						}
						text = "I";
					}
				}
				else
				{
					int num = GetMaxAbsEntry() + 1;
					ds.Tables[0].Rows[0]["AbsEntry"] = num;
					for (int k = 0; k < ds.Tables[1].Rows.Count; k++)
					{
						ds.Tables[1].Rows[k]["AbsEntry"] = num;
					}
					text = "I";
				}
			}
			else
			{
				int num2 = Function.ParseInt(ds.Tables[0].Rows[0]["AbsEntry"]);
				for (int l = 0; l < ds.Tables[1].Rows.Count; l++)
				{
					ds.Tables[1].Rows[l]["AbsEntry"] = num2;
				}
				text = "U";
			}
			if (ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data must not null!");
			}
			DataTable tableStruct = GetTableStruct("OEDG");
			DataTable dataTable = ds.Tables[0];
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, "#OEDG");
			BulkCopy(dataTable, "#OEDG");
			DataTable tableStruct2 = GetTableStruct("EDG1");
			DataTable dataTable2 = ds.Tables[1];
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			CreateTempTable(dataTable2, "#EDG1");
			BulkCopy(dataTable2, "#EDG1");
			SqlParameter sqlParameter = new SqlParameter("@type", text);
			DataTable dataTable3 = ExecuteDataTable("APZ_DiscountGroup_Save", CommandType.StoredProcedure, sqlParameter);
			if (dataTable3 != null && dataTable3.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable3.Rows[0][1])))
				{
					objId = Function.ToString(dataTable3.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable3.Rows[0][0])}-{Function.ToString(dataTable3.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, "Save error!");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private int GetMaxAbsEntry()
	{
		int result = 0;
		try
		{
			string maxAbsEntryQuery = GetMaxAbsEntryQuery();
			DataTable dataTable = ExecuteDataTable(maxAbsEntryQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ParseInt(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string GetMaxAbsEntryQuery()
	{
		return "SELECT isnull(max(AbsEntry),0) FROM OEDG";
	}

	private int GetEntryTypeAll()
	{
		int result = 0;
		try
		{
			string query = "SELECT \"AbsEntry\" FROM OEDG WHERE \"Type\"='A';";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ParseInt(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable SearchLine(int entry)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_DiscountGroup_SearchLine";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Entry", entry));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable Search(DataTable dt)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_DiscountGroup_Search";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Type", Function.ToString(dt.Rows[0]["Type"])));
			list.Add(new SqlParameter("@ObjCode", Function.ToString(dt.Rows[0]["ObjCode"])));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet LoadTabPagesData()
	{
		DataSet result = new DataSet();
		try
		{
			string query = "APZ_DiscountGroup_LoadTabPagesData";
			result = ExecuteData(query, CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetAllBPEntry()
	{
		DataTable result = new DataTable();
		try
		{
			string allBPEntryQuery = GetAllBPEntryQuery();
			result = ExecuteDataTable(allBPEntryQuery, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string GetAllBPEntryQuery()
	{
		return "select top 1 isnull(AbsEntry,0) FROM OEDG WHERE Type='A'";
	}

	private bool CheckLine(DataRow row)
	{
		bool result = false;
		try
		{
			int num = Function.ParseInt(row["AbsEntry"]);
			string value = Function.ToString(row["ObjType"]);
			string value2 = Function.ToString(row["ObjKey"]);
			string query = "select count(*) from EDG1 where AbsEntry=@AbsEntry and ObjType=@ObjType and ObjKey=@ObjKey";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@AbsEntry", num));
			list.Add(new SqlParameter("@ObjType", value));
			list.Add(new SqlParameter("@ObjKey", value2));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ParseInt(dataTable.Rows[0][0]) > 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	private bool ExistsTable(string table)
	{
		bool result = false;
		try
		{
			string query = ExistsTableQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@TablePara", table));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ParseInt(dataTable.Rows[0][0]) != 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string ExistsTableQuery()
	{
		return "DECLARE @TableName VARCHAR(100) = @TablePara;\r\n                            DECLARE @Exists bigint = ( SELECT  COUNT(1) FROM sys.tables WHERE name = @TableName AND type = 'U' );\r\n                            select @Exists";
	}

	public DataTable GenDocTable()
	{
		DataTable dataTable = new DataTable();
		try
		{
			string query = "select top 0 a.*\r\n\t                            ,b.\"CardCode\"\r\n\t                            ,b. \"CardName\",\r\n\t                            c.\"GroupCode\"\r\n                            from OEDG a left join OCRD  b on a.\"ObjCode\"=b.\"CardCode\"\r\n                            left join OCRG c on a.\"ObjCode\"=c.\"GroupCode\";\r\n                            ";
			dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			DataRow row = dataTable.NewRow();
			dataTable.Rows.Add(row);
			dataTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return dataTable;
	}
}
