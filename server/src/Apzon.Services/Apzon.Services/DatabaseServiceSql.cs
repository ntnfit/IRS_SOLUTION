using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.HDBConnection;
using Apzon.HDBConnection.Interfaces;

namespace Apzon.Services;

public class DatabaseServiceSql : DatabaseSqlClient, IDatabaseService, IDatabaseClient
{
	public DatabaseServiceSql(string connectionString)
		: base(connectionString)
	{
	}

	public DatabaseServiceSql(SqlConnection connection)
		: base(connection)
	{
	}

	public override string InsertDataTableIntoSql(DataTable dt)
	{
		try
		{
			return base.InsertDataTableIntoSql(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override DataTable GetTableStruct(string tblName)
	{
		try
		{
			return base.GetTableStruct(tblName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public override void SetDefaultValueToTable(string table, DataTable dt)
	{
		try
		{
			base.SetDefaultValueToTable(table, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public string GetObjectId(ApzObjectTypes objtype)
	{
		DataTable dataTable = ExecuteDataTable("SELECT AutoKey FROM dbo.ONNM WHERE ObjectCode=@ObjType", CommandType.Text, new SqlParameter("@ObjType", Function.ToString((int)objtype)));
		if (dataTable.IsNotNull())
		{
			return Function.ToString(dataTable.Rows[0][0]);
		}
		return string.Empty;
	}

	public string GetObjectId(string udocode)
	{
		DataTable dataTable = ExecuteDataTable("SELECT AutoKey FROM dbo.ONNM WHERE ObjectCode=@ObjType", CommandType.Text, new SqlParameter("@ObjType", udocode));
		if (dataTable.IsNotNull())
		{
			return Function.ToString(dataTable.Rows[0][0]);
		}
		return string.Empty;
	}

	public void SetNextAutoKey(ApzObjectTypes objtype)
	{
		ExecuteNonQuery("UPDATE ONNM Set AutoKey=AutoKey+1 WHERE ObjectCode=@ObjType", CommandType.Text, new SqlParameter("@ObjType", Function.ToString((int)objtype)));
	}

	public string GetObjectNum(ApzObjectTypes objtype, int bplId, int series, int usersign)
	{
		if (!ExecuteDataTable("SELECT Series,SeriesName,ISNULL(BPLId,-1) BPLId FROM NNM1 WHERE ObjectCode=@ObjectCode AND \r\n                                  (BPLId IN (SELECT BPLId FROM USR6 A INNER JOIN OUSR B ON A.UserCode=b.USER_CODE WHERE b.USERID=@USERID) OR ISNULL(BPLId,-1)=-1) AND Series=@Series", CommandType.Text, new SqlParameter("@ObjectCode", Function.ToString((int)objtype)), new SqlParameter("@USERID", usersign), new SqlParameter("@Series", series)).IsNotNull())
		{
			throw new Exception($"Numbering series assigned to another branch");
		}
		DataTable dataTable = ExecuteDataTable("SELECT NextNumber FROM NNM1 WHERE ObjectCode=@ObjType AND ISNULL(Series,-1)=@Series and IsManual = 'N'", CommandType.Text, new SqlParameter("@ObjType", Function.ToString((int)objtype)), new SqlParameter("@Series", series));
		if (!dataTable.IsNotNull())
		{
			return string.Empty;
		}
		return Function.ToString(dataTable.Rows[0][0]);
	}

	public string GetObjectNumBySeries(ApzObjectTypes objtype, int series, int usersign = -1)
	{
		DataTable dataTable = ExecuteDataTable("SELECT NextNumber FROM NNM1 WHERE ObjectCode=@ObjType AND ISNULL(Series,-1)=@Series and IsManual = 'N'", CommandType.Text, new SqlParameter("@ObjType", Function.ToString((int)objtype)), new SqlParameter("@Series", series));
		if (!dataTable.IsNotNull())
		{
			return string.Empty;
		}
		return Function.ToString(dataTable.Rows[0][0]);
	}

	public void LockNumbering(bool lockNumbering, ApzObjectTypes objtype)
	{
		if (lockNumbering)
		{
			ExecuteNonQuery("update a set autokey = a.autokey from (select *, ROW_NUMBER() over(order by ObjectCode) NewId from ONNM where ObjectCode = @ObjType) a where NewId = 1", CommandType.Text, new SqlParameter("@ObjType", Function.ToString((int)objtype)));
		}
	}

	public void ProcessChangeLog(Enums.TransactionType transtype, ApzObjectTypes objtype, LogInstance header, LogInstance line, int headerMaxInsKey, string paramanter, int maxHistory)
	{
		int num = ((line.TableLogInstanceKey == header.TableLogInstanceKey) ? headerMaxInsKey : MaxInstanceKey(paramanter, line));
		string text = "";
		List<SqlParameter> list = new List<SqlParameter>();
		List<SqlParameter> list2 = new List<SqlParameter>();
		string[] array = line.Key.Split(',');
		string[] array2 = paramanter.Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i];
			if (!string.IsNullOrWhiteSpace(text2))
			{
				string value = array2[i];
				text = text + " AND  " + text2 + "=@objId" + i;
				list.Add(new SqlParameter("@objId" + i, value));
				list2.Add(new SqlParameter("@objId" + i, value));
			}
		}
		string query2;
		if (transtype != Enums.TransactionType.D)
		{
			string text3 = $"#temp_history{line.TableName}";
			string query = $" SELECT * into {text3} from {line.TableName}  where 1=1 {text}; \r\n\t\t\t\t\t\t\t\t\t\t\t\t\tUPDATE {text3} set {line.LogInstanceKey} = {num}; \r\n\t\t\t\t\t\t\t\t\t\t\t\t\tINSERT into  {line.HistoryTableName} SELECT * from {text3};";
			if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) <= 0)
			{
				Logging.Write("ERROR", $"create log failed, no row affected. object {objtype} - id {paramanter} - table {line.TableName}");
			}
			query2 = $"DELETE from {line.HistoryTableName} where 1=1 {text} AND {line.LogInstanceKey}  not in (Select top {maxHistory} {line.LogInstanceKey} from (select distinct {line.LogInstanceKey} from {line.HistoryTableName} where 1=1 {text}) as a  ORDER BY {line.LogInstanceKey} DESC);";
		}
		else
		{
			query2 = "DELETE from " + line.HistoryTableName + " where 1=1 " + text;
		}
		int num2 = ExecuteNonQuery(query2, CommandType.Text, list2.ToArray());
		if (num2 > 0)
		{
			Logging.Write("TRACE", $"delete log , {num2} rows affected. object {objtype} - id {paramanter} - table {line.TableName}");
		}
	}

	public int MaxInstanceKey(string val, LogInstance logInstance)
	{
		List<SqlParameter> list = new List<SqlParameter>();
		string text = "";
		string[] array = val.Split(',');
		string[] array2 = logInstance.Key.Split(',');
		for (int i = 0; i < array2.Length; i++)
		{
			string text2 = array2[i];
			if (!string.IsNullOrWhiteSpace(text2))
			{
				string value = array[i];
				text = text + " AND " + text2 + "=@objId" + i;
				list.Add(new SqlParameter("@objId" + i, value));
			}
		}
		string query = " select isnull( max(" + logInstance.LogInstanceKey + "),-1)+1 from " + logInstance.TableLogInstanceKey + " where 1=1 " + text;
		return Function.ParseInt(ExecuteDataTable(query, CommandType.Text, list.ToArray()).Rows[0][0]);
	}

	public string ConvertSqlErrorMessage(string error)
	{
		return error;
	}
}
