using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Apzon.HDBConnection.Interfaces;

namespace Apzon.HDbConnection;

public class BaseServiceClient
{
	protected IDatabaseClient DatabaseServiceClient { get; set; }

	public BaseServiceClient(IDatabaseClient databaseServiceClient)
	{
		if (databaseServiceClient == null)
		{
			throw new ArgumentNullException("databaseServiceClient");
		}
		DatabaseServiceClient = databaseServiceClient;
	}

	public string InsertDataTableIntoSql(DataTable dt)
	{
		return DatabaseServiceClient.InsertDataTableIntoSql(dt);
	}

	public int ExecuteNonQueryNotParse(string query, CommandType commandType = CommandType.Text)
	{
		return DatabaseServiceClient.ExecuteNonQueryNotParse(query, commandType);
	}

	public int ExecuteNonQuery(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		return DatabaseServiceClient.ExecuteNonQuery(query, commandType, parameters);
	}

	public int ExecuteNonQueryNoParam(string query, CommandType commandType = CommandType.Text)
	{
		return DatabaseServiceClient.ExecuteNonQueryNoParam(query, commandType);
	}

	public object ExecuteScalar(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		return DatabaseServiceClient.ExecuteScalar(query, commandType, parameters);
	}

	public DataSet ExecuteData(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		return DatabaseServiceClient.ExecuteData(query, commandType, parameters);
	}

	public DataTable ExecuteDataTable(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		return DatabaseServiceClient.ExecuteDataTable(query, commandType, parameters);
	}

	public DataTable ExecuteDataTable(string query, string structTable, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		return DatabaseServiceClient.ExecuteDataTable(query, structTable, commandType, parameters);
	}

	public DataTable BulkToTempTable(DataTable source, string tableName, string temptableName)
	{
		DataTable tableStruct = GetTableStruct(tableName);
		if (tableStruct == null || tableStruct.Rows.Count == 0)
		{
			throw new Exception("Could not get data structure");
		}
		IEnumerable<string> source2 = from DataColumn t in tableStruct.Columns
			select t.ColumnName;
		source = ((source != null && source.Rows.Count > 0) ? source.DefaultView.ToTable(distinct: false, source2.ToArray()) : tableStruct.Clone());
		CreateTempTable(source, temptableName);
		BulkCopy(source, temptableName);
		return source;
	}

	public void CreateTempTable(DataTable dt, string tblName)
	{
		DatabaseServiceClient.CreateTempTable(dt, tblName);
	}

	public void BulkCopy(DataTable dt, string tblName)
	{
		DatabaseServiceClient.BulkCopy(dt, tblName);
	}

	public DataTable GetTableStruct(string tblName)
	{
		return DatabaseServiceClient.GetTableStruct(tblName);
	}

	public void SetDefaultValueToTable(string table, DataTable dt)
	{
		DatabaseServiceClient.SetDefaultValueToTable(table, dt);
	}

	public object GetDefaultSqlValue(string dbtype)
	{
		switch (dbtype)
		{
		case "int":
		case "bigint":
		case "smallint":
		case "decimal":
		case "numeric":
		case "float":
			return 0;
		case "datetime":
		case "date":
			return new DateTime(1753, 1, 1);
		case "bit":
			return false;
		default:
			return string.Empty;
		}
	}
}
