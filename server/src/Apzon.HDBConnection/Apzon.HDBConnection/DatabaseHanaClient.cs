using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Apzon.HDBConnection.Interfaces;
using Sap.Data.Hana;

namespace Apzon.HDBConnection;

public class DatabaseHanaClient : IDatabaseClient
{
	public string ConnectionString { get; set; }

	public string SchemaName { get; set; }

	public HanaConnection DbConnection { get; set; }

	public HanaTransaction DbTransaction { get; set; }

	public DatabaseHanaClient(string connectionString)
	{
		if (connectionString == null)
		{
			throw new ArgumentNullException("connectionString");
		}
		ConnectionString = $"{connectionString}; Pooling=false;Max Pool Size=50;Min Pool Size=5";
		DbTransaction = null;
	}

	public DatabaseHanaClient(HanaConnection connection)
	{
		if (connection == null)
		{
			throw new ArgumentNullException("connection");
		}
		DbConnection = connection;
		DbTransaction = null;
	}

	public virtual string InsertDataTableIntoSql(DataTable dt)
	{
		using HanaConnection hanaConnection = new HanaConnection(ConnectionString);
		if (hanaConnection.State == ConnectionState.Closed)
		{
			hanaConnection.Open();
		}
		using HanaBulkCopy hanaBulkCopy = new HanaBulkCopy(ConnectionString);
		hanaBulkCopy.DestinationTableName = "\"" + dt.TableName + "\"";
		foreach (object column in dt.Columns)
		{
			hanaBulkCopy.ColumnMappings.Add(column.ToString(), column.ToString());
		}
		hanaBulkCopy.WriteToServer(dt);
		dt.Rows.Clear();
		return "";
	}

	public int ExecuteNonQueryNotParse(string query, CommandType commandType = CommandType.Text)
	{
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (HanaCommand hanaCommand = DbConnection.CreateCommand())
			{
				hanaCommand.CommandTimeout = 60000;
				hanaCommand.CommandText = query;
				hanaCommand.CommandType = commandType;
				hanaCommand.Transaction = DbTransaction;
				return hanaCommand.ExecuteNonQuery();
			}
		}
		using HanaConnection hanaConnection = new HanaConnection(ConnectionString);
		if (hanaConnection.State == ConnectionState.Closed)
		{
			hanaConnection.Open();
		}
		HanaCommand hanaCommand2 = hanaConnection.CreateCommand();
		hanaCommand2.CommandTimeout = 60000;
		hanaCommand2.CommandText = query;
		hanaCommand2.CommandType = commandType;
		return hanaCommand2.ExecuteNonQuery();
	}

	public int ExecuteNonQuery(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		HanaParameter[] array = ((parameters == null) ? null : new HanaParameter[parameters.Length]);
		query = ParseParamater(array, parameters, commandType, query);
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (HanaCommand hanaCommand = DbConnection.CreateCommand())
			{
				hanaCommand.CommandTimeout = 60000;
				hanaCommand.CommandText = query;
				hanaCommand.CommandType = commandType;
				hanaCommand.Transaction = DbTransaction;
				hanaCommand.TryAddParameters(array);
				return hanaCommand.ExecuteNonQuery();
			}
		}
		using HanaConnection hanaConnection = new HanaConnection(ConnectionString);
		if (hanaConnection.State == ConnectionState.Closed)
		{
			hanaConnection.Open();
		}
		HanaCommand hanaCommand2 = hanaConnection.CreateCommand();
		hanaCommand2.CommandTimeout = 60000;
		hanaCommand2.CommandText = query;
		hanaCommand2.CommandType = commandType;
		hanaCommand2.TryAddParameters(array);
		return hanaCommand2.ExecuteNonQuery();
	}

	public int ExecuteNonQueryNoParam(string query, CommandType commandType = CommandType.Text)
	{
		query = ParseParamater(null, null, commandType, query);
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (HanaCommand hanaCommand = DbConnection.CreateCommand())
			{
				hanaCommand.CommandTimeout = 60000;
				hanaCommand.CommandText = query;
				hanaCommand.Transaction = DbTransaction;
				hanaCommand.CommandType = commandType;
				return hanaCommand.ExecuteNonQuery();
			}
		}
		using HanaConnection hanaConnection = new HanaConnection(ConnectionString);
		if (hanaConnection.State == ConnectionState.Closed)
		{
			hanaConnection.Open();
		}
		HanaCommand hanaCommand2 = hanaConnection.CreateCommand();
		hanaCommand2.CommandTimeout = 60000;
		hanaCommand2.CommandText = query;
		hanaCommand2.CommandType = commandType;
		return hanaCommand2.ExecuteNonQuery();
	}

	public object ExecuteScalar(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		HanaParameter[] array = ((parameters == null) ? null : new HanaParameter[parameters.Length]);
		query = ParseParamater(array, parameters, commandType, query);
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (HanaCommand hanaCommand = DbConnection.CreateCommand())
			{
				hanaCommand.CommandTimeout = 60000;
				hanaCommand.CommandText = query;
				hanaCommand.Transaction = DbTransaction;
				hanaCommand.CommandType = commandType;
				hanaCommand.TryAddParameters(array);
				return hanaCommand.ExecuteScalar();
			}
		}
		using HanaConnection hanaConnection = new HanaConnection(ConnectionString);
		if (hanaConnection.State == ConnectionState.Closed)
		{
			hanaConnection.Open();
		}
		HanaCommand hanaCommand2 = hanaConnection.CreateCommand();
		hanaCommand2.CommandTimeout = 60000;
		hanaCommand2.CommandText = query;
		hanaCommand2.CommandType = commandType;
		hanaCommand2.TryAddParameters(array);
		return hanaCommand2.ExecuteScalar();
	}

	public DataSet ExecuteData(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		HanaParameter[] array = ((parameters == null) ? null : new HanaParameter[parameters.Length]);
		query = ParseParamater(array, parameters, commandType, query);
		DataSet dataSet = new DataSet();
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (HanaCommand hanaCommand = DbConnection.CreateCommand())
			{
				hanaCommand.CommandTimeout = 60000;
				hanaCommand.CommandText = query;
				hanaCommand.Transaction = DbTransaction;
				hanaCommand.CommandType = commandType;
				hanaCommand.TryAddParameters(array);
				new HanaDataAdapter(hanaCommand).Fill(dataSet);
				hanaCommand.Parameters.Clear();
				return dataSet;
			}
		}
		using HanaConnection hanaConnection = new HanaConnection(ConnectionString);
		if (hanaConnection.State == ConnectionState.Closed)
		{
			hanaConnection.Open();
		}
		using HanaCommand hanaCommand2 = hanaConnection.CreateCommand();
		hanaCommand2.CommandTimeout = 60000;
		hanaCommand2.CommandText = query;
		hanaCommand2.CommandType = commandType;
		hanaCommand2.TryAddParameters(array);
		new HanaDataAdapter(hanaCommand2).Fill(dataSet);
		hanaCommand2.Parameters.Clear();
		return dataSet;
	}

	public DataTable ExecuteDataTable(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		HanaParameter[] array = ((parameters == null) ? null : new HanaParameter[parameters.Length]);
		query = ParseParamater(array, parameters, commandType, query);
		DataTable dataTable = new DataTable();
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (HanaCommand hanaCommand = DbConnection.CreateCommand())
			{
				hanaCommand.CommandTimeout = 60000;
				hanaCommand.CommandText = query;
				hanaCommand.CommandType = commandType;
				hanaCommand.Transaction = DbTransaction;
				hanaCommand.TryAddParameters(array);
				new HanaDataAdapter(hanaCommand).Fill(dataTable);
				hanaCommand.Parameters.Clear();
				return dataTable;
			}
		}
		using HanaConnection hanaConnection = new HanaConnection(ConnectionString);
		if (hanaConnection.State == ConnectionState.Closed)
		{
			hanaConnection.Open();
		}
		using HanaCommand hanaCommand2 = hanaConnection.CreateCommand();
		hanaCommand2.CommandTimeout = 60000;
		hanaCommand2.CommandText = query;
		hanaCommand2.CommandType = commandType;
		hanaCommand2.TryAddParameters(array);
		new HanaDataAdapter(hanaCommand2).Fill(dataTable);
		hanaCommand2.Parameters.Clear();
		hanaConnection.Dispose();
		return dataTable;
	}

	public DataTable ExecuteDataTable(string query, string structTable, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		HanaParameter[] array = ((parameters == null) ? null : new HanaParameter[parameters.Length]);
		query = ParseParamater(array, parameters, commandType, query);
		DataTable dataTable = new DataTable();
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (HanaCommand hanaCommand = DbConnection.CreateCommand())
			{
				hanaCommand.CommandTimeout = 60000;
				hanaCommand.CommandText = query;
				hanaCommand.Transaction = DbTransaction;
				hanaCommand.CommandType = commandType;
				hanaCommand.TryAddParameters(array);
				new HanaDataAdapter(hanaCommand).Fill(dataTable);
				hanaCommand.Parameters.Clear();
				if (!string.IsNullOrEmpty(structTable))
				{
					SetDefaultValueToTable(structTable, dataTable);
				}
				return dataTable;
			}
		}
		using HanaConnection hanaConnection = new HanaConnection(ConnectionString);
		if (hanaConnection.State == ConnectionState.Closed)
		{
			hanaConnection.Open();
		}
		using HanaCommand hanaCommand2 = hanaConnection.CreateCommand();
		hanaCommand2.CommandTimeout = 60000;
		hanaCommand2.CommandText = query;
		hanaCommand2.CommandType = commandType;
		hanaCommand2.TryAddParameters(array);
		new HanaDataAdapter(hanaCommand2).Fill(dataTable);
		if (!string.IsNullOrEmpty(structTable))
		{
			SetDefaultValueToTable(structTable, dataTable);
		}
		hanaCommand2.Parameters.Clear();
		return dataTable;
	}

	public void CreateTempTable(DataTable dt, string tblName)
	{
		string text = "";
		List<string> list = new List<string>();
		for (int i = 0; i < dt.Columns.Count; i++)
		{
			DataColumn col = dt.Columns[i];
			string text2 = dt.AsEnumerable().Max((DataRow t) => (t[col] != null) ? t[col].ToString() : "");
			string item = string.Format("\"{0}\" {1} {2}", col.ColumnName, GetSqlType(col, text2?.Length ?? 50), col.AllowDBNull ? "NULL" : "");
			list.Add(item);
		}
		text += string.Format(" CREATE local temporary TABLE \"{0}\"({1});", tblName, string.Join(",", list.ToArray()));
		if (DbConnection != null)
		{
			if (string.IsNullOrEmpty(DbConnection.ConnectionString))
			{
				DbConnection.ConnectionString = ConnectionString;
			}
			if (DbConnection.State == ConnectionState.Closed)
			{
				DbConnection.Open();
			}
		}
		ExecuteNonQuery(text, CommandType.Text);
	}

	private string GetSqlType(DataColumn col, int columnLength)
	{
		HanaDbType dBType = GetDBType(col.DataType);
		if (col.DataType == typeof(string))
		{
			if (columnLength >= 5001)
			{
				return "nclob";
			}
			return dBType.ToString() + "(5000)";
		}
		if (col.DataType == typeof(char))
		{
			return "CHAR(1)";
		}
		if (dBType == HanaDbType.Decimal)
		{
			return dBType.ToString() + " (19,6)";
		}
		return dBType.ToString();
	}

	private HanaDbType GetDBType(Type theType)
	{
		HanaParameter hanaParameter = new HanaParameter();
		TypeConverter converter = TypeDescriptor.GetConverter(hanaParameter.DbType);
		if (converter.CanConvertFrom(theType))
		{
			object obj = converter.ConvertFrom(theType.Name);
			if (obj != null)
			{
				hanaParameter.DbType = (DbType)obj;
			}
		}
		else
		{
			try
			{
				object obj2 = converter.ConvertFrom(theType.Name);
				if (obj2 != null)
				{
					hanaParameter.DbType = (DbType)obj2;
				}
			}
			catch (Exception)
			{
				if (theType.Name.Equals("TimeSpan"))
				{
					hanaParameter.DbType = DbType.Time;
				}
			}
		}
		return hanaParameter.HanaDbType;
	}

	public void BulkCopy(DataTable dt, string tblName)
	{
		if (DbConnection != null)
		{
			if (string.IsNullOrEmpty(DbConnection.ConnectionString))
			{
				DbConnection.ConnectionString = ConnectionString;
			}
			if (DbConnection.State == ConnectionState.Closed)
			{
				DbConnection.Open();
			}
			using HanaBulkCopy hanaBulkCopy = new HanaBulkCopy(DbConnection, HanaBulkCopyOptions.Default, DbTransaction);
			hanaBulkCopy.DestinationTableName = tblName;
			hanaBulkCopy.BatchSize = 1000;
			hanaBulkCopy.BulkCopyTimeout = 60000;
			hanaBulkCopy.WriteToServer(dt);
			hanaBulkCopy.Close();
			return;
		}
		using HanaBulkCopy hanaBulkCopy2 = new HanaBulkCopy(ConnectionString);
		hanaBulkCopy2.DestinationTableName = tblName;
		hanaBulkCopy2.BatchSize = 1000;
		hanaBulkCopy2.BulkCopyTimeout = 60000;
		hanaBulkCopy2.WriteToServer(dt);
		hanaBulkCopy2.Close();
	}

	public virtual void SetDefaultValueToTable(string table, DataTable dt)
	{
		if (dt == null || dt.Columns.Count == 0)
		{
			return;
		}
		DataTable dataTable = ExecuteDataTable("SELECT COLUMN_NAME,DEFAULT_VALUE COLUMN_DEFAULT FROM TABLE_COLUMNS WHERE SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME= @tblName and DEFAULT_VALUE is not null;", CommandType.Text, new SqlParameter("@tblName", table));
		if (dataTable == null || dataTable.Rows.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < dataTable.Rows.Count; i++)
		{
			string text = dataTable.Rows[i]["COLUMN_NAME"].To_String();
			for (int j = 0; j < dt.Columns.Count; j++)
			{
				if (dt.Columns[j].ColumnName == text)
				{
					dt.Columns[j].DefaultValue = dataTable.Rows[i]["COLUMN_DEFAULT"];
					break;
				}
			}
		}
	}

	public virtual DataTable GetTableStruct(string tblName)
	{
		DataTable dataTable = ExecuteDataTable("SELECT \"COLUMN_NAME\",DEFAULT_VALUE COLUMN_DEFAULT,\"IS_NULLABLE\",DATA_TYPE_NAME DATA_TYPE FROM TABLE_COLUMNS WHERE SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = UPPER(@tblName) order by position;", CommandType.Text, new SqlParameter("@tblName", tblName));
		DataTable dataTable2 = new DataTable
		{
			TableName = tblName
		};
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				string text = dataTable.Rows[i]["DATA_TYPE"].To_String().ToLower();
				string columnName = dataTable.Rows[i]["COLUMN_NAME"].To_String();
				DataColumn dataColumn = new DataColumn
				{
					ColumnName = columnName
				};
				Type typeFromHandle;
				switch (text)
				{
				case "int":
				case "integer":
				case "bigint":
				case "smallint":
				case "tinyint":
					typeFromHandle = typeof(int);
					break;
				case "decimal":
				case "smalldecimal":
					typeFromHandle = typeof(decimal);
					break;
				case "datetime":
				case "date":
				case "timestamp":
					typeFromHandle = typeof(DateTime);
					break;
				case "numeric":
				case "float":
				case "real":
					typeFromHandle = typeof(double);
					break;
				case "bit":
					typeFromHandle = typeof(bool);
					break;
				default:
					typeFromHandle = typeof(string);
					break;
				}
				dataColumn.DataType = typeFromHandle;
				if (dataTable.Rows[i]["COLUMN_DEFAULT"].ToString().ToUpper() == "CURRENT_DATE")
				{
					dataColumn.DefaultValue = DateTime.Now;
				}
				else
				{
					dataColumn.DefaultValue = dataTable.Rows[i]["COLUMN_DEFAULT"];
				}
				dataTable2.Columns.Add(dataColumn);
			}
		}
		if (dataTable2.Columns.Count > 0)
		{
			dataTable2.Rows.Add(dataTable2.NewRow());
			return dataTable2;
		}
		return new DataTable();
	}

	public DbConnection OpenConnection()
	{
		DbConnection = DbConnection ?? new HanaConnection(ConnectionString);
		if (DbConnection.State == ConnectionState.Closed)
		{
			DbConnection.Open();
		}
		return DbConnection;
	}

	public DbTransaction BeginTransaction(IsolationLevel level = IsolationLevel.RepeatableRead)
	{
		DbTransaction = DbConnection.BeginTransaction(level);
		return DbTransaction;
	}

	private string ParseParamater(HanaParameter[] arrHanaPara, SqlParameter[] arrSqlPara, CommandType commandType, string hanaQuery)
	{
		if (commandType == CommandType.Text)
		{
			string text = "";
			if (arrSqlPara != null && arrSqlPara.Length != 0)
			{
				text = "(";
				for (int i = 0; i < arrSqlPara.Length; i++)
				{
					hanaQuery = hanaQuery.Replace(arrSqlPara[i].ParameterName, "$" + arrSqlPara[i].ParameterName.Substring(1));
					arrHanaPara[i] = new HanaParameter(arrSqlPara[i].ParameterName.Substring(1), arrSqlPara[i].Value);
					string text2 = arrHanaPara[i].HanaDbType.ToString();
					string text3 = text2.ToUpper();
					if (!(text3 == "NVARCHAR"))
					{
						if (text3 == "DECIMAL")
						{
							text2 += "(19,6)";
						}
					}
					else
					{
						text2 += ((arrHanaPara[i].Value.To_String().Length < 4000) ? "(4000)" : $"({arrHanaPara[i].Value.To_String().Length + 10})");
					}
					text += $"in {arrHanaPara[i].ParameterName} {text2} => ?,";
				}
				text = $"{text.Substring(0, text.Length - 1)})";
			}
			string text4 = hanaQuery.ToLower().Trim();
			if (text4.StartsWith("do ") || text4.StartsWith("do\n") || text4.StartsWith("do\t") || text4.StartsWith("do\r") || (string.IsNullOrEmpty(text) && !text4.Contains("declare ") && text4.Split(';').Length <= 2))
			{
				return hanaQuery;
			}
			return string.Format("DO {1}\r\n                                                                        BEGIN \r\n                                                                                {0}\r\n                                                                           end;", hanaQuery, text);
		}
		if (arrSqlPara != null && arrSqlPara.Length != 0)
		{
			for (int j = 0; j < arrSqlPara.Length; j++)
			{
				arrHanaPara[j] = new HanaParameter(arrSqlPara[j].ParameterName.Substring(1), arrSqlPara[j].Value);
			}
		}
		return hanaQuery;
	}
}
