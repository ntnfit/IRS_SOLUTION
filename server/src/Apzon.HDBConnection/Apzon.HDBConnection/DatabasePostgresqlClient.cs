using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Apzon.HDBConnection.Interfaces;
using Npgsql;
using NpgsqlTypes;
using Z.BulkOperations;

namespace Apzon.HDBConnection;

public class DatabasePostgresqlClient : IDatabaseClient
{
	public string ConnectionString { get; set; }

	public string SchemaName { get; set; }

	public NpgsqlConnection DbConnection { get; set; }

	public NpgsqlTransaction DbTransaction { get; set; }

	public DatabasePostgresqlClient(string connectionString, string schemaName)
	{
		if (connectionString == null)
		{
			throw new ArgumentNullException("connectionString");
		}
		if (schemaName == null)
		{
			throw new ArgumentNullException("schemaName");
		}
		ConnectionString = connectionString;
		SchemaName = schemaName;
		DbTransaction = null;
	}

	public DatabasePostgresqlClient(NpgsqlConnection connection, string schemaName)
	{
		if (connection == null)
		{
			throw new ArgumentNullException("connection");
		}
		if (schemaName == null)
		{
			throw new ArgumentNullException("schemaName");
		}
		DbConnection = connection;
		SchemaName = schemaName;
		DbTransaction = null;
	}

	public virtual string InsertDataTableIntoSql(DataTable dt)
	{
		using NpgsqlConnection npgsqlConnection = new NpgsqlConnection(ConnectionString);
		if (npgsqlConnection.State == ConnectionState.Closed)
		{
			npgsqlConnection.Open();
		}
		SetConnectionSchema(npgsqlConnection);
		using BulkOperation bulkOperation = new BulkOperation(npgsqlConnection);
		bulkOperation.DestinationTableName = "\"" + dt.TableName + "\"";
		foreach (object column in dt.Columns)
		{
			bulkOperation.ColumnMappings.Add(column.ToString(), column.ToString());
		}
		bulkOperation.BulkInsert(dt);
		dt.Rows.Clear();
		return "";
	}

	public int ExecuteNonQueryNotParse(string query, CommandType commandType = CommandType.Text)
	{
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (NpgsqlCommand npgsqlCommand = DbConnection.CreateCommand())
			{
				npgsqlCommand.CommandTimeout = 60000;
				npgsqlCommand.CommandText = query;
				npgsqlCommand.CommandType = commandType;
				npgsqlCommand.Transaction = DbTransaction;
				return npgsqlCommand.ExecuteNonQuery();
			}
		}
		using NpgsqlConnection npgsqlConnection = new NpgsqlConnection(ConnectionString);
		if (npgsqlConnection.State == ConnectionState.Closed)
		{
			npgsqlConnection.Open();
		}
		SetConnectionSchema(npgsqlConnection);
		NpgsqlCommand npgsqlCommand2 = npgsqlConnection.CreateCommand();
		npgsqlCommand2.CommandTimeout = 60000;
		npgsqlCommand2.CommandText = query;
		npgsqlCommand2.CommandType = commandType;
		return npgsqlCommand2.ExecuteNonQuery();
	}

	public int ExecuteNonQuery(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		NpgsqlParameter[] parameters2 = ParseParamater(parameters);
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (NpgsqlCommand npgsqlCommand = DbConnection.CreateCommand())
			{
				npgsqlCommand.CommandTimeout = 60000;
				npgsqlCommand.CommandText = query;
				npgsqlCommand.CommandType = commandType;
				npgsqlCommand.Transaction = DbTransaction;
				npgsqlCommand.TryAddParameters(parameters2);
				return npgsqlCommand.ExecuteNonQuery();
			}
		}
		using NpgsqlConnection npgsqlConnection = new NpgsqlConnection(ConnectionString);
		if (npgsqlConnection.State == ConnectionState.Closed)
		{
			npgsqlConnection.Open();
		}
		SetConnectionSchema(npgsqlConnection);
		NpgsqlCommand npgsqlCommand2 = npgsqlConnection.CreateCommand();
		npgsqlCommand2.CommandTimeout = 60000;
		npgsqlCommand2.CommandText = query;
		npgsqlCommand2.CommandType = commandType;
		npgsqlCommand2.TryAddParameters(parameters2);
		return npgsqlCommand2.ExecuteNonQuery();
	}

	public int ExecuteNonQueryNoParam(string query, CommandType commandType = CommandType.Text)
	{
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (NpgsqlCommand npgsqlCommand = DbConnection.CreateCommand())
			{
				npgsqlCommand.CommandTimeout = 60000;
				npgsqlCommand.CommandText = query;
				npgsqlCommand.Transaction = DbTransaction;
				npgsqlCommand.CommandType = commandType;
				return npgsqlCommand.ExecuteNonQuery();
			}
		}
		using NpgsqlConnection npgsqlConnection = new NpgsqlConnection(ConnectionString);
		if (npgsqlConnection.State == ConnectionState.Closed)
		{
			npgsqlConnection.Open();
		}
		SetConnectionSchema(npgsqlConnection);
		NpgsqlCommand npgsqlCommand2 = npgsqlConnection.CreateCommand();
		npgsqlCommand2.CommandTimeout = 60000;
		npgsqlCommand2.CommandText = query;
		npgsqlCommand2.CommandType = commandType;
		return npgsqlCommand2.ExecuteNonQuery();
	}

	public object ExecuteScalar(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		NpgsqlParameter[] parameters2 = ParseParamater(parameters);
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (NpgsqlCommand npgsqlCommand = DbConnection.CreateCommand())
			{
				npgsqlCommand.CommandTimeout = 60000;
				npgsqlCommand.CommandText = query;
				npgsqlCommand.Transaction = DbTransaction;
				npgsqlCommand.CommandType = commandType;
				npgsqlCommand.TryAddParameters(parameters2);
				return npgsqlCommand.ExecuteScalar();
			}
		}
		using NpgsqlConnection npgsqlConnection = new NpgsqlConnection(ConnectionString);
		if (npgsqlConnection.State == ConnectionState.Closed)
		{
			npgsqlConnection.Open();
		}
		SetConnectionSchema(npgsqlConnection);
		NpgsqlCommand npgsqlCommand2 = npgsqlConnection.CreateCommand();
		npgsqlCommand2.CommandTimeout = 60000;
		npgsqlCommand2.CommandText = query;
		npgsqlCommand2.CommandType = commandType;
		npgsqlCommand2.TryAddParameters(parameters2);
		return npgsqlCommand2.ExecuteScalar();
	}

	public DataSet ExecuteData(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		NpgsqlParameter[] parameters2 = ParseParamater(parameters);
		DataSet dataSet = new DataSet();
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (NpgsqlCommand npgsqlCommand = DbConnection.CreateCommand())
			{
				npgsqlCommand.CommandTimeout = 60000;
				npgsqlCommand.CommandText = query;
				npgsqlCommand.CommandType = commandType;
				npgsqlCommand.Transaction = DbTransaction;
				npgsqlCommand.TryAddParameters(parameters2);
				NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(npgsqlCommand);
				if (commandType == CommandType.Text)
				{
					npgsqlDataAdapter.Fill(dataSet);
				}
				else
				{
					DataTable dataTable = new DataTable();
					npgsqlDataAdapter.Fill(dataTable);
					if (dataTable.Columns.Count == 1)
					{
						npgsqlCommand.CommandType = CommandType.Text;
						npgsqlCommand.CommandText = "select lower(data_type) from information_schema.routines where routine_type = 'FUNCTION' and routine_name = @function";
						npgsqlCommand.Parameters.Clear();
						npgsqlCommand.TryAddParameters(new NpgsqlParameter("@function", query));
						if (ToString(npgsqlCommand.ExecuteScalar()) == "refcursor")
						{
							if (dataTable.Rows.Count == 0)
							{
								return new DataSet();
							}
							query = "";
							for (int i = 0; i < dataTable.Rows.Count; i++)
							{
								query += $"FETCH ALL IN \"{dataTable.Rows[i][0]}\"; ";
							}
							npgsqlCommand.CommandText = query;
							npgsqlCommand.CommandType = CommandType.Text;
							npgsqlCommand.Parameters.Clear();
							npgsqlDataAdapter.Fill(dataSet);
							return dataSet;
						}
					}
					dataSet.Tables.Add(dataTable);
				}
			}
			return dataSet;
		}
		using NpgsqlConnection npgsqlConnection = new NpgsqlConnection(ConnectionString);
		if (npgsqlConnection.State == ConnectionState.Closed)
		{
			npgsqlConnection.Open();
		}
		SetConnectionSchema(npgsqlConnection);
		using (NpgsqlCommand npgsqlCommand2 = npgsqlConnection.CreateCommand())
		{
			npgsqlCommand2.CommandTimeout = 60000;
			npgsqlCommand2.CommandText = query;
			npgsqlCommand2.CommandType = commandType;
			npgsqlCommand2.TryAddParameters(parameters2);
			NpgsqlDataAdapter npgsqlDataAdapter2 = new NpgsqlDataAdapter(npgsqlCommand2);
			if (commandType == CommandType.Text)
			{
				npgsqlDataAdapter2.Fill(dataSet);
			}
			else
			{
				NpgsqlTransaction npgsqlTransaction = (npgsqlCommand2.Transaction = npgsqlConnection.BeginTransaction());
				DataTable dataTable2 = new DataTable();
				npgsqlDataAdapter2.Fill(dataTable2);
				if (dataTable2.Columns.Count == 1)
				{
					npgsqlCommand2.CommandType = CommandType.Text;
					npgsqlCommand2.CommandText = "select lower(data_type) from information_schema.routines where routine_type = 'FUNCTION' and routine_name = @function";
					npgsqlCommand2.Parameters.Clear();
					npgsqlCommand2.TryAddParameters(new NpgsqlParameter("@function", query));
					if (ToString(npgsqlCommand2.ExecuteScalar()) == "refcursor")
					{
						if (dataTable2.Rows.Count == 0)
						{
							return new DataSet();
						}
						query = "";
						for (int j = 0; j < dataTable2.Rows.Count; j++)
						{
							query += $"FETCH ALL IN \"{dataTable2.Rows[j][0]}\"; ";
						}
						npgsqlCommand2.CommandText = query;
						npgsqlCommand2.CommandType = CommandType.Text;
						npgsqlCommand2.Parameters.Clear();
						npgsqlDataAdapter2.Fill(dataSet);
						npgsqlTransaction.Commit();
						return dataSet;
					}
				}
				dataSet.Tables.Add(dataTable2);
				npgsqlTransaction.Commit();
			}
		}
		return dataSet;
	}

	public DataTable ExecuteDataTable(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		NpgsqlParameter[] parameters2 = ParseParamater(parameters);
		DataTable dataTable = new DataTable();
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (NpgsqlCommand npgsqlCommand = DbConnection.CreateCommand())
			{
				npgsqlCommand.CommandTimeout = 60000;
				npgsqlCommand.CommandText = query;
				npgsqlCommand.CommandType = commandType;
				npgsqlCommand.Transaction = DbTransaction;
				npgsqlCommand.TryAddParameters(parameters2);
				NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(npgsqlCommand);
				npgsqlDataAdapter.Fill(dataTable);
				npgsqlCommand.Parameters.Clear();
				if (commandType == CommandType.Text || dataTable.Columns.Count > 1)
				{
					return dataTable;
				}
				npgsqlCommand.CommandType = CommandType.Text;
				npgsqlCommand.CommandText = "select lower(data_type) from information_schema.routines where routine_type = 'FUNCTION' and routine_name = @function";
				npgsqlCommand.TryAddParameters(new NpgsqlParameter("@function", query));
				if (ToString(npgsqlCommand.ExecuteScalar()) == "refcursor")
				{
					if (dataTable.Rows.Count == 0)
					{
						return new DataTable();
					}
					npgsqlCommand.CommandText = "FETCH ALL IN \"" + ToString(dataTable.Rows[0][0]) + "\";";
					npgsqlCommand.CommandType = CommandType.Text;
					npgsqlCommand.Parameters.Clear();
					dataTable = new DataTable();
					npgsqlDataAdapter.Fill(dataTable);
				}
				return dataTable;
			}
		}
		using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(ConnectionString))
		{
			if (npgsqlConnection.State == ConnectionState.Closed)
			{
				npgsqlConnection.Open();
			}
			SetConnectionSchema(npgsqlConnection);
			using NpgsqlCommand npgsqlCommand2 = npgsqlConnection.CreateCommand();
			npgsqlCommand2.CommandTimeout = 60000;
			npgsqlCommand2.CommandText = query;
			npgsqlCommand2.CommandType = commandType;
			npgsqlCommand2.TryAddParameters(parameters2);
			NpgsqlDataAdapter npgsqlDataAdapter2 = new NpgsqlDataAdapter(npgsqlCommand2);
			if (commandType == CommandType.Text)
			{
				npgsqlDataAdapter2.Fill(dataTable);
			}
			else
			{
				NpgsqlTransaction npgsqlTransaction = (npgsqlCommand2.Transaction = npgsqlConnection.BeginTransaction());
				npgsqlDataAdapter2.Fill(dataTable);
				npgsqlCommand2.Parameters.Clear();
				if (dataTable.Columns.Count == 1)
				{
					npgsqlCommand2.CommandType = CommandType.Text;
					npgsqlCommand2.CommandText = "select lower(data_type) from information_schema.routines where routine_type = 'FUNCTION' and routine_name = @function;";
					npgsqlCommand2.TryAddParameters(new NpgsqlParameter("@function", query));
					if (ToString(npgsqlCommand2.ExecuteScalar()) == "refcursor")
					{
						if (dataTable.Rows.Count == 0)
						{
							return new DataTable();
						}
						npgsqlCommand2.CommandText = "FETCH ALL IN \"" + ToString(dataTable.Rows[0][0]) + "\";";
						npgsqlCommand2.Parameters.Clear();
						npgsqlCommand2.CommandType = CommandType.Text;
						dataTable = new DataTable();
						npgsqlDataAdapter2.Fill(dataTable);
					}
				}
				npgsqlTransaction.Commit();
			}
		}
		return dataTable;
	}

	public DataTable ExecuteDataTable(string query, string structTable, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		NpgsqlParameter[] parameters2 = ParseParamater(parameters);
		DataTable dataTable = new DataTable();
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (NpgsqlCommand npgsqlCommand = DbConnection.CreateCommand())
			{
				npgsqlCommand.CommandTimeout = 60000;
				npgsqlCommand.CommandText = query;
				npgsqlCommand.CommandType = commandType;
				npgsqlCommand.Transaction = DbTransaction;
				npgsqlCommand.TryAddParameters(parameters2);
				NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(npgsqlCommand);
				npgsqlDataAdapter.Fill(dataTable);
				npgsqlCommand.Parameters.Clear();
				if (commandType == CommandType.Text || dataTable.Columns.Count > 1)
				{
					return dataTable;
				}
				npgsqlCommand.CommandType = CommandType.Text;
				npgsqlCommand.CommandText = "select lower(data_type) from information_schema.routines where routine_type = 'FUNCTION' and routine_name = @function";
				npgsqlCommand.TryAddParameters(new NpgsqlParameter("@function", query));
				if (ToString(npgsqlCommand.ExecuteScalar()) == "refcursor")
				{
					if (dataTable.Rows.Count == 0)
					{
						return new DataTable();
					}
					npgsqlCommand.CommandText = "FETCH ALL IN \"" + ToString(dataTable.Rows[0][0]) + "\";";
					npgsqlCommand.CommandType = CommandType.Text;
					npgsqlCommand.Parameters.Clear();
					dataTable = new DataTable();
					npgsqlDataAdapter.Fill(dataTable);
				}
				if (!string.IsNullOrEmpty(structTable))
				{
					SetDefaultValueToTable(structTable, dataTable);
				}
				return dataTable;
			}
		}
		using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(ConnectionString))
		{
			if (npgsqlConnection.State == ConnectionState.Closed)
			{
				npgsqlConnection.Open();
			}
			SetConnectionSchema(npgsqlConnection);
			using NpgsqlCommand npgsqlCommand2 = npgsqlConnection.CreateCommand();
			npgsqlCommand2.CommandTimeout = 60000;
			npgsqlCommand2.CommandText = query;
			npgsqlCommand2.CommandType = commandType;
			npgsqlCommand2.TryAddParameters(parameters2);
			NpgsqlDataAdapter npgsqlDataAdapter2 = new NpgsqlDataAdapter(npgsqlCommand2);
			if (commandType == CommandType.Text)
			{
				npgsqlDataAdapter2.Fill(dataTable);
			}
			else
			{
				NpgsqlTransaction npgsqlTransaction = (npgsqlCommand2.Transaction = npgsqlConnection.BeginTransaction());
				npgsqlDataAdapter2.Fill(dataTable);
				npgsqlCommand2.Parameters.Clear();
				if (dataTable.Columns.Count == 1)
				{
					npgsqlCommand2.CommandType = CommandType.Text;
					npgsqlCommand2.CommandText = "select lower(data_type) from information_schema.routines where routine_type = 'FUNCTION' and routine_name = @function;";
					npgsqlCommand2.TryAddParameters(new NpgsqlParameter("@function", query));
					if (ToString(npgsqlCommand2.ExecuteScalar()) == "refcursor")
					{
						if (dataTable.Rows.Count == 0)
						{
							return new DataTable();
						}
						npgsqlCommand2.CommandText = "FETCH ALL IN \"" + ToString(dataTable.Rows[0][0]) + "\";";
						npgsqlCommand2.Parameters.Clear();
						npgsqlCommand2.CommandType = CommandType.Text;
						dataTable = new DataTable();
						npgsqlDataAdapter2.Fill(dataTable);
					}
				}
				npgsqlTransaction.Commit();
			}
		}
		if (!string.IsNullOrEmpty(structTable))
		{
			SetDefaultValueToTable(structTable, dataTable);
		}
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
				SetConnectionSchema(DbConnection);
			}
		}
		ExecuteNonQuery(text, CommandType.Text);
	}

	private string GetSqlType(DataColumn col, int columnLength)
	{
		NpgsqlDbType dBType = GetDBType(col.DataType);
		if (col.DataType == typeof(string))
		{
			if (columnLength >= 5001)
			{
				return "text";
			}
			return "character varying(5000)";
		}
		if (col.DataType == typeof(char))
		{
			return "CHAR(1)";
		}
		switch (dBType)
		{
		case NpgsqlDbType.Double:
		case NpgsqlDbType.Numeric:
			return "numeric(19,6)";
		default:
			if (dBType != NpgsqlDbType.TimestampTZ)
			{
				return dBType.ToString();
			}
			goto case NpgsqlDbType.Date;
		case NpgsqlDbType.Date:
		case NpgsqlDbType.Timestamp:
		case NpgsqlDbType.TimestampTZ:
			return "date";
		}
	}

	private NpgsqlDbType GetDBType(Type theType)
	{
		NpgsqlParameter npgsqlParameter = new NpgsqlParameter();
		TypeConverter converter = TypeDescriptor.GetConverter(npgsqlParameter.DbType);
		if (converter.CanConvertFrom(theType))
		{
			object obj = converter.ConvertFrom(theType.Name);
			if (obj != null)
			{
				npgsqlParameter.DbType = (DbType)obj;
			}
		}
		else
		{
			try
			{
				object obj2 = converter.ConvertFrom(theType.Name);
				if (obj2 != null)
				{
					npgsqlParameter.DbType = (DbType)obj2;
				}
			}
			catch (Exception)
			{
				if (theType.Name.Equals("TimeSpan"))
				{
					npgsqlParameter.DbType = DbType.Time;
				}
			}
		}
		return npgsqlParameter.NpgsqlDbType;
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
				SetConnectionSchema(DbConnection);
			}
			using BulkOperation bulkOperation = new BulkOperation(DbConnection);
			bulkOperation.DestinationTableName = tblName;
			bulkOperation.BatchSize = 1000;
			bulkOperation.BatchTimeout = 60000;
			bulkOperation.BulkInsert(dt);
			return;
		}
		using NpgsqlConnection npgsqlConnection = new NpgsqlConnection(ConnectionString);
		if (npgsqlConnection.State == ConnectionState.Closed)
		{
			npgsqlConnection.Open();
		}
		SetConnectionSchema(npgsqlConnection);
		using BulkOperation bulkOperation2 = new BulkOperation(npgsqlConnection);
		bulkOperation2.DestinationTableName = tblName;
		bulkOperation2.BatchSize = 1000;
		bulkOperation2.BatchTimeout = 60000;
		bulkOperation2.BulkInsert(dt);
	}

	public virtual void SetDefaultValueToTable(string table, DataTable dt)
	{
		if (dt == null || dt.Columns.Count == 0)
		{
			return;
		}
		DataTable dataTable = ExecuteDataTable("SELECT column_name \"COLUMN_NAME\",column_default \"COLUMN_DEFAULT\",is_nullable \"IS_NULLABLE\",data_type \"DATA_TYPE FROM\" information_schema.columns\r\n                WHERE table_catalog = current_database() and table_schema = current_schema() and table_name= @tblName and (is_nullable = 'NO' or column_default is not null);", CommandType.Text, new SqlParameter("@tblName", table));
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
					if (dataTable.Rows[i]["COLUMN_DEFAULT"] != null && dataTable.Rows[i]["COLUMN_DEFAULT"].ToString() != "")
					{
						string text2 = dataTable.Rows[i]["COLUMN_DEFAULT"].ToString();
						int num = text2.LastIndexOf("::");
						text2 = ((num > 0) ? text2.Substring(0, num) : text2);
						dt.Columns[j].DefaultValue = text2;
					}
					break;
				}
			}
		}
	}

	public virtual DataTable GetTableStruct(string tblName)
	{
		DataTable dataTable = ExecuteDataTable("SELECT column_name \"COLUMN_NAME\",column_default \"COLUMN_DEFAULT\",is_nullable \"IS_NULLABLE\",data_type \"DATA_TYPE\" FROM information_schema.columns \r\n\r\n    where table_catalog = current_database() and table_schema = current_schema() and table_name = @tblName order by ordinal_position;", CommandType.Text, new SqlParameter("@tblName", tblName));
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
				case "timestamp without time zone":
				case "timestamp with time zone":
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
				if (dataTable.Rows[i]["COLUMN_DEFAULT"] != null && dataTable.Rows[i]["COLUMN_DEFAULT"].ToString() != "")
				{
					string text2 = dataTable.Rows[i]["COLUMN_DEFAULT"].ToString();
					int num = text2.LastIndexOf("::");
					text2 = ((num > 0) ? text2.Substring(1, num - 2) : text2);
					dataColumn.DefaultValue = text2;
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
		DbConnection = ((DbConnection == null || DbConnection.State == ConnectionState.Closed) ? new NpgsqlConnection(ConnectionString) : DbConnection);
		if (DbConnection.State == ConnectionState.Closed)
		{
			DbConnection.Open();
		}
		SetConnectionSchema(DbConnection);
		return DbConnection;
	}

	public DbTransaction BeginTransaction(IsolationLevel level = IsolationLevel.ReadCommitted)
	{
		DbTransaction = DbConnection.BeginTransaction(level);
		return DbTransaction;
	}

	private NpgsqlParameter[] ParseParamater(SqlParameter[] arrSqlPara)
	{
		if (arrSqlPara == null || arrSqlPara.Length == 0)
		{
			return null;
		}
		int num = arrSqlPara.Length;
		NpgsqlParameter[] array = new NpgsqlParameter[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = new NpgsqlParameter(arrSqlPara[i].ParameterName, arrSqlPara[i].Value ?? DBNull.Value);
		}
		return array;
	}

	private void SetConnectionSchema(NpgsqlConnection connection)
	{
		if (!string.IsNullOrEmpty(SchemaName))
		{
			using (NpgsqlCommand npgsqlCommand = connection.CreateCommand())
			{
				npgsqlCommand.CommandTimeout = 60000;
				npgsqlCommand.CommandText = $"set schema '{SchemaName}'";
				npgsqlCommand.CommandType = CommandType.Text;
				npgsqlCommand.ExecuteNonQuery();
			}
		}
	}

	private string ToString(object obj)
	{
		try
		{
			if (obj == null)
			{
				return string.Empty;
			}
			return obj.ToString().Trim();
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}
}
