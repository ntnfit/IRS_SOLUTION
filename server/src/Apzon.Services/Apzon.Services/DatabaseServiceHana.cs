using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.HDBConnection;
using Apzon.HDBConnection.Interfaces;
using Sap.Data.Hana;

namespace Apzon.Services;

public class DatabaseServiceHana : DatabaseHanaClient, IDatabaseService, IDatabaseClient
{
	private static readonly Regex NullException = new Regex("column\\s+'([^']+)'.*?table\\s+'[^:]+:([^']+)'", RegexOptions.Compiled);

	private static readonly Regex ToLargeException = new Regex("max\\s+length\\s+\\((\\d+)\\).*?column\\s+'([^']+)'.*?table\\s+'[^:]+:([^']+)'", RegexOptions.Compiled);

	private static readonly Regex UniqueConstraintException = new Regex("Table\\(([^)$]+)", RegexOptions.Compiled);

	private const string StartError = "user-defined error exception:";

	private static readonly int StartErrorLength = "user-defined error exception:".Length;

	private static readonly Regex InvalidNumberException = new Regex("attribute value is not a number;column\\s+'([^']+)'.*?table\\s+'[^:]+:([^']+)'", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	public DatabaseServiceHana(string connectionString)
		: base(connectionString)
	{
	}

	public DatabaseServiceHana(HanaConnection connection)
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

	public string GetObjectId(ApzObjectTypes objtype)
	{
		DataTable dataTable = ExecuteDataTable("SELECT \"AutoKey\" FROM ONNM WHERE \"ObjectCode\"=ObjType;", CommandType.Text, new SqlParameter("@ObjType", Function.ToString((int)objtype)));
		if (dataTable.IsNotNull())
		{
			return Function.ToString(dataTable.Rows[0][0]);
		}
		return string.Empty;
	}

	public string GetObjectId(string udocode)
	{
		DataTable dataTable = ExecuteDataTable("SELECT \"AutoKey\" FROM dbo.ONNM WHERE \"ObjectCode\"=ObjType;", CommandType.Text, new SqlParameter("@ObjType", udocode));
		if (dataTable.IsNotNull())
		{
			return Function.ToString(dataTable.Rows[0][0]);
		}
		return string.Empty;
	}

	public void SetNextAutoKey(ApzObjectTypes objtype)
	{
		ExecuteNonQuery("UPDATE ONNM Set \"AutoKey\"=\"AutoKey\"+1 WHERE \"ObjectCode\"=ObjType;", CommandType.Text, new SqlParameter("@ObjType", Function.ToString((int)objtype)));
	}

	public string GetObjectNum(ApzObjectTypes objtype, int bplId, int series, int usersign)
	{
		if (!ExecuteDataTable("SELECT \"Series\",\"SeriesName\",IFNULL(\"BPLId\",-1) \"BPLId\" FROM NNM1 WHERE \"ObjectCode\"=ObjectCode AND \r\n                                  (\"BPLId\" IN (SELECT \"BPLId\" FROM USR6 A INNER JOIN OUSR B ON A.\"UserCode\"=b.\"USER_CODE\" WHERE b.\"USERID\"=USERID) OR IFNULL(\"BPLId\",-1)=-1) AND \"Series\"=Series;", CommandType.Text, new SqlParameter("@ObjectCode", Function.ToString((int)objtype)), new SqlParameter("@USERID", usersign), new SqlParameter("@Series", series)).IsNotNull())
		{
			throw new Exception($"Numbering series assigned to another branch");
		}
		DataTable dataTable = ExecuteDataTable("SELECT \"NextNumber\" FROM NNM1 WHERE \"ObjectCode\"=ObjType AND IFNULL(\"Series\",-1)=Series and \"IsManual\" = 'N';", CommandType.Text, new SqlParameter("@ObjType", Function.ToString((int)objtype)), new SqlParameter("@Series", series));
		if (!dataTable.IsNotNull())
		{
			return string.Empty;
		}
		return Function.ToString(dataTable.Rows[0][0]);
	}

	public string GetObjectNumBySeries(ApzObjectTypes objtype, int series, int usersign = -1)
	{
		DataTable dataTable = ExecuteDataTable("SELECT \"NextNumber\" FROM NNM1 WHERE \"ObjectCode\"=ObjType AND IFNULL(\"Series\",-1)=Series and \"IsManual\" = 'N';", CommandType.Text, new SqlParameter("@ObjType", Function.ToString((int)objtype)), new SqlParameter("@Series", series));
		if (!dataTable.IsNotNull())
		{
			return string.Empty;
		}
		return Function.ToString(dataTable.Rows[0][0]);
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

	public void LockNumbering(bool lockNumbering, ApzObjectTypes objtype)
	{
		if (lockNumbering)
		{
			ExecuteNonQuery("update ONNM set \"AutoKey\" = \"AutoKey\"  where \"ObjectCode\" = ObjType;", CommandType.Text, new SqlParameter("@ObjType", Function.ToString((int)objtype)));
		}
	}

	private string ParseParamater(HanaParameter[] arrHanaPara, SqlParameter[] arrSqlPara, ref string hanaQuery)
	{
		if (arrSqlPara == null || arrSqlPara.Length == 0)
		{
			return "";
		}
		string text = "(";
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
				text2 += ((Function.ToString(arrHanaPara[i].Value).Length < 4000) ? "(4000)" : $"({Function.ToString(arrHanaPara[i].Value).Length + 10})");
			}
			text += $"in {arrHanaPara[i].ParameterName} {text2} => ?,";
		}
		return $"{text.Substring(0, text.Length - 1)})";
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
				string text3 = array2[i];
				text = text + " AND \"" + text2 + "\"=@objId" + i;
				list.Add(new SqlParameter("@objId" + i, string.IsNullOrEmpty(text3) ? null : text3));
				list2.Add(new SqlParameter("@objId" + i, string.IsNullOrEmpty(text3) ? null : text3));
			}
		}
		string query2;
		if (transtype != Enums.TransactionType.D)
		{
			string text4 = $"\"#temp_history{line.TableName}{Function.ToString(paramanter)}\"";
			string query = $" create local temporary table {text4} as (SELECT top 0 * from {line.TableName});\r\n                    insert into {text4} select * from {line.TableName} where 1=1{text};\r\n\t\t\t\t\t\t\t\t\t\t\t\t\tUPDATE {text4} set \"{line.LogInstanceKey}\" = {num}; \r\n\t\t\t\t\t\t\t\t\t\t\t\t\tINSERT into {line.HistoryTableName} SELECT * from {text4};";
			ExecuteNonQuery(query, CommandType.Text, list.ToArray());
			query2 = $"DELETE from {line.HistoryTableName} where 1=1 {text} AND \"{line.LogInstanceKey}\"  not in (Select top {maxHistory} \"{line.LogInstanceKey}\" from (select distinct \"{line.LogInstanceKey}\" from {line.HistoryTableName} where 1=1 {text}) as a  ORDER BY \"{line.LogInstanceKey}\" DESC);";
		}
		else
		{
			query2 = "DELETE from " + line.HistoryTableName + " where 1=1 " + text + ";";
		}
		ExecuteNonQuery(query2, CommandType.Text, list2.ToArray());
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
				string text3 = array[i];
				text = text + " AND \"" + text2 + "\"=@objId" + i;
				list.Add(new SqlParameter("@objId" + i, string.IsNullOrEmpty(text3) ? null : text3));
			}
		}
		string query = " select ifnull( max(\"" + logInstance.LogInstanceKey + "\"),-1)+1 from " + logInstance.TableLogInstanceKey + " where 1=1" + text + ";";
		return Function.ParseInt(ExecuteDataTable(query, CommandType.Text, list.ToArray()).Rows[0][0]);
	}

	public string ConvertSqlErrorMessage(string error)
	{
		if (error.IndexOf("cannot insert NULL or update to NULL", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			Match match = NullException.Match(error);
			if (match.Success)
			{
				string value = match.Groups[1].Value;
				string value2 = match.Groups[2].Value;
				return "[" + value2 + "].[" + value + "] cannot be empty.";
			}
		}
		else if (error.IndexOf("inserted value too large for column", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			Match match2 = ToLargeException.Match(error);
			if (match2.Success)
			{
				string value3 = match2.Groups[1].Value;
				string value4 = match2.Groups[2].Value;
				string value5 = match2.Groups[3].Value;
				return "[" + value5 + "].[" + value4 + "]: Character number is greater than allowed (" + value3 + ").";
			}
		}
		else if (error.IndexOf("unique constraint violated", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			Match match3 = UniqueConstraintException.Match(error);
			if (match3.Success)
			{
				string value6 = match3.Groups[1].Value;
				return "[" + value6 + "]: Unique constraint violated.";
			}
		}
		else if (error.StartsWith("user-defined error:", StringComparison.OrdinalIgnoreCase))
		{
			int num = error.LastIndexOf("user-defined error exception:", StringComparison.OrdinalIgnoreCase);
			if (num >= 0)
			{
				return error.Substring(num + StartErrorLength).Trim();
			}
		}
		else if (error.IndexOf("attribute value is not a number", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			Match match4 = InvalidNumberException.Match(error);
			if (match4.Success)
			{
				string value7 = match4.Groups[1].Value;
				string value8 = match4.Groups[2].Value;
				return "[" + value8 + "].[" + value7 + "]: Value must be a valid number.";
			}
		}
		return string.Empty;
	}
}
