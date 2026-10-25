using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class DefineNewRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IDefineNew
{
	private string TempTableTab = "";

	private string fieldName = "";

	public DataTable Load(string tableName)
	{
		try
		{
			string query = $"select * from {tableName};";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable Check(int Id, string tableName, string fieldName)
	{
		if (string.IsNullOrEmpty(tableName))
		{
			return new DataTable();
		}
		if (string.IsNullOrEmpty(fieldName))
		{
			return new DataTable();
		}
		DataTable dataTable = new DataTable();
		try
		{
			dataTable.Columns.Add("Error", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Error"] = "Y";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
		}
		catch
		{
		}
		try
		{
			string text = "";
			string[] array = tableName.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 0)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "No tables to check.");
				dataTable.Rows[0]["Error"] = "No tables to check.";
				return dataTable;
			}
			string[] array2 = fieldName.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length < array.Length)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "No fields to check.");
				dataTable.Rows[0]["Error"] = "No fields to check.";
				return dataTable;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (!ExistsTable(array[i]))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Table doesn't exist in database.");
					return dataTable;
				}
				if (!ExistsColumn(array[i], array2[i]))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Column doesn't exist in table.");
					return dataTable;
				}
				text += $" SELECT 1 FROM {array[i]} WHERE \"{array2[i]}\"=@ID";
				if (i != array.Length - 1)
				{
					text += " UNION ALL ";
				}
				text += ";";
			}
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ID", Id));
			DataTable dataTable2 = ExecuteDataTable(text, CommandType.Text, list.ToArray());
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				return dataTable2;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			dataTable.Rows[0]["Error"] = ex.Message;
			return dataTable;
		}
	}

	public DataTable CheckStatusTrackingIssue(int Id, string tableName)
	{
		if (string.IsNullOrEmpty(tableName))
		{
			return new DataTable();
		}
		DataTable dataTable = new DataTable();
		try
		{
			dataTable.Columns.Add("Error", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Error"] = "Y";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
		}
		catch
		{
		}
		try
		{
			string text = "";
			string[] array = tableName.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 0)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "No tables to check.");
				dataTable.Rows[0]["Error"] = "No tables to check.";
				return dataTable;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (!ExistsTable(array[i]))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Table doesn't exist in database.");
					return dataTable;
				}
				text += $" SELECT 1 FROM APZ_TGIE WHERE \"Status\"=@ID";
				if (i != array.Length - 1)
				{
					text += " UNION ALL ";
				}
				text += ";";
			}
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ID", Id));
			DataTable dataTable2 = ExecuteDataTable(text, CommandType.Text, list.ToArray());
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				return dataTable2;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			dataTable.Rows[0]["Error"] = ex.Message;
			return dataTable;
		}
	}

	public HttpResult Save(Tuple<DataTable, string, string> dt)
	{
		string item = dt.Item2;
		if (!ExistsTable(item))
		{
			return new HttpResult();
		}
		fieldName = dt.Item3;
		if (!ExistsColumn(item, fieldName))
		{
			return new HttpResult();
		}
		TempTableTab = "#dt" + item;
		DataTable dataTable = dt.Item1;
		if (dataTable == null || dataTable.Rows.Count == 0)
		{
			dataTable = GetTableStruct(item);
			dataTable.Rows.Clear();
		}
		return Process(ApzObjectTypes.oBusinessPartnerPriorities, -1, Enums.TransactionType.A, dataTable);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTableTab);
			BulkCopy(document, TempTableTab);
			string tableName = TempTableTab.Substring(3, TempTableTab.Length - 3);
			object obj = ExecuteScalar(ProcessDataGetQuery(tableName, TempTableTab, fieldName), CommandType.Text);
			objId = Function.ToString(obj);
			objNum = objId;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual string ProcessDataGetQuery(string tableName, string tempTableTab, string fieldName)
	{
		return string.Format("\r\ndeclare @i int  = (SELECT ISNULL(MAX({2}),0) FROM {0}) \r\n\tUPDATE {1} SET {2}  = @i ,@i = @i + 1\r\n\twhere {2} = 0 OR {2} is null\r\n                                     DELETE {0}\r\n                                     INSERT INTO {0}\r\n                                     SELECT * FROM {1}", tableName, tempTableTab, fieldName);
	}

	private bool ExistsTable(string table)
	{
		bool result = false;
		try
		{
			result = ExecuteDataTable(ExistsTableGetQuery(), CommandType.Text, new SqlParameter("@TableName", table)).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string ExistsTableGetQuery()
	{
		return "SELECT 1 FROM sys.tables WHERE name = @TableName AND type = 'U'";
	}

	private bool ExistsColumn(string tableName, string columnName)
	{
		bool result = false;
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Table", tableName));
			list.Add(new SqlParameter("@Column", columnName));
			DataTable dataTable = ExecuteDataTable(ExistsColumnGetQuery(), CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]) == 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string ExistsColumnsQuery()
	{
		return "select case when COL_LENGTH(@Table, @Column) IS NOT NULL then 1 else 0 end CheckColumn";
	}

	protected virtual string ExistsColumnGetQuery()
	{
		return ExistsColumnsQuery();
	}
}
