using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.ImportData;

namespace Apzon.Services.Repository.Integrated;

public class ImportDataBPRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IImportDataBP
{
	private Dictionary<string, string> dicType = new Dictionary<string, string>
	{
		{ "bigint", "Int64" },
		{ "binary", "Byte[]" },
		{ "bit", "Boolean" },
		{ "char", "String" },
		{ "date", "DateTime" },
		{ "datetime", "DateTime" },
		{ "datetime2", "DateTime" },
		{ "datetimeoffset", "DateTimeOffset" },
		{ "decimal", "Decimal" },
		{ "varbinary(max)", "Byte[]" },
		{ "float", "Double" },
		{ "image", "Byte[]" },
		{ "int", "Int32" },
		{ "money", "Decimal" },
		{ "nchar", "String" },
		{ "ntext", "String" },
		{ "numeric", "Decimal" },
		{ "nvarchar", "String" },
		{ "real", "Single" },
		{ "rowversion", "Byte[]" },
		{ "smalldatetime", "DateTime" },
		{ "smallint", "Int16" },
		{ "smallmoney", "Decimal" },
		{ "sql_variant", "Object *" },
		{ "text", "String" },
		{ "time", "TimeSpan" },
		{ "timestamp", "Byte[]" },
		{ "tinyint", "Byte" },
		{ "uniqueidentifier", "Guid" },
		{ "varbinary", "Byte[]" },
		{ "varchar", "String" },
		{ "xml", "Xml" }
	};

	public DataTable GetTableColumns(string tableName)
	{
		DataTable dataTable = new DataTable();
		try
		{
			if (ExistsTable(tableName))
			{
				string query = "select top 0 * from " + tableName;
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

	private bool ExistsTable(string table)
	{
		bool result = false;
		try
		{
			string query = "DECLARE @TableName VARCHAR(100) = @Table;\r\n                            DECLARE @Exists BIT = ( SELECT CAST( COUNT(1) AS BIT ) FROM sys.tables WHERE name = @TableName AND type = 'U' );\r\n                            select @Exists";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Table", table));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ToString(dataTable.Rows[0][0]).Trim().ToUpper() == "TRUE";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	private bool ExistsColumn(string table, string column)
	{
		bool result = false;
		try
		{
			string query = "DECLARE @ColumnName VARCHAR(100) = @Column;\r\n                            DECLARE @TableName VARCHAR(100) = @Table;\r\n                            DECLARE @Exists BIT = ( SELECT CAST( COUNT(1) AS BIT ) FROM sys.columns  WHERE name = @ColumnName  AND Object_ID = Object_ID(@TableName) );\r\n                            select @Exists";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Table", table));
			list.Add(new SqlParameter("@Column", column));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ToString(dataTable.Rows[0][0]).Trim().ToUpper() == "TRUE";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	private bool ExistsALine(List<Tuple<string, Type>> lstKey, DataRow row, string table)
	{
		bool result = false;
		try
		{
			if (ExistsTable(table))
			{
				string text = "Select count(*) from " + table + " where 1=1 ";
				List<SqlParameter> list = new List<SqlParameter>();
				for (int i = 0; i < lstKey.Count; i++)
				{
					if (ExistsColumn(table, lstKey[i].Item1))
					{
						text = text + " and " + lstKey[i].Item1 + "=@" + lstKey[i].Item1;
						switch (lstKey[i].Item2.GetType().ToString().ToUpper())
						{
						case "SYSTEM.STRING":
							list.Add(new SqlParameter("@" + lstKey[i].Item1, Function.ToString(row[lstKey[i].Item1])));
							break;
						case "SYSTEM.INT64":
							list.Add(new SqlParameter("@" + lstKey[i].Item1, Function.ParseInt(row[lstKey[i].Item1])));
							break;
						case "SYSTEM.BYTE[]":
							list.Add(new SqlParameter("@" + lstKey[i].Item1, row[lstKey[i].Item1]));
							break;
						case "SYSTEM.BOOLEAN":
							list.Add(new SqlParameter("@" + lstKey[i].Item1, Function.ParseBool(row[lstKey[i].Item1])));
							break;
						case "SYSTEM.DATETIME":
							list.Add(new SqlParameter("@" + lstKey[i].Item1, Function.ParseDateTime(row[lstKey[i].Item1])));
							break;
						case "SYSTEM.DECIMAL":
							list.Add(new SqlParameter("@" + lstKey[i].Item1, Function.ParseDecimal(row[lstKey[i].Item1])));
							break;
						case "SYSTEM.DOUBLE":
							list.Add(new SqlParameter("@" + lstKey[i].Item1, Function.ParseDouble(row[lstKey[i].Item1])));
							break;
						case "SYSTEM.INT32":
							list.Add(new SqlParameter("@" + lstKey[i].Item1, Function.ParseInt(row[lstKey[i].Item1])));
							break;
						case "SYSTEM.SINGLE":
							list.Add(new SqlParameter("@" + lstKey[i].Item1, Function.ParseDouble(row[lstKey[i].Item1])));
							break;
						case "SYSTEM.INT16":
							list.Add(new SqlParameter("@" + lstKey[i].Item1, Function.ParseInt(row[lstKey[i].Item1])));
							break;
						case "SYSTEM.TIMESPAN":
							list.Add(new SqlParameter("@" + lstKey[i].Item1, Function.ParseDateTimes(row[lstKey[i].Item1]).TimeOfDay));
							break;
						case "SYSTEM.BYTE":
							list.Add(new SqlParameter("@" + lstKey[i].Item1, row[lstKey[i].Item1]));
							break;
						default:
							list.Add(new SqlParameter("@" + lstKey[i].Item1, Function.ToString(row[lstKey[i].Item1])));
							break;
						}
					}
				}
				DataTable dataTable = ExecuteDataTable(text, CommandType.Text, list.ToArray());
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					result = Function.ParseInt(dataTable.Rows[0][0]) > 0;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public string SaveData(Tuple<string, DataTable, List<Tuple<string, Type>>> tupleSave)
	{
		string text = "";
		try
		{
			if (ExistsTable(tupleSave.Item1) && tupleSave.Item2 != null && tupleSave.Item2.Rows.Count > 0)
			{
				using (DatabaseService.OpenConnection())
				{
					using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
					text = ((!(Function.ToString(tupleSave.Item2.Rows[0]["MyStatus"]).ToUpper() == "UPDATE")) ? InsertData(tupleSave) : UpdateData(tupleSave));
					if (string.IsNullOrWhiteSpace(text))
					{
						dbTransaction.Commit();
					}
					else
					{
						dbTransaction.Rollback();
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			text = ex.Message;
		}
		return text;
	}

	private string UpdateData(Tuple<string, DataTable, List<Tuple<string, Type>>> tupleSave)
	{
		string text = "";
		try
		{
			DataRow dataRow = tupleSave.Item2.Rows[0];
			List<SqlParameter> list = new List<SqlParameter>();
			string text2 = "UPDATE " + tupleSave.Item1 + " SET ";
			int num = 0;
			int i;
			for (i = 0; i < tupleSave.Item2.Columns.Count; i++)
			{
				if (!ExistsColumn(tupleSave.Item1, tupleSave.Item2.Columns[i].ColumnName) || tupleSave.Item3.Exists((Tuple<string, Type> t) => t.Item1.ToUpper() == tupleSave.Item2.Columns[i].ColumnName.ToUpper()) || dataRow[tupleSave.Item2.Columns[i].ColumnName] == null || dataRow[tupleSave.Item2.Columns[i].ColumnName] == DBNull.Value)
				{
					continue;
				}
				switch (tupleSave.Item2.Columns[i].DataType.ToString().ToUpper())
				{
				case "SYSTEM.STRING":
					if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow[tupleSave.Item2.Columns[i].ColumnName])))
					{
						text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + "=@" + tupleSave.Item2.Columns[i].ColumnName + ",";
						list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[i].ColumnName, Function.ToString(dataRow[tupleSave.Item2.Columns[i].ColumnName])));
						num++;
					}
					break;
				case "SYSTEM.INT64":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + "=@" + tupleSave.Item2.Columns[i].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[i].ColumnName, Function.ParseInt(dataRow[tupleSave.Item2.Columns[i].ColumnName])));
					num++;
					break;
				case "SYSTEM.BYTE[]":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + "=@" + tupleSave.Item2.Columns[i].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[i].ColumnName, dataRow[tupleSave.Item2.Columns[i].ColumnName]));
					num++;
					break;
				case "SYSTEM.BOOLEAN":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + "=@" + tupleSave.Item2.Columns[i].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[i].ColumnName, Function.ParseBool(dataRow[tupleSave.Item2.Columns[i].ColumnName])));
					num++;
					break;
				case "SYSTEM.DATETIME":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + "=@" + tupleSave.Item2.Columns[i].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[i].ColumnName, Function.ParseDateTimes(dataRow[tupleSave.Item2.Columns[i].ColumnName]).TimeOfDay));
					num++;
					break;
				case "SYSTEM.DECIMAL":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + "=@" + tupleSave.Item2.Columns[i].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[i].ColumnName, Function.ParseDecimal(dataRow[tupleSave.Item2.Columns[i].ColumnName])));
					num++;
					break;
				case "SYSTEM.DOUBLE":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + "=@" + tupleSave.Item2.Columns[i].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[i].ColumnName, Function.ParseDouble(dataRow[tupleSave.Item2.Columns[i].ColumnName])));
					num++;
					break;
				case "SYSTEM.INT32":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + "=@" + tupleSave.Item2.Columns[i].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[i].ColumnName, Function.ParseInt(dataRow[tupleSave.Item2.Columns[i].ColumnName])));
					num++;
					break;
				case "SYSTEM.SINGLE":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + "=@" + tupleSave.Item2.Columns[i].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[i].ColumnName, Function.ParseDouble(dataRow[tupleSave.Item2.Columns[i].ColumnName])));
					num++;
					break;
				case "SYSTEM.INT16":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + "=@" + tupleSave.Item2.Columns[i].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[i].ColumnName, Function.ParseInt(dataRow[tupleSave.Item2.Columns[i].ColumnName])));
					num++;
					break;
				case "SYSTEM.TIMESPAN":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + "=@" + tupleSave.Item2.Columns[i].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[i].ColumnName, Function.ParseDateTimes(dataRow[tupleSave.Item2.Columns[i].ColumnName]).TimeOfDay));
					num++;
					break;
				case "SYSTEM.BYTE":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + "=@" + tupleSave.Item2.Columns[i].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[i].ColumnName, dataRow[tupleSave.Item2.Columns[i].ColumnName]));
					num++;
					break;
				default:
					if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow[tupleSave.Item2.Columns[i].ColumnName])))
					{
						text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + "=@" + tupleSave.Item2.Columns[i].ColumnName + ",";
						list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[i].ColumnName, Function.ToString(dataRow[tupleSave.Item2.Columns[i].ColumnName])));
						num++;
					}
					break;
				}
			}
			if (num > 0)
			{
				if (text2[text2.Length - 1] == ',')
				{
					text2 = text2.Remove(text2.Length - 1);
				}
				text2 += " WHERE 1=1 ";
				for (int num2 = 0; num2 < tupleSave.Item3.Count; num2++)
				{
					if (!ExistsColumn(tupleSave.Item1, tupleSave.Item3[num2].Item1) || dataRow[tupleSave.Item3[num2].Item1] == null || dataRow[tupleSave.Item3[num2].Item1] == DBNull.Value)
					{
						continue;
					}
					switch (tupleSave.Item3[num2].Item2.ToString().ToUpper())
					{
					case "SYSTEM.STRING":
						if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow[tupleSave.Item3[num2].Item1])))
						{
							text2 = text2 + " AND " + tupleSave.Item3[num2].Item1 + "=@" + tupleSave.Item3[num2].Item1;
							list.Add(new SqlParameter("@" + tupleSave.Item3[num2].Item1, Function.ToString(dataRow[tupleSave.Item3[num2].Item1])));
						}
						break;
					case "SYSTEM.INT64":
						text2 = text2 + " AND " + tupleSave.Item3[num2].Item1 + "=@" + tupleSave.Item3[num2].Item1;
						list.Add(new SqlParameter("@" + tupleSave.Item3[num2].Item1, Function.ToString(dataRow[tupleSave.Item3[num2].Item1])));
						break;
					case "SYSTEM.BYTE[]":
						text2 = text2 + " AND " + tupleSave.Item3[num2].Item1 + "=@" + tupleSave.Item3[num2].Item1;
						list.Add(new SqlParameter("@" + tupleSave.Item3[num2].Item1, dataRow[tupleSave.Item3[num2].Item1]));
						break;
					case "SYSTEM.BOOLEAN":
						text2 = text2 + " AND " + tupleSave.Item3[num2].Item1 + "=@" + tupleSave.Item3[num2].Item1;
						list.Add(new SqlParameter("@" + tupleSave.Item3[num2].Item1, dataRow[tupleSave.Item3[num2].Item1]));
						break;
					case "SYSTEM.DATETIME":
						text2 = text2 + " AND " + tupleSave.Item3[num2].Item1 + "=@" + tupleSave.Item3[num2].Item1;
						list.Add(new SqlParameter("@" + tupleSave.Item3[num2].Item1, dataRow[tupleSave.Item3[num2].Item1]));
						break;
					case "SYSTEM.DECIMAL":
						text2 = text2 + " AND " + tupleSave.Item3[num2].Item1 + "=@" + tupleSave.Item3[num2].Item1;
						list.Add(new SqlParameter("@" + tupleSave.Item3[num2].Item1, Function.ParseDecimal(dataRow[tupleSave.Item3[num2].Item1])));
						break;
					case "SYSTEM.DOUBLE":
						text2 = text2 + " AND " + tupleSave.Item3[num2].Item1 + "=@" + tupleSave.Item3[num2].Item1;
						list.Add(new SqlParameter("@" + tupleSave.Item3[num2].Item1, Function.ParseDouble(dataRow[tupleSave.Item3[num2].Item1])));
						break;
					case "SYSTEM.INT32":
						text2 = text2 + " AND " + tupleSave.Item3[num2].Item1 + "=@" + tupleSave.Item3[num2].Item1;
						list.Add(new SqlParameter("@" + tupleSave.Item3[num2].Item1, Function.ParseInt(dataRow[tupleSave.Item3[num2].Item1])));
						break;
					case "SYSTEM.SINGLE":
						text2 = text2 + " AND " + tupleSave.Item3[num2].Item1 + "=@" + tupleSave.Item3[num2].Item1;
						list.Add(new SqlParameter("@" + tupleSave.Item3[num2].Item1, Function.ParseDouble(dataRow[tupleSave.Item3[num2].Item1])));
						break;
					case "SYSTEM.INT16":
						text2 = text2 + " AND " + tupleSave.Item3[num2].Item1 + "=@" + tupleSave.Item3[num2].Item1;
						list.Add(new SqlParameter("@" + tupleSave.Item3[num2].Item1, Function.ParseInt(dataRow[tupleSave.Item3[num2].Item1])));
						break;
					case "SYSTEM.TIMESPAN":
						text2 = text2 + " AND " + tupleSave.Item3[num2].Item1 + "=@" + tupleSave.Item3[num2].Item1;
						list.Add(new SqlParameter("@" + tupleSave.Item3[num2].Item1, Function.ParseDateTimes(dataRow[tupleSave.Item3[num2].Item1]).TimeOfDay));
						break;
					case "SYSTEM.BYTE":
						text2 = text2 + " AND " + tupleSave.Item3[num2].Item1 + "=@" + tupleSave.Item3[num2].Item1;
						list.Add(new SqlParameter("@" + tupleSave.Item3[num2].Item1, dataRow[tupleSave.Item3[num2].Item1]));
						break;
					default:
						if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow[tupleSave.Item3[num2].Item1])))
						{
							text2 = text2 + " AND " + tupleSave.Item3[num2].Item1 + "=@" + tupleSave.Item3[num2].Item1;
							list.Add(new SqlParameter("@" + tupleSave.Item3[num2].Item1, Function.ToString(dataRow[tupleSave.Item3[num2].Item1])));
						}
						break;
					}
				}
				if (ExecuteNonQuery(text2, CommandType.Text, list.ToArray()) > 0)
				{
					return "";
				}
				return "Update Error";
			}
			return "No columns change to update.";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string InsertData(Tuple<string, DataTable, List<Tuple<string, Type>>> tupleSave)
	{
		string text = "";
		try
		{
			DataRow dataRow = tupleSave.Item2.Rows[0];
			switch (tupleSave.Item1.ToUpper())
			{
			case "OCRD":
				dataRow["DocEntry"] = GetNewEntry() + 1;
				break;
			case "OCPR":
				dataRow["CntctCode"] = GetNewCntctCode() + 1;
				break;
			case "OCRB":
				dataRow["AbsEntry"] = GetNewBankAccAbsEntry() + 1;
				break;
			}
			List<SqlParameter> list = new List<SqlParameter>();
			string text2 = "INSERT INTO " + tupleSave.Item1 + " (";
			for (int i = 0; i < tupleSave.Item2.Columns.Count; i++)
			{
				if (!ExistsColumn(tupleSave.Item1, tupleSave.Item2.Columns[i].ColumnName) || dataRow[tupleSave.Item2.Columns[i].ColumnName] == null || dataRow[tupleSave.Item2.Columns[i].ColumnName] == DBNull.Value || string.IsNullOrWhiteSpace(Function.ToString(dataRow[tupleSave.Item2.Columns[i].ColumnName])))
				{
					continue;
				}
				switch (tupleSave.Item2.Columns[i].DataType.ToString().ToUpper())
				{
				case "SYSTEM.STRING":
					if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow[tupleSave.Item2.Columns[i].ColumnName])))
					{
						text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + ",";
					}
					break;
				case "SYSTEM.INT64":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + ",";
					break;
				case "SYSTEM.BYTE[]":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + ",";
					break;
				case "SYSTEM.BOOLEAN":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + ",";
					break;
				case "SYSTEM.DATETIME":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + ",";
					break;
				case "SYSTEM.DECIMAL":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + ",";
					break;
				case "SYSTEM.DOUBLE":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + ",";
					break;
				case "SYSTEM.INT32":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + ",";
					break;
				case "SYSTEM.SINGLE":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + ",";
					break;
				case "SYSTEM.INT16":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + ",";
					break;
				case "SYSTEM.TIMESPAN":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + ",";
					break;
				case "SYSTEM.BYTE":
					text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + ",";
					break;
				default:
					if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow[tupleSave.Item2.Columns[i].ColumnName])))
					{
						text2 = text2 + tupleSave.Item2.Columns[i].ColumnName + ",";
					}
					break;
				}
			}
			if (text2[text2.Length - 1] == ',')
			{
				text2 = text2.Remove(text2.Length - 1);
			}
			text2 += ") VALUES (";
			for (int j = 0; j < tupleSave.Item2.Columns.Count; j++)
			{
				if (!ExistsColumn(tupleSave.Item1, tupleSave.Item2.Columns[j].ColumnName) || dataRow[tupleSave.Item2.Columns[j].ColumnName] == null || dataRow[tupleSave.Item2.Columns[j].ColumnName] == DBNull.Value)
				{
					continue;
				}
				switch (tupleSave.Item2.Columns[j].DataType.ToString().ToUpper())
				{
				case "SYSTEM.STRING":
					if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow[tupleSave.Item2.Columns[j].ColumnName])))
					{
						text2 = text2 + "@" + tupleSave.Item2.Columns[j].ColumnName + ",";
						list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[j].ColumnName, Function.ToString(dataRow[tupleSave.Item2.Columns[j].ColumnName])));
					}
					break;
				case "SYSTEM.INT64":
					text2 = text2 + "@" + tupleSave.Item2.Columns[j].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[j].ColumnName, Function.ParseInt(dataRow[tupleSave.Item2.Columns[j].ColumnName])));
					break;
				case "SYSTEM.BYTE[]":
					text2 = text2 + "@" + tupleSave.Item2.Columns[j].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[j].ColumnName, dataRow[tupleSave.Item2.Columns[j].ColumnName]));
					break;
				case "SYSTEM.BOOLEAN":
					text2 = text2 + "@" + tupleSave.Item2.Columns[j].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[j].ColumnName, Function.ParseBool(dataRow[tupleSave.Item2.Columns[j].ColumnName])));
					break;
				case "SYSTEM.DATETIME":
					text2 = text2 + "@" + tupleSave.Item2.Columns[j].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[j].ColumnName, Function.ParseDateTimes(dataRow[tupleSave.Item2.Columns[j].ColumnName]).TimeOfDay));
					break;
				case "SYSTEM.DECIMAL":
					text2 = text2 + "@" + tupleSave.Item2.Columns[j].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[j].ColumnName, Function.ParseDecimal(dataRow[tupleSave.Item2.Columns[j].ColumnName])));
					break;
				case "SYSTEM.DOUBLE":
					text2 = text2 + "@" + tupleSave.Item2.Columns[j].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[j].ColumnName, Function.ParseDouble(dataRow[tupleSave.Item2.Columns[j].ColumnName])));
					break;
				case "SYSTEM.INT32":
					text2 = text2 + "@" + tupleSave.Item2.Columns[j].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[j].ColumnName, Function.ParseInt(dataRow[tupleSave.Item2.Columns[j].ColumnName])));
					break;
				case "SYSTEM.SINGLE":
					text2 = text2 + "@" + tupleSave.Item2.Columns[j].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[j].ColumnName, Function.ParseDouble(dataRow[tupleSave.Item2.Columns[j].ColumnName])));
					break;
				case "SYSTEM.INT16":
					text2 = text2 + "@" + tupleSave.Item2.Columns[j].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[j].ColumnName, Function.ParseInt(dataRow[tupleSave.Item2.Columns[j].ColumnName])));
					break;
				case "SYSTEM.TIMESPAN":
					text2 = text2 + "@" + tupleSave.Item2.Columns[j].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[j].ColumnName, Function.ParseDateTimes(dataRow[tupleSave.Item2.Columns[j].ColumnName]).TimeOfDay));
					break;
				case "SYSTEM.BYTE":
					text2 = text2 + "@" + tupleSave.Item2.Columns[j].ColumnName + ",";
					list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[j].ColumnName, dataRow[tupleSave.Item2.Columns[j].ColumnName]));
					break;
				default:
					if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow[tupleSave.Item2.Columns[j].ColumnName])))
					{
						text2 = text2 + "@" + tupleSave.Item2.Columns[j].ColumnName + ",";
						list.Add(new SqlParameter("@" + tupleSave.Item2.Columns[j].ColumnName, Function.ToString(dataRow[tupleSave.Item2.Columns[j].ColumnName])));
					}
					break;
				}
			}
			if (text2[text2.Length - 1] == ',')
			{
				text2 = text2.Remove(text2.Length - 1);
			}
			text2 += ")";
			if (ExecuteNonQuery(text2, CommandType.Text, list.ToArray()) > 0)
			{
				return "";
			}
			return "Insert Error";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable CheckData(Tuple<string, DataTable, List<Tuple<string, Type>>> tupleCheck)
	{
		DataTable dataTable = new DataTable();
		try
		{
			for (int i = 0; i < tupleCheck.Item3.Count; i++)
			{
				dataTable.Columns.Add(tupleCheck.Item3[i].Item1, tupleCheck.Item3[i].Item2);
			}
			dataTable.Columns.Add("MyStatus", typeof(string));
			for (int j = 0; j < tupleCheck.Item2.Rows.Count; j++)
			{
				DataRow dataRow = dataTable.NewRow();
				for (int k = 0; k < tupleCheck.Item3.Count; k++)
				{
					dataRow[tupleCheck.Item3[k].Item1] = tupleCheck.Item2.Rows[j][tupleCheck.Item3[k].Item1];
				}
				if (ExistsALine(tupleCheck.Item3, dataRow, tupleCheck.Item1))
				{
					dataRow["MyStatus"] = "Update";
				}
				else
				{
					dataRow["MyStatus"] = "Insert";
				}
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return dataTable;
	}

	private int GetNewEntry()
	{
		int result = 0;
		try
		{
			string query = "SELECT MAX(isnull(docentry,0)) FROM OCRD";
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

	private int GetNewCntctCode()
	{
		int result = 0;
		try
		{
			string query = "SELECT MAX(isnull(CntctCode,0)) FROM OCPR";
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

	private int GetNewBankAccAbsEntry()
	{
		int result = 0;
		try
		{
			string query = "SELECT MAX(isnull(AbsEntry,0)) FROM OCRB";
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
}
