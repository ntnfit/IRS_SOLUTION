using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Apzon.Commons;
using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaUDTRepository : UDTRepository
{
	public HanaUDTRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetStructureTableUdtQuery()
	{
		return "SELECT\r\n\t                 COLUMN_NAME,\r\n\t                 LOWER(CASE DATA_TYPE_NAME \r\n                         WHEN 'NCLOB' THEN 'NTEXT' \r\n\t                     WHEN 'INTEGER' THEN 'INT' \r\n\t                     WHEN 'TIMESTAMP' THEN 'DATETIME' \r\n\t                     WHEN 'DECIMAL' THEN 'NUMERIC' \r\n\t                     ELSE DATA_TYPE_NAME \r\n\t                     END) AS DATA_TYPE,\r\n\t                 CASE WHEN DATA_TYPE_NAME LIKE '%CHAR' OR DATA_TYPE_NAME LIKE '%TEXT' THEN '('''||DEFAULT_VALUE||''')'\r\n\t\t             ELSE '(('||DEFAULT_VALUE||'))' \r\n\t\t             END AS COLUMN_DEFAULT,\r\n\t                 CASE IS_NULLABLE \r\n                        WHEN 'TRUE' THEN 'YES' \r\n\t\t                ELSE 'NO' \r\n\t\t                END AS IS_NULLABLE\r\n                FROM TABLE_COLUMNS\r\n                WHERE SCHEMA_NAME = CURRENT_SCHEMA \r\n                    AND TABLE_NAME = '@'||@table ORDER BY POSITION;";
	}

	protected override string GetTableStructureQuery()
	{
		return "SELECT COLUMN_NAME,\r\n                     LOWER(CASE DATA_TYPE_NAME \r\n                     WHEN 'NCLOB' THEN 'NTEXT' \r\n                     WHEN 'INTEGER' THEN 'INT' \r\n                     WHEN 'TIMESTAMP' THEN 'DATETIME' \r\n                     WHEN 'DECIMAL' THEN 'NUMERIC' \r\n                     ELSE DATA_TYPE_NAME END) AS DATA_TYPE,\r\n                     CASE WHEN DATA_TYPE_NAME LIKE '%CHAR' OR DATA_TYPE_NAME LIKE '%TEXT' THEN '('''||DEFAULT_VALUE||''')'\r\n                     ELSE '(('||DEFAULT_VALUE||'))' END AS COLUMN_DEFAULT,\r\n                     CASE IS_NULLABLE \r\n                     WHEN 'TRUE' THEN 'YES' ELSE 'NO' END AS IS_NULLABLE\r\n                     FROM TABLE_COLUMNS\r\n                     WHERE SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = @table ORDER BY POSITION;";
	}

	protected override string GetSql(DataTable dt, string tableName)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			string empty = string.Empty;
			stringBuilder.Append("SELECT ");
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				switch (Function.ToString(dt.Rows[i]["DATA_TYPE"]).ToUpper())
				{
				case "NVARCHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						stringBuilder.Append(Environment.NewLine + "IFNULL(\"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",'') AS \"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",");
					}
					else
					{
						empty = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						stringBuilder.Append(Environment.NewLine + "IFNULL(\"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\"," + empty + ") AS \"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",");
					}
					break;
				case "NTEXT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						stringBuilder.Append(Environment.NewLine + "IFNULL(\"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",'') AS \"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",");
					}
					else
					{
						empty = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						stringBuilder.Append(Environment.NewLine + "IFNULL(\"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\"," + empty + ") AS \"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",");
					}
					break;
				case "DATETIME":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						stringBuilder.Append(Environment.NewLine + "IFNULL(\"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",CURRENT_DATE) AS \"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",");
					}
					else
					{
						empty = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						stringBuilder.Append(Environment.NewLine + "IFNULL(\"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\"," + empty + ") AS \"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",");
					}
					break;
				case "CHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						stringBuilder.Append(Environment.NewLine + "IFNULL(\"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",'') AS \"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",");
					}
					else
					{
						empty = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						stringBuilder.Append(Environment.NewLine + "IFNULL(\"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\"," + empty + ") AS \"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",");
					}
					break;
				case "BIT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						stringBuilder.Append(Environment.NewLine + "IFNULL(\"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",0) AS \"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",");
					}
					else
					{
						empty = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						stringBuilder.Append(Environment.NewLine + "IFNULL(\"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\"," + empty + ") AS \"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",");
					}
					break;
				default:
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						stringBuilder.Append(Environment.NewLine + "IFNULL(\"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",0) AS \"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",");
					}
					else
					{
						empty = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(2, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 4);
						stringBuilder.Append(Environment.NewLine + "IFNULL(\"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\"," + empty + ") AS \"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\",");
					}
					break;
				}
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append(Environment.NewLine + $"FROM \"@{tableName}\";");
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "Function get HANA Script ERROR ";
		}
	}

	protected override string GetQueryText(DataTable dtStr, string tableName, string tempName)
	{
		try
		{
			string text = "";
			text += $"CREATE LOCAL TEMPORARY TABLE #LocalTmp AS (SELECT TOP 0 * FROM \"@{tableName}\"); ";
			text += "\n";
			text += $"EXEC 'INSERT INTO #LocalTmp (SELECT * FROM \"{tempName}\")';";
			text += "\n";
			text += $"DELETE FROM \"@{tableName}\";";
			text += "\n";
			if (dtStr.AsEnumerable().Any((DataRow t) => Function.ToString(t["COLUMN_NAME"]).Equals("Code") && Function.ToString(t["DATA_TYPE"]).ToUpper().Equals("INT")))
			{
				text += string.Format("delete a from \"@{0}\" a where not exists(select 1 from \"{1}\" where \"Code\" = a.\"Code\");\r\n                            Delete a from \"{1}\" a INNER JOIN \"@{0}\" b ON a.\"Code\" = b.\"Code\"; ", tableName, tempName);
			}
			string text2 = "";
			string text3 = "";
			for (int num = 0; num < dtStr.Rows.Count; num++)
			{
				string text4 = Function.ToString(dtStr.Rows[num]["COLUMN_NAME"]);
				if (!(text4 == "Code") || !Function.ToString(dtStr.Rows[num]["DATA_TYPE"]).ToUpper().Equals("INT"))
				{
					text2 += $"\"{text4}\",";
					text3 += $"nullif(trim(\"{text4}\"), ''),";
				}
			}
			text2 = text2.Substring(0, text2.Length - 1);
			text3 = text3.Substring(0, text3.Length - 1);
			text += "\n";
			text += $" INSERT INTO \"@{tableName}\" ({text2}) select {text3} from #LocalTmp;";
			text += "\n";
			return text + "DROP TABLE #LocalTmp;";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected override string GetTableUdt(string tableName)
	{
		return $"\"@{tableName}\"";
	}

	protected override string GenerateTableQuery(string tableName, int objType)
	{
		try
		{
			string result = string.Empty;
			switch (objType)
			{
			case 0:
				result = " CREATE TABLE " + tableName + " (\"Code\" NVARCHAR(50) NOT NULL, \"Name\" NVARCHAR(100) NOT NULL);";
				break;
			case 1:
				result = "CREATE TABLE " + tableName + " (\"Code\" NVARCHAR(50) NOT NULL, \"Name\" NVARCHAR(100) NULL, \"DocEntry\" INT NOT NULL, \"Canceled\" CHAR(1) NULL, \"Object\" NVARCHAR(20) NULL, \"LogInst\" INT NULL, \r\n                                \"UserSign\" INT NULL, \"Transfered\" CHAR(1) NULL, \"CreateDate\" TIMESTAMP NULL, \"CreateTime\" SMALLINT NULL, \"UpdateDate\" TIMESTAMP NULL, \"UpdateTime\" SMALLINT NULL, \"DataSource\" CHAR(1) NULL);";
				break;
			case 2:
				result = "CREATE TABLE " + tableName + " (\"Code\" NVARCHAR(50) NOT NULL, \"LineId\" INT NOT NULL, \"Object\" NVARCHAR(20) NULL, \"LogInst\" INT NULL);";
				break;
			case 3:
				result = "CREATE TABLE " + tableName + " (\"DocEntry\" INT NOT NULL, \"DocNum\" INT NULL, \"Period\" INT NULL, \"Instance\" SMALLINT NULL, \"Series\" INT NULL, \"Handwrtten\" CHAR(1) NULL, \"Canceled\" CHAR(1) NULL, \r\n                                   \"Object\" NVARCHAR(20) NULL, \"LogInst\" INT NULL, \"UserSign\" INT NULL, \"Transfered\" CHAR(1) NULL, \"Status\" CHAR(1) NULL, \"CreateDate\" TIMESTAMP NULL, \"CreateTime\" SMALLINT NULL, \"UpdateDate\" TIMESTAMP NULL, \r\n                                   \"UpdateTime\" SMALLINT NULL, \"DataSource\" CHAR(1) NULL, \"RequestStatus\" CHAR(1) NULL, \"Creator\" NVARCHAR(8) NULL, \"Remark\" CLOB NULL);";
				break;
			case 4:
				result = "CREATE TABLE " + tableName + "(\"DocEntry\" INT NOT NULL, \"LineId\" INT NOT NULL, \"VisOrder\" INT NULL, \"Object\" NVARCHAR(20) NULL, \"LogInst\" INT NULL);";
				break;
			case 5:
				result = " CREATE TABLE " + tableName + " (\"Code\" INT GENERATED BY DEFAULT AS IDENTITY NOT NULL, \"Name\" NVARCHAR(100) NULL);";
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	protected override string GetTableOUTBQuery()
	{
		return "select STRING_AGG(\"TableName\", ',') from OUTB;";
	}
}
