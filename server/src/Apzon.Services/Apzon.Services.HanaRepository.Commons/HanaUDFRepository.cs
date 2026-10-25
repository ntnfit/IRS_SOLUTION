using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaUDFRepository : UDFRepository
{
	public HanaUDFRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetDefaultValueUdfQuery()
	{
		return "SELECT\r\n\t                     COLUMN_NAME \"name\",\r\n\t                     CASE DATA_TYPE_NAME WHEN 'NCLOB' THEN 'NTEXT' \r\n\t                     WHEN 'INTEGER' THEN 'INT' \r\n\t                     WHEN 'TIMESTAMP' THEN 'DATETIME' \r\n\t                     WHEN 'DECIMAL' THEN 'NUMERIC' \r\n\t                     ELSE DATA_TYPE_NAME \r\n\t                     END AS \"datatype\"\r\n                    FROM TABLE_COLUMNS\r\n                    WHERE SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME=@TableName AND COLUMN_NAME LIKE 'U\\_%' ESCAPE '\\';";
	}

	protected override string GetStructureTableQuery()
	{
		return "SELECT\r\n\t                     COLUMN_NAME,\r\n\t                     CASE DATA_TYPE_NAME WHEN 'NCLOB' THEN 'NTEXT' \r\n\t                     WHEN 'INTEGER' THEN 'INT' \r\n\t                     WHEN 'TIMESTAMP' THEN 'DATETIME' \r\n\t                     WHEN 'DECIMAL' THEN 'NUMERIC' \r\n\t                     ELSE DATA_TYPE_NAME \r\n\t                     END AS DATA_TYPE,\r\n                            CASE WHEN DATA_TYPE_NAME LIKE '%CHAR' OR DATA_TYPE_NAME LIKE '%TEXT' THEN '('''||DEFAULT_VALUE||''')'\r\n                         ELSE '(('||DEFAULT_VALUE||'))' \r\n                         END AS COLUMN_DEFAULT,\r\n                            CASE IS_NULLABLE WHEN 'TRUE' \r\n\t                        THEN 'YES' \r\n\t                        ELSE 'NO' \r\n\t                        END AS IS_NULLABLE,\r\n                    FROM TABLE_COLUMNS\r\n                    WHERE SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME=@TableName;";
	}

	protected override string GetSql(DataTable dt, string tableName)
	{
		try
		{
			string text = "SELECT ";
			for (int i = 0; i < Function.ParseInt(dt.Rows.Count); i++)
			{
				dt.Rows[i]["COLUMN_NAME"] = "\"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\"";
				switch (Function.ToString(dt.Rows[i]["DATA_TYPE"]).ToUpper())
				{
				case "NVARCHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text6 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text6 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "NTEXT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text7 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text7 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "DATETIME":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",NOW()) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text5 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text5 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "CHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text3 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text3 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "BIT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text4 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text4 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				default:
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text2 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(2, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 4);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text2 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				}
			}
			text = text.Substring(0, text.Length - 1);
			return text + Environment.NewLine + "FROM " + TableCufd(tableName) + " Where \"TableID\"=@TableId;";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return "Function get SQL ERROR ";
		}
	}

	protected override string GetAddNewRowQuery()
	{
		return "SELECT @TableName AS \"TableID\", -1 AS \"FieldID\", '' AS \"AliasID\", '' AS \"Descr\", 'A' AS \"TypeID\", '' AS \"EditType\", 10 AS \"SizeID\", 10 AS \"EditSize\", '' AS \"Dflt\", 'N' AS \"NotNull\", 'N' AS \"IndexID\", '' AS \"RTable\", 0 AS \"RField\", '' AS \"Action\", 'N' AS \"Sys\", NOW() AS \"DfltDate\", '' AS \"RelUDO\", '' AS \"ValidRule\", '' AS \"RelSO\", '' AS \"RThrdPTab\", '' AS \"RThrdPFld\", 'A' AS \"TypeID1\", '' AS \"EditType1\" FROM DUMMY;";
	}

	protected override string GetListUDFFieldSQLQuery()
	{
		return "SELECT string_agg(COLUMN_NAME || ' ' || DATA_TYPE_NAME || ',' ) FROM TABLE_COLUMNS WHERE TABLE_NAME = @tableName AND LEFT(COLUMN_NAME, 2) = 'U_' AND SCHEMA_NAME = CURRENT_SCHEMA ;";
	}

	protected override string GetNewFieldIdQuery(string tableID)
	{
		return "Select max(IFNULL(\"FieldID\",0)) FROM " + TableCufd(tableID) + " WHERE \"TableID\"=@TableId; ";
	}

	protected override string CheckExistColumnQuery()
	{
		return "SELECT TOP 1 a.COLUMN_NAME AS \"Name\", b.TYPE_NAME AS \"Type\", a.LENGTH AS \"MaxLength\" FROM TABLE_COLUMNS a INNER JOIN SYS.DATA_TYPES b ON a.DATA_TYPE_ID = b.TYPE_ID  WHERE TABLE_NAME = @table AND a.COLUMN_NAME = @col and SCHEMA_NAME = CURRENT_SCHEMA;";
	}

	protected override string AddColumn(DataTable dtSet, string tableID, string aliasID, string notNull, string dflt)
	{
		try
		{
			string text = Function.ReplaceSQLInjectCharInColumnName(Function.ToString(dtSet.Rows[0]["Name"]));
			string text2 = Function.ReplaceSQLInjectCharInColumnName(Function.ToString(dtSet.Rows[0]["Type"]));
			string text3 = "";
			if (Function.ToString(dtSet.Rows[0]["Type"]).Equals("nvarchar"))
			{
				text3 = text3 + "exec 'ALTER TABLE " + tableID + " ADD( \"" + text + "\" " + Function.ToString(dtSet.Rows[0]["Type"]) + "(" + Function.ParseInt(dtSet.Rows[0]["MaxLength"]) + ") ";
			}
			else
			{
				text3 = ((!Function.ToString(dtSet.Rows[0]["Type"]).Equals("numeric")) ? (text3 + " exec 'ALTER TABLE " + tableID + " ADD(\"" + text + "\" " + text2) : (text3 + "exec 'ALTER TABLE " + tableID + " ADD(\"" + text + "\" " + text2 + "(19,6)"));
			}
			string result;
			if (notNull == "Y")
			{
				result = (string.IsNullOrEmpty(dflt) ? (text3 + " NOT NULL);';") : (text3 + " NOT NULL DEFAULT ''" + dflt + "'');';"));
			}
			else
			{
				result = (string.IsNullOrEmpty(dflt) ? (text3 + " NULL);';") : (text3 + " NULL DEFAULT ''" + dflt + "'');';"));
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	protected override string UpdateColumn(DataTable dtCheck, DataTable dtSet, string tableID, string aliasID, string notNull, string dflt)
	{
		try
		{
			string text = "";
			string text2 = Function.ReplaceSQLInjectCharInColumnName(Function.ToString(dtSet.Rows[0]["Name"]));
			string text3 = Function.ReplaceSQLInjectCharInColumnName(Function.ToString(dtSet.Rows[0]["Type"]));
			int num = Function.ParseInt(dtCheck.Rows[0]["MaxLength"]);
			if (notNull == "Y")
			{
				text = text + " UPDATE " + tableID + " SET \"" + text2 + "\" = '" + dflt + "' WHERE \"" + text2 + "\" IS NULL; ";
			}
			if (Function.ToString(dtCheck.Rows[0]["Type"]) == Function.ToString(dtSet.Rows[0]["Type"]) && num / 2 <= Function.ParseInt(dtSet.Rows[0]["MaxLength"]) && Function.ToString(dtSet.Rows[0]["Type"]).Equals("nvarchar"))
			{
				text = text + "exec 'ALTER TABLE " + tableID + " ALTER(\"" + text2 + "\" " + text3 + "(" + Function.ParseInt(dtSet.Rows[0]["MaxLength"]) + ") ";
			}
			else if (Function.ToString(dtCheck.Rows[0]["Type"]) == Function.ToString(dtSet.Rows[0]["Type"]) && num <= Function.ParseInt(dtSet.Rows[0]["MaxLength"]) && (Function.ToString(dtSet.Rows[0]["Type"]).Equals("varchar") || Function.ToString(dtSet.Rows[0]["Type"]).Equals("char")))
			{
				text = text + "exec 'ALTER TABLE " + tableID + " ALTER(\"" + text2 + "\" " + text3 + "(" + Function.ParseInt(dtSet.Rows[0]["MaxLength"]) + ") ";
			}
			else if (text3.Equals("numeric"))
			{
				text = text + "exec 'ALTER TABLE " + tableID + " ALTER(\"" + text2 + "\" " + text3 + "(19,6)";
			}
			else
			{
				text = ((!text3.Equals("int") && !text3.Equals("ntext") && !text3.Equals("datetime")) ? (text + "exec 'ALTER TABLE " + tableID + " ALTER(\"" + text2 + "\" " + text3 + "(" + ((num > Function.ParseInt(dtSet.Rows[0]["MaxLength"])) ? num : Function.ParseInt(dtSet.Rows[0]["MaxLength"])) + ")") : (text + "exec 'ALTER TABLE " + tableID + " ALTER(\"" + text2 + "\" " + text3));
			}
			if (notNull == "Y")
			{
				text = (string.IsNullOrEmpty(dflt) ? (text + " NOT NULL);';") : (text + " NOT NULL DEFAULT ''" + dflt + "'');';"));
			}
			else
			{
				text = (string.IsNullOrEmpty(dflt) ? (text + " NULL);';") : (text + " NULL DEFAULT ''" + dflt + "'');';"));
			}
			return text + Environment.NewLine;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			DataTable dataTable = ds.Tables[0];
			DataTable dataTable2 = ds.Tables[1];
			DataTable dataTable3 = ds.Tables[2];
			DataTable dataTable4 = ds.Tables[3];
			string tbl = Function.ReplaceSQLInjectCharInColumnName(Function.ToString(dataTable.Rows[0]["TableID"]));
			string[] array = new string[30];
			if (!string.IsNullOrEmpty(tbl) && tbl.Contains("@"))
			{
				array = tbl.Split(',');
			}
			else
			{
				DataRow dataRow = CreateTableName().AsEnumerable().FirstOrDefault((DataRow t) => tbl != null && Function.ToString(t["TableName"]).Contains(tbl));
				if (dataRow != null)
				{
					array = Function.ToString(dataRow["TableName"]).Split(',');
				}
			}
			string[] array2 = array;
			foreach (string text in array2)
			{
				List<SqlParameter> list = new List<SqlParameter>();
				DataRow dataRow2 = dataTable.Rows[0];
				short? num2 = Function.ParseShort(dataRow2["FieldID"]);
				if (Function.ToString(dataRow2["Validation"]) != "V" && Function.ToString(dataRow2["Validation"]) != "N")
				{
					string query = string.Format("select a.\"TableID\", a.\"FieldID\", a.\"RTable\", a.\"RelUDO\", a.\"RelSO\", \r\n                                    (select count(1) from {1} where \"TableID\" = @TableId and \"FieldID\" = @FieldId) as \"ValidCount\"\r\n                                    from {0} a\r\n                                    where a.\"TableID\" = @TableId and a.\"FieldID\" = @FieldId;", TableCufd(text), Tableufd1(text));
					DataTable dataTable5 = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@TableId", text), new SqlParameter("@FieldId", num2));
					if (dataTable5.IsNotNull() && (!string.IsNullOrEmpty(Function.ToString(dataTable5.Rows[0]["RTable"])) || !string.IsNullOrEmpty(Function.ToString(dataTable5.Rows[0]["RelUDO"])) || !string.IsNullOrEmpty(Function.ToString(dataTable5.Rows[0]["RelSO"])) || Function.ParseInt(dataTable5.Rows[0]["ValidCount"]) > 0))
					{
						return new HttpResult(Enums.MessageCode.Error, "You cannot set two validation methods for one UDF");
					}
				}
				if (num2 == Function.ParseShort(-1))
				{
					num2 = GetNewFieldId(text);
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder("DELETE FROM " + TableCufd(text) + " WHERE \"TableID\"=@TableId and \"FieldID\"=@fieldId;");
					list.Add(new SqlParameter("@TableId", text));
					list.Add(new SqlParameter("@fieldId", num2));
					ExecuteNonQuery(stringBuilder.ToString(), CommandType.Text, list.ToArray());
				}
				list = new List<SqlParameter>();
				string inputStr = Function.ToString(dataRow2["AliasID"]);
				inputStr = Function.ReplaceSQLInjectCharInColumnName(inputStr);
				string notNull = Function.ToString(dataRow2["NotNull"]);
				string dflt = Function.ToString(dataRow2["Dflt"]);
				StringBuilder stringBuilder2 = new StringBuilder();
				DataTable dataTable6 = CheckExistColumn(text, inputStr);
				DataTable dtSet = SetTypeCol(dataRow2);
				if (dataTable6.IsNotNull())
				{
					stringBuilder2.Append(UpdateColumn(dataTable6, dtSet, text, inputStr, notNull, dflt));
				}
				else
				{
					stringBuilder2.Append(AddColumn(dtSet, text, inputStr, notNull, dflt));
				}
				stringBuilder2.Append(" IF NOT EXISTS(SELECT 1 FROM " + TableCufd(text) + " WHERE \"TableID\"=@TableId AND \"AliasID\"=@AliasID) THEN \r\n                                    INSERT INTO " + TableCufd(text) + "(\"TableID\",\"FieldID\",\"AliasID\",\"Descr\",\"TypeID\",\"EditType\",\"SizeID\",\"EditSize\",\"Dflt\",\"NotNull\",\"IndexID\",\"RTable\",\"RField\",\"Action\",\"Sys\",\"DfltDate\",\"RelUDO\",\"ValidRule\",\"RelSO\",\"RThrdPTab\",\"RThrdPFld\")\r\n                                          VALUES(@TableId, @FieldID, @AliasID, @Descr, @TypeID,@EditType,@SizeID,@EditSize\r\n                                  , @Dflt, @NotNull, @IndexID, @RTable, @RField, @Action, 'N', CURRENT_DATE, @RelUDO, @ValidRule, @RelSO\r\n                                  , @RThrdPTab, @RThrdPFld);\r\n                            END IF;");
				list.Add(new SqlParameter("@TableId", text));
				list.Add(new SqlParameter("@FieldID", num2));
				list.Add(new SqlParameter("@AliasID", inputStr));
				list.Add(new SqlParameter("@Descr", Function.ToString(dataRow2["Descr"])));
				list.Add(new SqlParameter("@TypeID", Function.ToString(dataRow2["TypeID"])));
				list.Add(new SqlParameter("@EditType", Function.ToString(dataRow2["EditType"])));
				list.Add(new SqlParameter("@SizeID", Function.ParseInt(dataRow2["SizeID"])));
				if (dataRow2["EditSize"] == DBNull.Value)
				{
					list.Add(new SqlParameter("@EditSize", DBNull.Value));
				}
				else
				{
					list.Add(new SqlParameter("@EditSize", Function.ParseInt(dataRow2["EditSize"])));
				}
				list.Add(new SqlParameter("@Dflt", Function.ToString(dataRow2["Dflt"])));
				list.Add(new SqlParameter("@NotNull", Function.ToString(dataRow2["NotNull"])));
				list.Add(new SqlParameter("@IndexID", Function.ToString(dataRow2["IndexID"])));
				list.Add(new SqlParameter("@RTable", Function.ToString(dataRow2["RTable"])));
				list.Add(new SqlParameter("@RField", Function.ParseInt(dataRow2["RField"])));
				list.Add(new SqlParameter("@Action", Function.ToString(dataRow2["Action"])));
				list.Add(new SqlParameter("@RelUDO", Function.ToString(dataRow2["RelUDO"])));
				list.Add(new SqlParameter("@ValidRule", Function.ToString(dataRow2["ValidRule"])));
				list.Add(new SqlParameter("@RelSO", Function.ToString(dataRow2["RelSO"])));
				list.Add(new SqlParameter("@RThrdPTab", Function.ToString(dataRow2["RThrdPTab"])));
				list.Add(new SqlParameter("@RThrdPFld", Function.ToString(dataRow2["RThrdPFld"])));
				if (ExecuteNonQuery(stringBuilder2.ToString(), CommandType.Text, list.ToArray()) <= 0)
				{
					return new HttpResult(Enums.MessageCode.RecordAlreadyExists, "This entry already exists in the following tables (" + text + ")");
				}
				if (dataTable2.IsNotNull())
				{
					StringBuilder stringBuilder3 = new StringBuilder("DELETE FROM " + Tableufd1(text) + " WHERE \"TableID\"= @TableID and \"FieldID\"= @FieldID;" + Environment.NewLine);
					list = new List<SqlParameter>();
					list.Add(new SqlParameter("@TableID", Function.ToString(text)));
					list.Add(new SqlParameter("@FieldID", Function.ParseInt(num2)));
					int num3 = 0;
					int num4 = 0;
					foreach (DataRow row in dataTable2.Rows)
					{
						stringBuilder3.Append("INSERT INTO " + Tableufd1(text) + "(\"TableID\",\"FieldID\",\"IndexID\",\"FldValue\",\"Descr\",\"FldDate\")\r\n                                      VALUES(@TableID, @FieldID,@IndexID" + num3 + ",@FldValue" + num3 + ",@Descr" + num3 + ",CURRENT_DATE);" + Environment.NewLine);
						list.Add(new SqlParameter("@IndexID" + num3, num4++));
						list.Add(new SqlParameter("@FldValue" + num3, Function.ToString(row["FldValue"])));
						list.Add(new SqlParameter("@Descr" + num3, Function.ToString(row["Descr"])));
						num3++;
					}
					ExecuteNonQuery(stringBuilder3.ToString(), CommandType.Text, list.ToArray());
				}
				else
				{
					StringBuilder stringBuilder4 = new StringBuilder("DELETE FROM " + Tableufd1(text) + " WHERE \"TableID\"= @TableID and \"FieldID\"= @FieldID;");
					list = new List<SqlParameter>();
					list.Add(new SqlParameter("@TableID", Function.ToString(text)));
					list.Add(new SqlParameter("@FieldID", Function.ParseInt(num2)));
					ExecuteNonQuery(stringBuilder4.ToString(), CommandType.Text, list.ToArray());
				}
				if (dataTable3.IsNotNull())
				{
					StringBuilder stringBuilder5 = new StringBuilder("DELETE FROM " + TableOukd(text) + " WHERE \"TableName\"=@TableId;" + Environment.NewLine);
					list = new List<SqlParameter>();
					list.Add(new SqlParameter("@TableId", text));
					int num5 = 0;
					foreach (DataRow row2 in dataTable3.Rows)
					{
						stringBuilder5.Append(" INSERT INTO " + TableOukd(text) + "(\"TableName\",\"KeyId\",\"KeyName\",\"UniqueKey\",\"Action\")\r\n                                     VALUES(@TableId,@KeyId" + num5 + ",@KeyName" + num5 + ",@UniqueKey" + num5 + ",@Action" + num5 + "); " + Environment.NewLine);
						list.Add(new SqlParameter("@KeyId" + num5, Function.ParseInt(row2["KeyId"])));
						list.Add(new SqlParameter("@KeyName" + num5, Function.ToString(row2["KeyName"])));
						list.Add(new SqlParameter("@UniqueKey" + num5, Function.ToString(row2["UniqueKey"])));
						list.Add(new SqlParameter("@Action" + num5, Function.ToString(row2["Action"])));
						num5++;
					}
					ExecuteNonQuery(stringBuilder5.ToString(), CommandType.Text, list.ToArray());
				}
				if (dataTable4.IsNotNull())
				{
					StringBuilder stringBuilder6 = new StringBuilder();
					list = new List<SqlParameter>();
					list.Add(new SqlParameter("@TableId", text));
					int num6 = 0;
					foreach (var item in (from r in dataTable4.AsEnumerable()
						select new
						{
							KeyId = r["KeyId"]
						}).Distinct().ToList())
					{
						stringBuilder6.Append(" DELETE FROM " + TableUkd1(text) + " WHERE \"TableName\"=@TableId AND \"KeyId\"=@KeyId" + num6 + ";" + Environment.NewLine);
						list.Add(new SqlParameter("@KeyId" + num6, item.KeyId));
						num6++;
					}
					ExecuteNonQuery(stringBuilder6.ToString(), CommandType.Text, list.ToArray());
					stringBuilder6 = new StringBuilder();
					list = new List<SqlParameter>();
					list.Add(new SqlParameter("@TableId", text));
					num6 = 0;
					foreach (DataRow row3 in dataTable4.Rows)
					{
						stringBuilder6.Append(" INSERT INTO " + TableUkd1(text) + "(\"TableName\",\"KeyId\",\"SubKeyId\",\"ColAlias\")\r\n                                     VALUES(@TableId,@KeyId" + num6 + ",@SubKeyId" + num6 + ",@ColAlias" + num6 + "); " + Environment.NewLine);
						list.Add(new SqlParameter("@KeyId" + num6, Function.ParseInt(row3["KeyId"])));
						list.Add(new SqlParameter("@SubKeyId" + num6, Function.ParseInt(row3["SubKeyId"])));
						list.Add(new SqlParameter("@ColAlias" + num6, Function.ParseInt(row3["ColAlias"])));
						num6++;
					}
					ExecuteNonQuery(stringBuilder6.ToString(), CommandType.Text, list.ToArray());
				}
				else
				{
					list = new List<SqlParameter>();
					stringBuilder2 = new StringBuilder("SELECT 1 FROM " + TableUkd1(text) + " WHERE \"TableName\" = @tableId;");
					list.Add(new SqlParameter("@TableId", text));
					if (ExecuteDataTable(stringBuilder2.ToString(), CommandType.Text, new SqlParameter("@tableId", text)).IsNotNull())
					{
						stringBuilder2 = new StringBuilder(" DELETE FROM " + TableUkd1(text) + " WHERE \"TableName\"=@TableId;");
						ExecuteNonQuery(stringBuilder2.ToString(), CommandType.Text, list.ToArray());
					}
				}
				GetOUKD(text).IsNotNull();
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override string RemoveUDFQuery(string slp, string tableCUFD, string tableUFD1, string columnName)
	{
		return string.Format("if exists(select 1 from SYS.TABLE_COLUMNS WHERE SCHEMA_NAME = current_schema and TABLE_NAME = @table and COLUMN_NAME = 'U_{1}')\r\n                THEN\r\n                    EXECUTE IMMEDIATE 'ALTER TABLE {0} DROP(\"U_{1}\")';\r\n                END IF;\r\n                DELETE FROM {2} WHERE \"TableID\" = @table AND \"FieldID\" = @field;\r\n                DELETE FROM {3} WHERE \"TableID\" = @table AND \"FieldID\" = @field;", slp, columnName, tableCUFD, tableUFD1);
	}

	protected override string GetListFieldNotValidatedQuery()
	{
		return "select 'U_' || \"AliasID\" from CUFD a where \"TableID\" = @tableID and ifnull(\"RTable\",'')='' and ifnull(\"RelUDO\",'')='' and ifnull(\"RelSO\",'')=''\r\nand not exists(select 1 from UFD1 where \"TableID\"=a.\"TableID\" and \"FieldID\"=a.\"FieldID\");";
	}
}
