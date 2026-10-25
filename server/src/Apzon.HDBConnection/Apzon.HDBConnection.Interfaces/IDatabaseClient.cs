using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Apzon.HDBConnection.Interfaces;

public interface IDatabaseClient
{
	string ConnectionString { get; set; }

	string SchemaName { get; set; }

	string InsertDataTableIntoSql(DataTable dt);

	int ExecuteNonQueryNotParse(string query, CommandType commandType = CommandType.Text);

	int ExecuteNonQuery(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters);

	int ExecuteNonQueryNoParam(string query, CommandType commandType = CommandType.Text);

	object ExecuteScalar(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters);

	DataSet ExecuteData(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters);

	DataTable ExecuteDataTable(string query, string structTable, CommandType commandType = CommandType.Text, params SqlParameter[] parameters);

	DataTable ExecuteDataTable(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters);

	void CreateTempTable(DataTable dt, string tblName);

	DataTable GetTableStruct(string tblName);

	void SetDefaultValueToTable(string table, DataTable dt);

	void BulkCopy(DataTable dt, string tblName);

	DbConnection OpenConnection();

	DbTransaction BeginTransaction(IsolationLevel level = IsolationLevel.ReadCommitted);
}
