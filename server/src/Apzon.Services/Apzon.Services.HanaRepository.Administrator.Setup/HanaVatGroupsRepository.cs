using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaVatGroupsRepository : VatGroupsRepository
{
	public HanaVatGroupsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetVatGroupIQuery()
	{
		return "SELECT \"Code\", \"Name\", \"Rate\" FROM OVTG WHERE \"Category\" = 'I' AND IFNULL(\"Locked\", 'N') = 'N' AND IFNULL(\"Inactive\", 'N') = 'N' ORDER BY \"Rate\";";
	}

	protected override string GetVatGroupOQuery()
	{
		return "SELECT \"Code\", \"Name\", \"Rate\" FROM OVTG WHERE \"Category\" = 'O' AND IFNULL(\"Locked\", 'N') = 'N' AND IFNULL(\"Inactive\", 'N') = 'N' ORDER BY \"Rate\";";
	}

	protected override string ShowVatGroupIQuery()
	{
		return "SELECT \"Code\", \"Name\", \"Rate\", IFNULL(\"EffecDate\", NOW()) AS \"EffecDate\", CASE WHEN \"Category\" = 'I' THEN 'Input Taxt' WHEN \"Category\" = 'O' THEN 'Output Taxt' END AS \"Category\", CASE WHEN \"Category\" = 'N' THEN 'No' WHEN \"Category\" = 'Y' THEN 'Yes' END AS \"Category\", IFNULL(\"Account\", '') AS \"Account\" FROM OVTG WHERE \"Category\" = 'I' ORDER BY \"Rate\";";
	}

	protected override string ShowVatGroupOQuery()
	{
		return "SELECT \"Code\", \"Name\", \"Rate\", IFNULL(\"EffecDate\", NOW()) AS \"EffecDate\", CASE WHEN \"Category\" = 'I' THEN 'Input Taxt' WHEN \"Category\" = 'O' THEN 'Output Taxt' END AS \"Category\", CASE WHEN \"Category\" = 'N' THEN 'No' WHEN \"Category\" = 'Y' THEN 'Yes' END AS \"Category\", IFNULL(\"Account\", '') AS \"Account\" FROM OVTG WHERE \"Category\" = 'O' ORDER BY \"Rate\";";
	}

	protected override string GetStructureTableQuery(string tableName)
	{
		return "SELECT\r\n\t                     COLUMN_NAME,\r\n\t                     '(('||DEFAULT_VALUE||'))' AS COLUMN_DEFAULT,\r\n\t                     CASE IS_NULLABLE WHEN 'TRUE' \r\n\t                        THEN 'YES' \r\n\t                        ELSE 'NO' \r\n\t                        END AS IS_NULLABLE,\r\n\t                     CASE DATA_TYPE_NAME WHEN 'NCLOB' THEN 'NTEXT' \r\n\t                     WHEN 'INTEGER' THEN 'INT' \r\n\t                     WHEN 'TIMESTAMP' THEN 'DATETIME' \r\n\t                     WHEN 'DECIMAL' THEN 'NUMERIC' \r\n\t                     ELSE DATA_TYPE_NAME \r\n\t                     END AS DATA_TYPE\r\n                    FROM TABLE_COLUMNS \r\n                    WHERE SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = '" + tableName + "';";
	}

	protected override string GetSql(DataTable dt, string tableName)
	{
		try
		{
			string text = "SELECT ";
			for (int i = 0; i < Function.ParseInt(dt.Rows.Count); i++)
			{
				switch (Function.ToString(dt.Rows[i]["DATA_TYPE"]).ToUpper())
				{
				case "NVARCHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["DEFAULT_VALUE"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text6 = Function.ToString(dt.Rows[i]["DEFAULT_VALUE"]);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text6 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "NCLOB":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["DEFAULT_VALUE"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text7 = Function.ToString(dt.Rows[i]["DEFAULT_VALUE"]);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text7 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "TIMESTAMP":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["DEFAULT_VALUE"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",NOW()) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text5 = Function.ToString(dt.Rows[i]["DEFAULT_VALUE"]);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text5 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "CHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["DEFAULT_VALUE"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text3 = Function.ToString(dt.Rows[i]["DEFAULT_VALUE"]);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text3 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "TINYINT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["DEFAULT_VALUE"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text4 = Function.ToString(dt.Rows[i]["DEFAULT_VALUE"]);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text4 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				default:
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["DEFAULT_VALUE"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text2 = Function.ToString(dt.Rows[i]["DEFAULT_VALUE"]);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text2 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				}
			}
			text = text.Substring(0, text.Length - 1);
			return text + Environment.NewLine + "FROM " + tableName + ";";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "Function get SQL ERROR ";
		}
	}
}
