using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.ImportData;

namespace Apzon.Services.Repository.ImportData;

public class ImportItemMasterDataRepository : BaseService<IImportItemMasterData>, IImportItemMasterData
{
	public ImportItemMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetTableColumns(string tableName)
	{
		try
		{
			if (!tableName.Contains(" "))
			{
				string query = "SELECT COLUMN_NAME,DATA_TYPE\r\n                FROM INFORMATION_SCHEMA.COLUMNS\r\n                WHERE TABLE_NAME = '" + tableName + "'";
				DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
				if (dataTable.IsNotNull())
				{
					return dataTable;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetColumnNam(string tableName)
	{
		try
		{
			if (!tableName.Contains(" "))
			{
				string query = "SELECT TOP 0 * FROM  " + tableName;
				DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
				dataTable.AcceptChanges();
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
		return new DataTable();
	}

	public DataTable CheckStatus(Common.Table_Code tb)
	{
		try
		{
			if (!tb.Name.Contains(" "))
			{
				string query = "select ItemCode from " + tb.Name + " where ItemCode=@ItemCode";
				SqlParameter sqlParameter = new SqlParameter("@ItemCode", tb.Code);
				return ExecuteDataTable(query, CommandType.Text, sqlParameter);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string SaveAll(Tuple<DataTable, string> tup)
	{
		try
		{
			DataTable item = tup.Item1;
			string item2 = tup.Item2;
			if (!item2.Contains(" "))
			{
				DataRow dataRow = item.Rows[0];
				DataTable tableColumns = GetTableColumns(item2);
				using (DatabaseService.OpenConnection())
				{
					using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
					string text;
					if (Exists(Function.ToString(dataRow["ItemCode"]), item2))
					{
						text = UpdateData(dataRow, tableColumns, item2);
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
					}
					else
					{
						text = InsertData(dataRow, tableColumns, item2);
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
					}
					dbTransaction.Commit();
					return text;
				}
			}
			return "Table Name is not Exist";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public bool Exists(string code, string nameTable)
	{
		try
		{
			string query = "select ItemCode from " + nameTable + " where ItemCode=@ItemCode";
			SqlParameter sqlParameter = new SqlParameter("@ItemCode", code);
			if (ExecuteDataTable(query, CommandType.Text, sqlParameter).IsNotNull())
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

	private string InsertData(DataRow dataRow, DataTable dtStr, string tableName)
	{
		try
		{
			string text = " INSERT " + tableName + "  (";
			for (int i = 0; i < dtStr.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
				if (!string.IsNullOrEmpty(Function.ToString(dataRow[text2])))
				{
					text = text + text2 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += ")";
			text += " VALUES (";
			for (int j = 0; j < dtStr.Rows.Count; j++)
			{
				string text3 = Function.ToString(dtStr.Rows[j]["COLUMN_NAME"]);
				if (!string.IsNullOrEmpty(Function.ToString(dataRow[text3])))
				{
					text = text + "@" + text3 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += ")";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStr);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string UpdateData(DataRow dataRow, DataTable dtStr, string tableName)
	{
		try
		{
			string text = "UPDATE " + tableName + " SET ";
			for (int i = 0; i < dtStr.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
				if (!string.IsNullOrEmpty(Function.ToString(dataRow[text2])) && text2 != "ItemCode")
				{
					text = text + Environment.NewLine + text2 + "=@" + text2 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += " where ItemCode= @ItemCode";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStr);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private List<SqlParameter> MappingSqlParameter(DataRow dataRow, DataTable dtStr)
	{
		List<SqlParameter> list = new List<SqlParameter>();
		try
		{
			for (int i = 0; i < dtStr.Rows.Count; i++)
			{
				string text = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
				string text2 = Function.ToString(dtStr.Rows[i]["DATA_TYPE"]);
				if (!string.IsNullOrEmpty(Function.ToString(dataRow[text])))
				{
					switch (text2.ToUpper())
					{
					case "NVARCHAR":
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					case "VARCHAR":
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					case "DATETIME":
						list.Add(new SqlParameter("@" + text, Function.ParseDateTime(dataRow[text])));
						break;
					case "CHAR":
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					case "NTEXT":
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					case "SMALLINT":
						list.Add(new SqlParameter("@" + text, dataRow[text]));
						break;
					case "BIT":
						list.Add(new SqlParameter("@" + text, Function.ParseBool(dataRow[text])));
						break;
					case "DATE":
						list.Add(new SqlParameter("@" + text, Function.ParseDateTimes(dataRow[text])));
						break;
					case "DATETIME2":
						list.Add(new SqlParameter("@" + text, Function.ParseDateTimes(dataRow[text])));
						break;
					case "DECIMAL":
						list.Add(new SqlParameter("@" + text, Function.ParseDecimal(dataRow[text])));
						break;
					case "BINARY":
						list.Add(new SqlParameter("@" + text, dataRow[text]));
						break;
					case "NCHAR":
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					case "INT":
						list.Add(new SqlParameter("@" + text, Function.ParseInt(dataRow[text])));
						break;
					case "NUMERIC":
						list.Add(new SqlParameter("@" + text, Function.ParseDecimal(dataRow[text])));
						break;
					case "SMALLDATETIME":
						list.Add(new SqlParameter("@" + text, Function.ParseDateTimes(dataRow[text])));
						break;
					case "TEXT":
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					default:
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return list;
	}

	public string Save(DataTable docs)
	{
		throw new NotImplementedException();
	}
}
