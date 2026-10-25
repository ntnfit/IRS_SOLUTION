using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using Apzon.HDBConnection.Interfaces;

namespace Apzon.HDBConnection;

public class DatabaseSqlClient : IDatabaseClient
{
	public string ConnectionString { get; set; }

	public string SchemaName { get; set; }

	public SqlConnection DbConnection { get; set; }

	public SqlTransaction DbTransaction { get; set; }

	public DatabaseSqlClient(string connectionString)
	{
		if (connectionString == null)
		{
			throw new ArgumentNullException("connectionString");
		}
		ConnectionString = connectionString;
		DbTransaction = null;
	}

	public DatabaseSqlClient(SqlConnection connection)
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
		using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
		if (sqlConnection.State == ConnectionState.Closed)
		{
			sqlConnection.Open();
		}
		using SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(ConnectionString);
		sqlBulkCopy.DestinationTableName = dt.TableName;
		foreach (object column in dt.Columns)
		{
			sqlBulkCopy.ColumnMappings.Add(column.ToString(), column.ToString());
		}
		sqlBulkCopy.WriteToServer(dt);
		dt.Rows.Clear();
		return "";
	}

	public int ExecuteNonQueryNotParse(string query, CommandType commandType = CommandType.Text)
	{
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (SqlCommand sqlCommand = DbConnection.CreateCommand())
			{
				sqlCommand.CommandTimeout = 60000;
				sqlCommand.CommandText = query;
				sqlCommand.Transaction = DbTransaction;
				sqlCommand.CommandType = commandType;
				return sqlCommand.ExecuteNonQuery();
			}
		}
		using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
		if (sqlConnection.State == ConnectionState.Closed)
		{
			sqlConnection.Open();
		}
		SqlCommand sqlCommand2 = sqlConnection.CreateCommand();
		sqlCommand2.CommandTimeout = 60000;
		sqlCommand2.CommandText = query;
		sqlCommand2.CommandType = commandType;
		return sqlCommand2.ExecuteNonQuery();
	}

	public int ExecuteNonQuery(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (SqlCommand sqlCommand = DbConnection.CreateCommand())
			{
				sqlCommand.CommandTimeout = 60000;
				sqlCommand.CommandText = query;
				sqlCommand.Transaction = DbTransaction;
				sqlCommand.CommandType = commandType;
				sqlCommand.TryAddParameters(parameters);
				return sqlCommand.ExecuteNonQuery();
			}
		}
		using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
		if (sqlConnection.State == ConnectionState.Closed)
		{
			sqlConnection.Open();
		}
		SqlCommand sqlCommand2 = sqlConnection.CreateCommand();
		sqlCommand2.CommandTimeout = 60000;
		sqlCommand2.CommandText = query;
		sqlCommand2.CommandType = commandType;
		sqlCommand2.TryAddParameters(parameters);
		return sqlCommand2.ExecuteNonQuery();
	}

	public int ExecuteNonQueryNoParam(string query, CommandType commandType = CommandType.Text)
	{
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (SqlCommand sqlCommand = DbConnection.CreateCommand())
			{
				sqlCommand.CommandTimeout = 60000;
				sqlCommand.Transaction = DbTransaction;
				sqlCommand.CommandText = query;
				sqlCommand.CommandType = commandType;
				return sqlCommand.ExecuteNonQuery();
			}
		}
		using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
		if (sqlConnection.State == ConnectionState.Closed)
		{
			sqlConnection.Open();
		}
		SqlCommand sqlCommand2 = sqlConnection.CreateCommand();
		sqlCommand2.CommandTimeout = 60000;
		sqlCommand2.CommandText = query;
		sqlCommand2.CommandType = commandType;
		return sqlCommand2.ExecuteNonQuery();
	}

	public object ExecuteScalar(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (SqlCommand sqlCommand = DbConnection.CreateCommand())
			{
				sqlCommand.CommandTimeout = 60000;
				sqlCommand.CommandText = query;
				sqlCommand.Transaction = DbTransaction;
				sqlCommand.CommandType = commandType;
				sqlCommand.TryAddParameters(parameters);
				return sqlCommand.ExecuteScalar();
			}
		}
		using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
		if (sqlConnection.State == ConnectionState.Closed)
		{
			sqlConnection.Open();
		}
		SqlCommand sqlCommand2 = sqlConnection.CreateCommand();
		sqlCommand2.CommandTimeout = 60000;
		sqlCommand2.CommandText = query;
		sqlCommand2.CommandType = commandType;
		sqlCommand2.TryAddParameters(parameters);
		return sqlCommand2.ExecuteScalar();
	}

	public DataSet ExecuteData(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		DataSet dataSet = new DataSet();
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (SqlCommand sqlCommand = DbConnection.CreateCommand())
			{
				sqlCommand.CommandTimeout = 60000;
				sqlCommand.Transaction = DbTransaction;
				sqlCommand.CommandText = query;
				sqlCommand.CommandType = commandType;
				sqlCommand.TryAddParameters(parameters);
				new SqlDataAdapter(sqlCommand).Fill(dataSet);
				sqlCommand.Parameters.Clear();
				return dataSet;
			}
		}
		using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
		if (sqlConnection.State == ConnectionState.Closed)
		{
			sqlConnection.Open();
		}
		using SqlCommand sqlCommand2 = sqlConnection.CreateCommand();
		sqlCommand2.CommandTimeout = 60000;
		sqlCommand2.CommandText = query;
		sqlCommand2.CommandType = commandType;
		sqlCommand2.TryAddParameters(parameters);
		new SqlDataAdapter(sqlCommand2).Fill(dataSet);
		sqlCommand2.Parameters.Clear();
		return dataSet;
	}

	public DataTable ExecuteDataTable(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		DataTable dataTable = new DataTable();
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (SqlCommand sqlCommand = DbConnection.CreateCommand())
			{
				sqlCommand.CommandTimeout = 60000;
				sqlCommand.CommandText = query;
				sqlCommand.Transaction = DbTransaction;
				sqlCommand.CommandType = commandType;
				sqlCommand.TryAddParameters(parameters);
				new SqlDataAdapter(sqlCommand).Fill(dataTable);
				sqlCommand.Parameters.Clear();
				return dataTable;
			}
		}
		using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
		if (sqlConnection.State == ConnectionState.Closed)
		{
			sqlConnection.Open();
		}
		using SqlCommand sqlCommand2 = sqlConnection.CreateCommand();
		sqlCommand2.CommandTimeout = 60000;
		sqlCommand2.CommandText = query;
		sqlCommand2.CommandType = commandType;
		sqlCommand2.TryAddParameters(parameters);
		new SqlDataAdapter(sqlCommand2).Fill(dataTable);
		sqlCommand2.Parameters.Clear();
		return dataTable;
	}

	public DataTable ExecuteDataTable(string query, string structTable, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		DataTable dataTable = new DataTable();
		if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
		{
			using (SqlCommand sqlCommand = DbConnection.CreateCommand())
			{
				sqlCommand.CommandTimeout = 60000;
				sqlCommand.Transaction = DbTransaction;
				sqlCommand.CommandText = query;
				sqlCommand.CommandType = commandType;
				sqlCommand.TryAddParameters(parameters);
				new SqlDataAdapter(sqlCommand).Fill(dataTable);
				sqlCommand.Parameters.Clear();
				if (!string.IsNullOrEmpty(structTable))
				{
					SetDefaultValueToTable(structTable, dataTable);
				}
				return dataTable;
			}
		}
		using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
		if (sqlConnection.State == ConnectionState.Closed)
		{
			sqlConnection.Open();
		}
		using SqlCommand sqlCommand2 = sqlConnection.CreateCommand();
		sqlCommand2.CommandTimeout = 60000;
		sqlCommand2.CommandText = query;
		sqlCommand2.CommandType = commandType;
		sqlCommand2.TryAddParameters(parameters);
		new SqlDataAdapter(sqlCommand2).Fill(dataTable);
		if (!string.IsNullOrEmpty(structTable))
		{
			SetDefaultValueToTable(structTable, dataTable);
		}
		sqlCommand2.Parameters.Clear();
		return dataTable;
	}

	public virtual DataTable GetTableStruct(string tblName)
	{
		SqlParameter sqlParameter = new SqlParameter("@TblName", tblName);
		DataTable dataTable = ExecuteDataTable("SELECT COLUMN_NAME,COLUMN_DEFAULT,IS_NULLABLE,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@TblName", CommandType.Text, sqlParameter);
		DataTable dataTable2 = new DataTable();
		dataTable2.TableName = tblName;
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				string text = dataTable.Rows[i]["DATA_TYPE"].To_String().ToLower();
				string columnName = dataTable.Rows[i]["COLUMN_NAME"].To_String();
				DataColumn dataColumn = new DataColumn();
				dataColumn.ColumnName = columnName;
				string text2 = "";
				int length = dataTable.Rows[i]["COLUMN_DEFAULT"].To_String().Length;
				if (length >= 4)
				{
					text2 = dataTable.Rows[i]["COLUMN_DEFAULT"].To_String().Substring(2, length - 4);
				}
				bool flag = dataTable.Rows[i]["IS_NULLABLE"].To_String().Equals("YES");
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				Type typeFromHandle;
				switch (text)
				{
				case "int":
				case "bigint":
				case "smallint":
					typeFromHandle = typeof(int);
					if (!string.IsNullOrEmpty(text2) || !flag)
					{
						dataColumn.DefaultValue = text2.Replace("(", "").Replace(")", "").ParseInt();
					}
					break;
				case "decimal":
					typeFromHandle = typeof(decimal);
					if (!string.IsNullOrEmpty(text2) || !flag)
					{
						dataColumn.DefaultValue = text2.Replace("(", "").Replace(")", "").ParseDecimal();
					}
					break;
				case "datetime":
				case "date":
					typeFromHandle = typeof(DateTime);
					if (!string.IsNullOrEmpty(text2) || !flag)
					{
						dataColumn.DefaultValue = text2.ParseDateTime() ?? new DateTime(1900, 1, 1);
					}
					break;
				case "numeric":
				case "float":
					typeFromHandle = typeof(double);
					if (!string.IsNullOrEmpty(text2) || !flag)
					{
						dataColumn.DefaultValue = text2.Replace("(", "").Replace(")", "").ParseDouble(invariantCulture.NumberFormat);
					}
					break;
				case "bit":
					typeFromHandle = typeof(bool);
					if (!string.IsNullOrEmpty(text2) || !flag)
					{
						dataColumn.DefaultValue = text2.ParseBool();
					}
					break;
				default:
					typeFromHandle = typeof(string);
					if (!string.IsNullOrEmpty(text2) || !flag)
					{
						dataColumn.DefaultValue = text2.Replace("''", "'").To_String();
					}
					break;
				}
				dataColumn.AllowDBNull = flag;
				dataColumn.DataType = typeFromHandle;
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

	public virtual void SetDefaultValueToTable(string table, DataTable dt)
	{
		if (dt == null || dt.Columns.Count == 0)
		{
			return;
		}
		DataTable dataTable = ExecuteDataTable("SELECT COLUMN_NAME,COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@TblName and COLUMN_DEFAULT is not null", CommandType.Text, new SqlParameter("@TblName", table));
		if (dataTable == null || dataTable.Rows.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < dataTable.Rows.Count; i++)
		{
			string text = dataTable.Rows[i]["COLUMN_NAME"].To_String();
			string text2 = "";
			int length = dataTable.Rows[i]["COLUMN_DEFAULT"].To_String().Length;
			if (length >= 4)
			{
				text2 = dataTable.Rows[i]["COLUMN_DEFAULT"].To_String().Substring(2, length - 4);
			}
			for (int j = 0; j < dt.Columns.Count; j++)
			{
				if (dt.Columns[j].ColumnName == text)
				{
					dt.Columns[j].DefaultValue = text2.Replace("(", "").Replace(")", "").Replace("''", "'");
					break;
				}
			}
		}
	}

	private string GetDefaultString()
	{
		try
		{
			string initialCatalog = new SqlConnectionStringBuilder(ConnectionString).InitialCatalog;
			string query = $"SELECT DATABASEPROPERTYEX('{initialCatalog}', 'Collation') as Coll";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable.Rows[0]["Coll"].To_String();
			}
		}
		catch (Exception)
		{
		}
		return "";
	}

	private string GetSqlType(DataColumn col, string def)
	{
		SqlDbType dBType = GetDBType(col.DataType);
		if (col.DataType == typeof(string))
		{
			if (!string.IsNullOrEmpty(def))
			{
				return dBType.ToString() + "(MAX) COLLATE " + def;
			}
			return dBType.ToString() + "(MAX) COLLATE DATABASE_DEFAULT ";
		}
		if (dBType == SqlDbType.Decimal)
		{
			return dBType.ToString() + " (19,6)";
		}
		return dBType.ToString();
	}

	private SqlDbType GetDBType(Type theType)
	{
		SqlParameter sqlParameter = new SqlParameter();
		TypeConverter converter = TypeDescriptor.GetConverter(sqlParameter.DbType);
		if (converter.CanConvertFrom(theType))
		{
			object obj = converter.ConvertFrom(theType.Name);
			if (obj != null)
			{
				sqlParameter.DbType = (DbType)obj;
			}
		}
		else
		{
			try
			{
				object obj2 = converter.ConvertFrom(theType.Name);
				if (obj2 != null)
				{
					sqlParameter.DbType = (DbType)obj2;
				}
			}
			catch (Exception)
			{
			}
		}
		return sqlParameter.SqlDbType;
	}

	public void CreateTempTable(DataTable dt, string tblName)
	{
		string defaultString = GetDefaultString();
		string text = $"BEGIN TRY\r\n                                DROP TABLE [{tblName}]  \r\n                                END TRY\r\n                                BEGIN CATCH\r\n                                END CATCH";
		List<string> list = new List<string>();
		for (int i = 0; i < dt.Columns.Count; i++)
		{
			DataColumn dataColumn = dt.Columns[i];
			string item = string.Format("[{0}] {1} {2}", dataColumn.ColumnName, GetSqlType(dataColumn, defaultString), dataColumn.AllowDBNull ? "NULL" : "");
			list.Add(item);
		}
		text += string.Format(" CREATE TABLE [{0}]({1})", tblName, string.Join(",", list.ToArray()));
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
			using SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(DbConnection, SqlBulkCopyOptions.Default, DbTransaction);
			sqlBulkCopy.DestinationTableName = "[" + tblName + "]";
			sqlBulkCopy.BatchSize = 1000;
			sqlBulkCopy.BulkCopyTimeout = 60000;
			sqlBulkCopy.WriteToServer(dt);
			sqlBulkCopy.Close();
			return;
		}
		using SqlBulkCopy sqlBulkCopy2 = new SqlBulkCopy(ConnectionString);
		sqlBulkCopy2.DestinationTableName = "[" + tblName + "]";
		sqlBulkCopy2.BatchSize = 1000;
		sqlBulkCopy2.BulkCopyTimeout = 60000;
		sqlBulkCopy2.WriteToServer(dt);
		sqlBulkCopy2.Close();
	}

	public DbConnection OpenConnection()
	{
		DbConnection = ((DbConnection == null || string.IsNullOrEmpty(DbConnection.ConnectionString)) ? new SqlConnection(ConnectionString) : DbConnection);
		if (DbConnection.State == ConnectionState.Closed)
		{
			DbConnection.Open();
		}
		return DbConnection;
	}

	public DbTransaction BeginTransaction(IsolationLevel level = IsolationLevel.ReadCommitted)
	{
		DbTransaction = DbConnection.BeginTransaction(level);
		return DbTransaction;
	}
}
