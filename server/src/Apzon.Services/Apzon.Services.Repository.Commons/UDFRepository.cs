using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Services.Interfaces.Commons;
using SAPbobsCOM;

namespace Apzon.Services.Repository.Commons;

public class UDFRepository : BaseService<DataSet>, IUDFs
{
	public UDFRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public List<Common.Table_Code> GetDefaultValueUdf(string tableName)
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>();
			string defaultValueUdfQuery = GetDefaultValueUdfQuery();
			DataTable dataTable = ExecuteDataTable(defaultValueUdfQuery, CommandType.Text, new SqlParameter("@TableName", tableName));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					Common.Table_Code item = new Common.Table_Code
					{
						Code = Function.ToString(dataTable.Rows[i]["name"]),
						Name = Function.ToString(dataTable.Rows[i]["datatype"])
					};
					list.Add(item);
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new List<Common.Table_Code>();
	}

	protected virtual string GetDefaultValueUdfQuery()
	{
		return "SELECT COLUMN_NAME name,DATA_TYPE datatype FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@TableName AND COLUMN_NAME LIKE 'U\\_%' ESCAPE '\\'";
	}

	public List<Common.Table_Code> GetValidValue(string tableId, string fieldId)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("apz_udf_getvalid", CommandType.StoredProcedure, new SqlParameter("@TableId", tableId), new SqlParameter("@FieldId", fieldId));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				List<Common.Table_Code> list = new List<Common.Table_Code>();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					Common.Table_Code item = new Common.Table_Code
					{
						Code = Function.ToString(dataTable.Rows[i]["Code"]),
						Name = Function.ToString(dataTable.Rows[i]["Name"])
					};
					list.Add(item);
				}
				return list;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new List<Common.Table_Code>();
	}

	public List<Common.Table_Code> GetLinkedTable(string linkedTable, string userName, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(linkedTable))
			{
				if (linkedTable.StartsWith("@"))
				{
					linkedTable = linkedTable.Remove(0, 1);
				}
				list.Add(new SqlParameter("@LinkedTable", linkedTable));
			}
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (branch != -1)
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			DataTable dataTable = ExecuteDataTable("Apz_Udf_GetLinkedTable", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				List<Common.Table_Code> list2 = new List<Common.Table_Code>();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					Common.Table_Code item = new Common.Table_Code
					{
						Code = Function.ToString(dataTable.Rows[i]["Code"]),
						Name = Function.ToString(dataTable.Rows[i]["Name"])
					};
					list2.Add(item);
				}
				return list2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new List<Common.Table_Code>();
	}

	public DataTable LoadDataOUTB()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM OUTB;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public DataTable GetListUdf(string tableName)
	{
		try
		{
			DataTable structureTable = GetStructureTable("CUFD");
			string sql = GetSql(structureTable, tableName);
			return ExecuteDataTable(sql, CommandType.Text, new SqlParameter("@TableId", tableName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public DataTable GetListKeyAlias(string tableName)
	{
		try
		{
			if (tableName == null)
			{
				return new DataTable();
			}
			string query = "select \"AliasID\", \"Descr\" from " + TableCufd(tableName) + " where \"TableID\" = @table;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@table", tableName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public DataTable GetListCUFD(string tableName)
	{
		try
		{
			string query = "SELECT *, CASE WHEN \"TypeID\" = 'N' AND \"EditType\" = 'T' THEN 'D' WHEN \"TypeID\" = 'A' AND \"EditType\" = 'I' THEN 'M' WHEN \"TypeID\" = 'M' AND \"EditType\" = '' THEN 'A' ELSE \"TypeID\" END AS \"TypeID1\", CASE WHEN \"TypeID\" = 'M' AND \"EditType\" = '' THEN 'B' ELSE \"EditType\" END AS \"EditType1\" \r\n                FROM  " + TableCufd(tableName) + " WHERE \"TableID\" = @table ORDER BY \"FieldID\"; ";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@table", tableName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public DataTable GetStructureTable(string tableName)
	{
		try
		{
			string structureTableQuery = GetStructureTableQuery();
			DataTable dataTable = ExecuteDataTable(structureTableQuery, CommandType.Text, new SqlParameter("@TableName", tableName));
			if (dataTable.IsNotNull())
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	protected virtual string GetStructureTableQuery()
	{
		return "SELECT COLUMN_NAME,DATA_TYPE,COLUMN_DEFAULT,IS_NULLABLE\r\n                FROM INFORMATION_SCHEMA.COLUMNS\r\n                WHERE TABLE_NAME = @TableName";
	}

	protected virtual string GetSql(DataTable dt, string tableName)
	{
		try
		{
			string text = "SELECT ";
			for (int i = 0; i < Function.ParseInt(dt.Rows.Count); i++)
			{
				switch (Function.ToString(dt.Rows[i]["DATA_TYPE"]).ToUpper())
				{
				case "NVARCHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text6 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text6 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "NTEXT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text7 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text7 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "DATETIME":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",GETDATE()) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text5 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text5 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "CHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text3 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text3 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "BIT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text4 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text4 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				default:
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text2 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(2, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 4);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text2 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				}
			}
			text = text.Substring(0, text.Length - 1);
			return text + Environment.NewLine + "FROM " + TableCufd(tableName) + " Where TableID=@TableId";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return "Function get SQL ERROR ";
		}
	}

	public string TableCufd(string tableName)
	{
		if (!tableName.StartsWith("APZ_"))
		{
			return "CUFD";
		}
		return "APZ_CUFD";
	}

	public string Tableufd1(string tableName)
	{
		if (!tableName.StartsWith("APZ_"))
		{
			return "UFD1";
		}
		return "APZ_UFD1";
	}

	public string TableOukd(string tableName)
	{
		if (!tableName.StartsWith("APZ_"))
		{
			return "OUKD";
		}
		return "APZ_OUKD";
	}

	public string TableUkd1(string tableName)
	{
		if (!tableName.StartsWith("APZ_"))
		{
			return "UKD1";
		}
		return "APZ_UKD1";
	}

	public DataTable GetAddNewRow(string tableName)
	{
		try
		{
			string addNewRowQuery = GetAddNewRowQuery();
			return ExecuteDataTable(addNewRowQuery, CommandType.Text, new SqlParameter("@TableName", tableName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	protected virtual string GetAddNewRowQuery()
	{
		return "SELECT @TableName AS TableID ,-1 AS FieldID,'' AS AliasID, \r\n                            '' AS Descr,\r\n                            'A' AS TypeID,\r\n                            '' AS EditType,\r\n                            10 AS SizeID,\r\n                            10 AS EditSize,\r\n                            '' AS Dflt,\r\n                            'N' AS NotNull,\r\n                            'N' AS IndexID,\r\n                            '' AS RTable,\r\n                            0 AS RField,\r\n                            '' AS Action,\r\n                            'N' AS Sys,\r\n                            GETDATE() AS DfltDate,\r\n                            '' AS RelUDO,\r\n                            '' AS ValidRule,\r\n                            '' AS RelSO,\r\n                            '' AS RThrdPTab,\r\n                            '' AS RThrdPFld\r\n                            ,'A' TypeID1,'' EditType1 ";
	}

	public DataTable GetListUDFFieldSQL(string tableName)
	{
		try
		{
			string listUDFFieldSQLQuery = GetListUDFFieldSQLQuery();
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@tableName", tableName)
			};
			return ExecuteDataTable(listUDFFieldSQLQuery, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	protected virtual string GetListUDFFieldSQLQuery()
	{
		return "SELECT  COLUMN_NAME+' '+DATA_TYPE+','\r\n\t                        FROM INFORMATION_SCHEMA.COLUMNS\r\n\t                        WHERE TABLE_NAME = @tableName AND LEFT(COLUMN_NAME,2) ='U_'\r\n\t                        FOR XML PATH('')";
	}

	protected virtual string RemoveUDFQuery(string slp, string tableCUFD, string tableUFD1, string columnName)
	{
		return string.Format("declare @qry nvarchar(max);\r\n                                select @qry = (\r\n                                select  'IF EXISTS(SELECT * FROM sys.default_constraints WHERE name= c.name  AND object_id = OBJECT_ID([s.name].[o.name]))      \r\n                                            Alter Table [{0}] drop CONSTRAINT [c.name];'\r\n                                from sys.default_constraints c \r\n                                    inner join sys.objects o on  c.object_id=o.object_id\r\n                                    inner join sys.schemas s on o.schema_id = s.schema_id\r\n                                where o.type<>'S'\r\n                                and s.name!='sys' and s.name!='sys' and c.name = @Name\r\n                                for xml path(''));\r\n\r\n                                exec sp_executesql @qry;\r\n                                ALTER TABLE dbo.[{0}]  DROP COLUMN U_{1};\r\n                                DELETE FROM {2} WHERE TableID = @table AND FIeldID = @field;\r\n                                DELETE FROM {3} WHERE TableID = @table AND FIeldID = @field;", slp, columnName, tableCUFD, tableUFD1);
	}

	public string RemoveUdf(Tuple<string, int, string> tupe)
	{
		string empty = string.Empty;
		try
		{
			if (!string.IsNullOrEmpty(tupe.Item1))
			{
				DataTable dataTable = CreateTableName();
				if (dataTable.IsNotNull())
				{
					string[] array = new string[30];
					if (tupe.Item1.Contains("@"))
					{
						array = tupe.Item1.Split(',');
					}
					else
					{
						DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["TableName"]).Contains(tupe.Item1));
						if (dataRow != null)
						{
							array = Function.ToString(dataRow["TableName"]).Split(',');
						}
					}
					int num = 0;
					using (DatabaseService.OpenConnection())
					{
						using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
						string[] array2 = array;
						foreach (string text in array2)
						{
							string query = RemoveUDFQuery(text, TableCufd(text), Tableufd1(text), tupe.Item3);
							num += ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@table", text), new SqlParameter("@field", tupe.Item2), new SqlParameter("@Name", Function.ToString("DF_" + text + "_U_" + tupe.Item3)));
						}
						if (num > 0)
						{
							dbTransaction.Commit();
							return "";
						}
						return "No record effected";
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
		return empty;
	}

	public string RemoveUFD1(Tuple<string, int, int> tup)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = " DELETE FROM " + Tableufd1(tup.Item1) + " WHERE \"TableID\"=@table AND \"FieldID\"=@field AND \"IndexID\"=@index;";
				if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@table", tup.Item1), new SqlParameter("@field", tup.Item2), new SqlParameter("@index", tup.Item3)) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				return "No record effected";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	public DataTable GetTableLinked()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT \"TableName\" AS \"Code\",\"Descr\" AS \"Name\" FROM OUTB;", CommandType.Text, (SqlParameter[])null);
			if (dataTable.IsNotNull())
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public DataTable GetObject()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT \"Code\" AS \"Code\",\"Name\" AS \"Name\",\"TableName\" as \"TableName\" FROM OUDO WHERE \"CanFind\"='Y';", CommandType.Text, (SqlParameter[])null);
			if (dataTable.IsNotNull())
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public DataTable GetUfd1(string tableCode, int fieldId)
	{
		try
		{
			string query = "SELECT * FROM " + Tableufd1(tableCode) + " WHERE \"TableID\" = @table AND \"FieldID\" = @field; ";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@table", tableCode), new SqlParameter("@field", fieldId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public string CheckExistUFD1(Tuple<string, int, int> tup)
	{
		try
		{
			string query = "SELECT * FROM " + Tableufd1(tup.Item1) + " WHERE \"TableID\" = @table AND \"FieldID\" = @field AND \"IndexID\" = @index; ";
			if (ExecuteDataTable(query, CommandType.Text, new SqlParameter("@table", tup.Item1), new SqlParameter("@field", tup.Item2), new SqlParameter("@index", tup.Item3)).IsNotNull())
			{
				return "";
			}
			return "Exist!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public DataTable GetUkd1(string tableName, int keyId)
	{
		try
		{
			string query = "SELECT * FROM  " + TableUkd1(tableName) + " WHERE \"TableName\"=@table AND \"KeyId\"=@key;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@table", tableName), new SqlParameter("@key", keyId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetUKD1ByTable(string tableName)
	{
		try
		{
			string query = "SELECT * FROM  " + TableUkd1(tableName) + " WHERE \"TableName\"=@table;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@table", tableName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetOUKD(string tableName)
	{
		try
		{
			string query = "SELECT * FROM " + TableOukd(tableName) + " WHERE \"TableName\"=@table;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@table", tableName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public List<UserFieldsImpl> GetListField(string tableName)
	{
		try
		{
			DataSet dataSet = ExecuteData("APZ_UserFields_GetListField", CommandType.StoredProcedure, new SqlParameter("@TableId", tableName));
			List<UserFieldsImpl> list = new List<UserFieldsImpl>();
			if (dataSet.IsNotNull() && dataSet.Tables.Count > 1)
			{
				DataTable dataTable = dataSet.Tables[0];
				DataTable dataTable2 = dataSet.Tables[1];
				if (dataTable.IsNotNull())
				{
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						DataRow dataRow = dataTable.Rows[i];
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl
						{
							DefaultValue = Function.ToString(dataRow["Dflt"]),
							Description = Function.ToString(dataRow["Descr"]),
							EditSize = Function.ParseLong(dataRow["EditSize"]),
							FieldID = Function.ParseLong(dataRow["FieldID"]),
							LinkedTable = Function.ToString(dataRow["RTable"]),
							LinkedUDO = Function.ToString(dataRow["RelUDO"]),
							LinkedObject = (dataRow.IsExistsColumn("RelSO") ? Function.ToString(dataRow["RelSO"]) : ""),
							Mandatory = (Function.ToString(dataRow["NotNull"]).Equals("Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO),
							Name = Function.ToString(dataRow["AliasID"]),
							Size = Function.ParseLong(dataRow["SizeID"]),
							TableName = Function.ToString(dataRow["TableID"])
						};
						switch (Function.ToString(dataRow["TypeID"]))
						{
						case "A":
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
							break;
						case "M":
							userFieldsImpl.Type = BoFieldTypes.db_Memo;
							break;
						case "N":
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
							break;
						case "D":
							userFieldsImpl.Type = BoFieldTypes.db_Date;
							break;
						case "B":
							userFieldsImpl.Type = BoFieldTypes.db_Float;
							break;
						}
						switch (Function.ToString(dataRow["EditType"]))
						{
						case "S":
							userFieldsImpl.SubType = BoFldSubTypes.st_Sum;
							break;
						case "P":
							userFieldsImpl.SubType = BoFldSubTypes.st_Price;
							break;
						case "Q":
							userFieldsImpl.SubType = BoFldSubTypes.st_Quantity;
							break;
						case "%":
							userFieldsImpl.SubType = BoFldSubTypes.st_Percentage;
							break;
						case "?":
							userFieldsImpl.SubType = BoFldSubTypes.st_Address;
							break;
						case "T":
							userFieldsImpl.SubType = BoFldSubTypes.st_Time;
							break;
						case "R":
							userFieldsImpl.SubType = BoFldSubTypes.st_Rate;
							break;
						case "M":
							userFieldsImpl.SubType = BoFldSubTypes.st_Measurement;
							break;
						case "B":
							userFieldsImpl.SubType = BoFldSubTypes.st_Link;
							break;
						case "I":
							userFieldsImpl.SubType = BoFldSubTypes.st_Image;
							break;
						default:
							userFieldsImpl.SubType = BoFldSubTypes.st_None;
							break;
						}
						if (dataTable2.IsNotNull() && dataTable2.ExistsColumn("TableID") && dataTable2.ExistsColumn("FieldID"))
						{
							DataRow[] array = dataTable2.Select($"TableID='{userFieldsImpl.TableName}' AND FieldID='{userFieldsImpl.FieldID}'");
							if (array.Length != 0)
							{
								userFieldsImpl.ValidValues = new List<ValidValuesMDImpl>();
								DataRow[] array2 = array;
								foreach (DataRow dataRow2 in array2)
								{
									ValidValuesMDImpl item = new ValidValuesMDImpl
									{
										Value = Function.ToString(dataRow2["FldValue"]),
										Description = Function.ToString(dataRow2["Descr"])
									};
									userFieldsImpl.ValidValues.Add(item);
								}
							}
						}
						list.Add(userFieldsImpl);
					}
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return null;
	}

	public DataTable GetKeyFieldOUDO(string tblName)
	{
		try
		{
			(new SqlParameter[1])[0] = new SqlParameter("@Table", tblName);
			DataTable dataTable = ExecuteDataTable("SELECT TOP 1\r\n                                        c.name AS Name,\r\n                                        d.name AS Type,\r\n\t                                    c.max_length AS MaxLength\r\n                                    FROM sys.indexes a\r\n                                        inner join sys.index_columns b  ON a.object_id = b.object_id AND a.index_id = b.index_id\r\n                                        inner join sys.columns c ON b.object_id = c.object_id AND c.column_id = b.column_id\r\n\t                                    inner join sys.types d ON c.system_type_id=d.system_type_id AND c.user_type_id=d.user_type_id\r\n                                    WHERE a.is_primary_key = 1\r\n                                        and a.object_ID = OBJECT_ID(@Table)", CommandType.Text);
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = dataTable.Clone();
				DataRow dataRow = dataTable2.NewRow();
				dataRow["Name"] = dataTable.Rows[0]["Name"];
				switch (Function.ToString(dataTable.Rows[0]["Type"]))
				{
				case "nvarchar":
					dataRow["Type"] = "A";
					dataRow["MaxLength"] = Function.ParseInt(dataTable.Rows[0]["MaxLength"]);
					break;
				case "int":
					dataRow["Type"] = "N";
					dataRow["MaxLength"] = Function.ParseInt(dataTable.Rows[0]["MaxLength"]);
					break;
				case "ntext":
					dataRow["Type"] = "M";
					dataRow["MaxLength"] = "";
					break;
				case "datetime":
					dataRow["Type"] = "D";
					dataRow["MaxLength"] = "";
					break;
				case "smallint":
					dataRow["Type"] = "N";
					dataRow["MaxLength"] = Function.ParseInt(dataTable.Rows[0]["MaxLength"]);
					break;
				}
				dataTable2.Rows.Add(dataRow);
				return dataTable2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public DataTable GetStructureUKD1()
	{
		try
		{
			return ExecuteDataTable("SELECT TOP 0 * FROM UKD1;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetStructureUFD1()
	{
		try
		{
			return ExecuteDataTable("SELECT TOP 0 * FROM UFD1;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public HttpResult Save(DataSet ds)
	{
		return Process(ApzObjectTypes.oUserFields, -1, Enums.TransactionType.U, ds);
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
					string query = string.Format("select a.TableID, a.FieldID, a.RTable, a.RelUDO, a.RelSO, \r\n                                    (select count(1) from {1} where TableID = @TableId and FieldID = @FieldId) as ValidCount\r\n                                    from {0} a\r\n                                    where a.TableID = @TableId and a.FieldID = @FieldId", TableCufd(text), Tableufd1(text));
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
					StringBuilder stringBuilder = new StringBuilder("DELETE FROM " + TableCufd(text) + " WHERE \"TableID\"=@TableId and \"FieldID\"=@fieldId");
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
				stringBuilder2.Append(" IF NOT EXISTS(SELECT 1 FROM " + TableCufd(text) + " WHERE TableID=@TableId AND AliasID=@AliasID)\r\n                            INSERT INTO " + TableCufd(text) + "(\"TableID\",\"FieldID\",\"AliasID\",\"Descr\",\"TypeID\",\"EditType\",\"SizeID\",\"EditSize\",\"Dflt\",\"NotNull\",\"IndexID\",\"RTable\",\"RField\",\"Action\",\"Sys\",\"DfltDate\",\"RelUDO\",\"ValidRule\",\"RelSO\",\"RThrdPTab\",\"RThrdPFld\")\r\n                                  VALUES(@TableId, @FieldID, @AliasID, @Descr, @TypeID,@EditType,@SizeID,@EditSize\r\n                          , @Dflt, @NotNull, @IndexID, @RTable, @RField, @Action, 'N', GETDATE(), @RelUDO, @ValidRule, @RelSO\r\n                          , @RThrdPTab, @RThrdPFld); ");
				list.Add(new SqlParameter("@TableId", text));
				list.Add(new SqlParameter("@AliasID", inputStr));
				list.Add(new SqlParameter("@FieldID", num2));
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
					StringBuilder stringBuilder3 = new StringBuilder("DELETE FROM " + Tableufd1(text) + " WHERE \"TableID\"=@TableId and \"FieldID\"=@FieldId;" + Environment.NewLine);
					list = new List<SqlParameter>();
					list.Add(new SqlParameter("@TableId", text));
					list.Add(new SqlParameter("@FieldId", num2));
					int num3 = 0;
					int num4 = 0;
					foreach (DataRow row in dataTable2.Rows)
					{
						stringBuilder3.Append(" INSERT INTO " + Tableufd1(text) + "(\"TableID\",\"FieldID\",\"IndexID\",\"FldValue\",\"Descr\",\"FldDate\")\r\n                                      VALUES(@TableId, @FieldID,@IndexID" + num3 + ",@FldValue" + num3 + ",@Descr" + num3 + ",GETDATE()); " + Environment.NewLine);
						list.Add(new SqlParameter("@IndexID" + num3, num4++));
						list.Add(new SqlParameter("@FldValue" + num3, Function.ToString(row["FldValue"])));
						list.Add(new SqlParameter("@Descr" + num3, Function.ToString(row["Descr"])));
						num3++;
					}
					ExecuteNonQuery(stringBuilder3.ToString(), CommandType.Text, list.ToArray());
				}
				else
				{
					StringBuilder stringBuilder4 = new StringBuilder("DELETE FROM " + Tableufd1(text) + " WHERE \"TableID\"=@TableId and \"FieldID\"=@FieldId;");
					list = new List<SqlParameter>();
					list.Add(new SqlParameter("@TableId", text));
					list.Add(new SqlParameter("@FieldId", num2));
					ExecuteNonQuery(stringBuilder4.ToString(), CommandType.Text, list.ToArray());
				}
				if (dataTable3.IsNotNull())
				{
					StringBuilder stringBuilder5 = new StringBuilder("DELETE FROM " + TableOukd(text) + " WHERE TableName=@TableId" + Environment.NewLine);
					list = new List<SqlParameter>();
					list.Add(new SqlParameter("@TableId", text));
					int num5 = 0;
					foreach (DataRow row2 in dataTable3.Rows)
					{
						stringBuilder5.Append(" INSERT INTO " + TableOukd(text) + "(TableName,KeyId,KeyName,UniqueKey,Action)\r\n                                     VALUES(@TableId,@KeyId" + num5 + ",@KeyName" + num5 + ",@UniqueKey" + num5 + ",@Action" + num5 + ") " + Environment.NewLine);
						list.Add(new SqlParameter("@KeyId" + num5, Function.ParseInt(row2["KeyId"])));
						list.Add(new SqlParameter("@KeyName" + num5, Function.ToString(row2["KeyName"])));
						list.Add(new SqlParameter("@UniqueKey" + num5, Function.ToString(row2["UniqueKey"])));
						list.Add(new SqlParameter("@Action" + num5, Function.ToString(row2["Action"])));
						num5++;
					}
					ExecuteNonQuery(stringBuilder5.ToString(), CommandType.Text, list.ToArray());
				}
				else
				{
					list = new List<SqlParameter>();
					StringBuilder stringBuilder6 = new StringBuilder(" DELETE FROM " + TableOukd(text) + " WHERE TableName=@TableId " + Environment.NewLine);
					stringBuilder6.Append(" declare @qry nvarchar(max);\r\n                                select @qry = (\r\n                                select  'IF EXISTS(SELECT * FROM sys.indexes WHERE name='''+ i.name +''' AND object_id = OBJECT_ID(''['+s.name+'].['+o.name+']''))      drop index ['+i.name+'] ON ['+s.name+'].['+o.name+'];  '\r\n                                from sys.indexes i \r\n                                    inner join sys.objects o on  i.object_id=o.object_id\r\n                                    inner join sys.schemas s on o.schema_id = s.schema_id\r\n                                where o.type<>'S' and is_primary_key<>1 and index_id>0\r\n                                and s.name!='sys' and s.name!='sys' and o.name = @TableId and i.name not like @TableFilter and is_unique_constraint=0\r\n                                for xml path(''));\r\n\r\n                                exec sp_executesql @qry" + Environment.NewLine);
					list.Add(new SqlParameter("@TableId", text));
					list.Add(new SqlParameter("@TableFilter", Function.ToString(text + "[_]%")));
					ExecuteNonQuery(stringBuilder6.ToString(), CommandType.Text, list.ToArray());
				}
				if (dataTable4.IsNotNull())
				{
					StringBuilder stringBuilder7 = new StringBuilder();
					list = new List<SqlParameter>();
					list.Add(new SqlParameter("@TableId", text));
					int num6 = 0;
					foreach (var item in (from r in dataTable4.AsEnumerable()
						select new
						{
							KeyId = r["KeyId"]
						}).Distinct().ToList())
					{
						stringBuilder7.Append(" DELETE FROM " + TableUkd1(text) + " WHERE TableName=@TableId AND KeyId=@KeyId" + num6 + Environment.NewLine);
						list.Add(new SqlParameter("@KeyId" + num6, item.KeyId));
						num6++;
					}
					ExecuteNonQuery(stringBuilder7.ToString(), CommandType.Text, list.ToArray());
					stringBuilder7 = new StringBuilder();
					list = new List<SqlParameter>();
					list.Add(new SqlParameter("@TableId", text));
					num6 = 0;
					foreach (DataRow row3 in dataTable4.Rows)
					{
						stringBuilder7.Append(" INSERT INTO " + TableUkd1(text) + "(TableName,KeyId,SubKeyId,ColAlias)\r\n                                     VALUES(@TableId,@KeyId" + num6 + ",@SubKeyId" + num6 + ",@ColAlias" + num6 + ") " + Environment.NewLine);
						list.Add(new SqlParameter("@KeyId" + num6, Function.ParseInt(row3["KeyId"])));
						list.Add(new SqlParameter("@SubKeyId" + num6, Function.ParseInt(row3["SubKeyId"])));
						list.Add(new SqlParameter("@ColAlias" + num6, Function.ParseInt(row3["ColAlias"])));
						num6++;
					}
					ExecuteNonQuery(stringBuilder7.ToString(), CommandType.Text, list.ToArray());
				}
				else
				{
					list = new List<SqlParameter>();
					stringBuilder2 = new StringBuilder(" DELETE FROM " + TableUkd1(text) + " WHERE TableName=@TableId");
					list.Add(new SqlParameter("@TableId", text));
					ExecuteNonQuery(stringBuilder2.ToString(), CommandType.Text, list.ToArray());
				}
				DataTable oUKD = GetOUKD(text);
				if (!oUKD.IsNotNull())
				{
					continue;
				}
				list = new List<SqlParameter>();
				int num7 = 0;
				stringBuilder2 = new StringBuilder();
				foreach (DataRow row4 in oUKD.Rows)
				{
					DataTable ukd = GetUkd1(text, Function.ParseInt(row4["KeyId"]));
					string text2 = "";
					foreach (DataRow row5 in ukd.Rows)
					{
						text2 = text2 + "U_" + Function.ReplaceSQLInjectCharInColumnName(Function.ToString(row5["ColAlias"])) + " ASC,";
					}
					string text3 = Function.ReplaceSQLInjectCharInColumnName(Function.ToString(row4["KeyName"]));
					stringBuilder2.Append(" IF EXISTS (SELECT s.name FROM dbo.sysindexes s, sysobjects o \r\n                                    WHERE s.name = @Keyname" + num7 + " and s.id = o.id and o.name not like 'S%_C%') begin  DROP INDEX " + text + text3 + " ON " + text + " end ");
					stringBuilder2.Append("Create ");
					if (Function.ToString(row4["UniqueKey"]).Equals("Y"))
					{
						stringBuilder2.Append("UNIQUE ");
					}
					stringBuilder2.Append(" index " + text + text3 + " On " + text + "(" + text2.Substring(0, text2.Length - 1) + ") ");
					list.Add(new SqlParameter("@Keyname" + num7, Function.ToString(text + text3)));
					num7++;
				}
				ExecuteNonQuery(stringBuilder2.ToString(), CommandType.Text, list.ToArray());
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected short? GetNewFieldId(string tableID)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetNewFieldIdQuery(tableID), CommandType.Text, new SqlParameter("@TableId", tableID));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseShort(Function.ParseInt(dataTable.Rows[0][0]) + 1);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return (short)(-1);
		}
	}

	protected virtual string GetNewFieldIdQuery(string tableID)
	{
		return "Select max(isnull(fieldid,0)) FROM " + TableCufd(tableID) + " WHERE TableID=@TableId ";
	}

	protected virtual string AddColumn(DataTable dtSet, string tableID, string aliasID, string notNull, string dflt)
	{
		try
		{
			string text = Function.ReplaceSQLInjectCharInColumnName(Function.ToString(dtSet.Rows[0]["Name"]));
			string text2 = Function.ReplaceSQLInjectCharInColumnName(Function.ToString(dtSet.Rows[0]["Type"]));
			string text3 = "";
			if (Function.ToString(dtSet.Rows[0]["Type"]).Equals("nvarchar"))
			{
				text3 = text3 + " ALTER TABLE dbo.[" + tableID + "] ADD " + text + " " + Function.ToString(dtSet.Rows[0]["Type"]) + "(" + Function.ParseInt(dtSet.Rows[0]["MaxLength"]) + ") ";
			}
			else
			{
				text3 = ((!Function.ToString(dtSet.Rows[0]["Type"]).Equals("numeric")) ? (text3 + " ALTER TABLE dbo.[" + tableID + "] ADD " + text + " " + text2) : (text3 + " ALTER TABLE dbo.[" + tableID + "] ADD " + text + " " + text2 + "(19,6)"));
			}
			string result;
			if (notNull == "Y")
			{
				result = (string.IsNullOrEmpty(dflt) ? (text3 + " NOT NULL ") : (text3 + " NOT NULL  CONSTRAINT [DF_" + tableID + "_U_" + aliasID + "]  DEFAULT '" + dflt + "'"));
			}
			else
			{
				result = (string.IsNullOrEmpty(dflt) ? (text3 + " NULL ") : (text3 + " NULL  CONSTRAINT [DF_" + tableID + "_U_" + aliasID + "]  DEFAULT '" + dflt + "'"));
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	protected virtual string UpdateColumn(DataTable dtCheck, DataTable dtSet, string tableID, string aliasID, string notNull, string dflt)
	{
		try
		{
			string text = "";
			string text2 = Function.ReplaceSQLInjectCharInColumnName(Function.ToString(dtSet.Rows[0]["Name"]));
			string text3 = Function.ReplaceSQLInjectCharInColumnName(Function.ToString(dtSet.Rows[0]["Type"]));
			int num = Function.ParseInt(dtCheck.Rows[0]["MaxLength"]);
			if (notNull == "Y")
			{
				text = text + " UPDATE dbo.[" + tableID + "] SET " + text2 + " = '" + dflt + "' WHERE " + text2 + " IS NULL ";
			}
			if (Function.ToString(dtCheck.Rows[0]["Type"]) == Function.ToString(dtSet.Rows[0]["Type"]) && num / 2 <= Function.ParseInt(dtSet.Rows[0]["MaxLength"]) && Function.ToString(dtSet.Rows[0]["Type"]).Equals("nvarchar"))
			{
				text = text + " ALTER TABLE dbo.[" + tableID + "] ALTER COLUMN " + text2 + " " + text3 + "(" + Function.ParseInt(dtSet.Rows[0]["MaxLength"]) + ") ";
			}
			else if (Function.ToString(dtCheck.Rows[0]["Type"]) == Function.ToString(dtSet.Rows[0]["Type"]) && num <= Function.ParseInt(dtSet.Rows[0]["MaxLength"]) && (Function.ToString(dtSet.Rows[0]["Type"]).Equals("varchar") || Function.ToString(dtSet.Rows[0]["Type"]).Equals("char")))
			{
				text = text + " ALTER TABLE dbo.[" + tableID + "] ALTER COLUMN " + text2 + " " + text3 + "(" + Function.ParseInt(dtSet.Rows[0]["MaxLength"]) + ") ";
			}
			else
			{
				text = ((!text3.Equals("numeric")) ? (text + " ALTER TABLE dbo.[" + tableID + "] ALTER COLUMN " + text2 + " " + text3) : (text + " ALTER TABLE dbo.[" + tableID + "] ALTER COLUMN " + text2 + " " + text3 + "(19,6)"));
			}
			text = ((!(notNull == "Y")) ? (text + " NULL ") : (text + " NOT NULL "));
			text += Environment.NewLine;
			if (!string.IsNullOrEmpty(dflt))
			{
				text = text + " if exists (select name from sys.objects where type_desc = 'DEFAULT_CONSTRAINT' and name = 'DF_" + tableID + "_U_" + aliasID + "') ALTER TABLE dbo.[" + tableID + "] DROP CONSTRAINT [DF_" + tableID + "_U_" + aliasID + "]";
				text = text + " ALTER TABLE dbo.[" + tableID + "] ADD CONSTRAINT [DF_" + tableID + "_U_" + aliasID + "]  DEFAULT '" + dflt + "' FOR U_" + aliasID + " ";
			}
			else
			{
				text = text + " if exists (select name from sys.objects where type_desc = 'DEFAULT_CONSTRAINT' and name = 'DF_" + tableID + "_U_" + aliasID + "') ALTER TABLE dbo.[" + tableID + "] DROP CONSTRAINT [DF_" + tableID + "_U_" + aliasID + "]";
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	protected DataTable CheckExistColumn(string table, string column)
	{
		try
		{
			string query = CheckExistColumnQuery();
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@table", table), new SqlParameter("@col", "U_" + column));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	protected virtual string CheckExistColumnQuery()
	{
		return "SELECT TOP 1\r\n                                        a.name AS Name,\r\n                                        b.name AS Type,\r\n\t                                    a.max_length AS MaxLength\r\n                            FROM sys.columns a\r\n                            INNER JOIN sys.types b ON a.system_type_id=b.system_type_id AND a.user_type_id=b.user_type_id\r\n                            WHERE a.object_id=object_id(@table) and a.name = @col ";
	}

	protected DataTable SetTypeCol(DataRow dr)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Type", typeof(string));
			dataTable.Columns.Add("MaxLength", typeof(int));
			dataTable.AcceptChanges();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Name"] = "U_" + Function.ToString(dr["AliasID"]);
			switch (Function.ToString(dr["TypeID"]))
			{
			case "A":
				if (Function.ToString(dr["EditType"]).Equals(""))
				{
					dataRow["Type"] = "nvarchar";
					dataRow["MaxLength"] = dr["EditSize"];
				}
				if (Function.ToString(dr["EditType"]).Equals("?"))
				{
					dataRow["Type"] = "nvarchar";
					dataRow["MaxLength"] = 254;
				}
				if (Function.ToString(dr["EditType"]).Equals("#"))
				{
					dataRow["Type"] = "nvarchar";
					dataRow["MaxLength"] = 20;
				}
				if (Function.ToString(dr["EditType"]).Equals("I"))
				{
					dataRow["Type"] = "nvarchar";
					dataRow["MaxLength"] = 200;
				}
				break;
			case "N":
			{
				int num = Function.ParseInt(dr["EditSize"]);
				if (num <= 6)
				{
					dataRow["Type"] = "smallint";
					dataRow["MaxLength"] = num;
				}
				else
				{
					dataRow["Type"] = "int";
					dataRow["MaxLength"] = num;
				}
				break;
			}
			case "D":
				if (Function.ToString(dr["EditType"]).Equals(""))
				{
					dataRow["Type"] = "datetime";
					dataRow["MaxLength"] = 8;
				}
				if (Function.ToString(dr["EditType"]).Equals("T"))
				{
					dataRow["Type"] = "smallint";
					dataRow["MaxLength"] = 2;
				}
				break;
			case "B":
				dataRow["Type"] = "numeric";
				dataRow["MaxLength"] = 10;
				break;
			case "M":
				if (Function.ToString(dr["EditType"]).Equals(""))
				{
					dataRow["Type"] = "ntext";
					dataRow["MaxLength"] = 16;
				}
				if (Function.ToString(dr["EditType"]).Equals("B"))
				{
					dataRow["Type"] = "ntext";
					dataRow["MaxLength"] = 16;
				}
				if (Function.ToString(dr["EditType"]).Equals("I"))
				{
					dataRow["Type"] = "nvarchar";
					dataRow["MaxLength"] = 200;
				}
				break;
			}
			dataTable.Rows.Add(dataRow);
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	protected DataTable CreateTableName()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Group", typeof(int));
			dataTable.Columns.Add("TableName", typeof(string));
			dataTable.AcceptChanges();
			dataTable.Rows.Add(2, "OCLG");
			dataTable.Rows.Add(3, "OAGP");
			dataTable.Rows.Add(4, "OBIN,ABIN");
			dataTable.Rows.Add(6, "AOAT,OOAT");
			dataTable.Rows.Add(7, "AOA2,OAT2");
			dataTable.Rows.Add(8, "AOA1,OAT1");
			dataTable.Rows.Add(10, "ACRD,OCRD");
			dataTable.Rows.Add(11, "ACR1,CRD1");
			dataTable.Rows.Add(12, "ACPR,OCPR");
			dataTable.Rows.Add(14, "ACPN,OCPN");
			dataTable.Rows.Add(15, "ACP1,CPN1");
			dataTable.Rows.Add(16, "ACP2,CPN2");
			dataTable.Rows.Add(17, "ACP3,CPN3");
			dataTable.Rows.Add(18, "OCCD");
			dataTable.Rows.Add(19, "AEC2,ECM2");
			dataTable.Rows.Add(21, "AHEM,OHEM");
			dataTable.Rows.Add(22, "AHE1,HEM1");
			dataTable.Rows.Add(23, "AHE2,HEM2");
			dataTable.Rows.Add(24, "HEM5");
			dataTable.Rows.Add(25, "AHE3,HEM3");
			dataTable.Rows.Add(26, "AHE6,HEM6");
			dataTable.Rows.Add(27, "AHE4,HEM4");
			dataTable.Rows.Add(28, "AHE7,HEM7");
			dataTable.Rows.Add(29, "AEXT,OEXT");
			dataTable.Rows.Add(30, "AACT,OACT");
			dataTable.Rows.Add(31, "AITB,OITB");
			dataTable.Rows.Add(33, "AITM,OITM");
			dataTable.Rows.Add(34, "AITW,OITW");
			dataTable.Rows.Add(35, "APLN,OPLN");
			dataTable.Rows.Add(36, "APRJ,OPRJ");
			dataTable.Rows.Add(37, "APJT,OPJT");
			dataTable.Rows.Add(38, "ARSB,ORSB");
			dataTable.Rows.Add(40, "ARSC,ORSC");
			dataTable.Rows.Add(41, "ARSC1,RSC1");
			dataTable.Rows.Add(42, "OSLP");
			dataTable.Rows.Add(43, "AUSR,OUSR");
			dataTable.Rows.Add(44, "AWHS,OWHS");
			dataTable.Rows.Add(45, "AWHT,OWHT");
			dataTable.Rows.Add(46, "OWTA");
			dataTable.Rows.Add(47, "AWTT,OWTT");
			dataTable.Rows.Add(49, "ADOC,OCIN,OCPI,OCPV,OCSI,OCSV,ODLN,ODPI,ODPO,ODRF,OIEI,OIGE,OIGN,OINV,OOEI,OPCH,OPDN,OPOR,OPQT,OPRQ,OQUT,ORDN,ORDR,ORIN,ORPC,ORPD,OWTQ,OWTR");
			dataTable.Rows.Add(50, "ADO1,CIN1,CPI1,CPV1,CSI1,CSV1,DLN1,DPI1,DPO1,DRF1,IEI1,IGE1,IGN1,INV1,OEI1,PCH1,PDN1,POR1,PQT1,PRQ1,QUT1,RDN1,RDR1,RIN1,RPC1,RPD1,WTQ1,WTR1");
			dataTable.Rows.Add(51, "ADO3,CIN3,CPI3,CPV3,CSI3,CSV3,DLN3,DPI3,DPO3,DRF3,IEI3,IGE3,IGN3,INV3,OEI3,PCH3,PDN3,POR3,PQT3,PRQ3,QUT3,RDN3,RDR3,RIN3,RPC3,RPD3,WTQ3,WTR3");
			dataTable.Rows.Add(52, "ADO5,AJD2,BTF2,CIN5,CPI5,CPV5,CSI5,CSV5,DLN5,DPI5,DPO5,DRF5,IEI5,IGE5,IGN5,INV5,JDT2,OEI5,PCH5,PDN5,POR5,PQT5,PRQ5,QUT5,RDN5,RDR5,RIN5,RPC5,RPD5,WTQ5,WTR5");
			dataTable.Rows.Add(53, "ADO6,CIN6,CPI6,CPV6,CSI6,CSV6,DLN6,DPI6,DPO6,DRF6,IEI6,IGE6,IGN6,INV6,OEI6,PCH6,PDN6,POR6,PQT6,PRQ6,QUT6,RDN6,RDR6,RIN6,RPC6,RPD6,WTQ6,WTR6");
			dataTable.Rows.Add(54, "ADO7,CIN7,CPI7,CPV7,CSI7,CSV7,DLN7,DPI7,DPO7,DRF7,IEI7,IGE7,IGN7,INV7,OEI7,PCH7,PDN7,POR7,PQT7,PRQ7,QUT7,RDN7,RDR7,RIN7,RPC7,RPD7,WTQ7,WTR7");
			dataTable.Rows.Add(55, "ADO14,CPI14,CPV14,CSI14,CSV14,DLN14,DPI14,DPO14,DRF14,IEI14,IGE14,IGN14,INV14,OEI14,PCH14,PDN14,POR14,PQT14,PRQ14,QUT14,RDN14,RDR14,RIN14,RPC14,RPD14,WTQ14,WTR14");
			dataTable.Rows.Add(57, "ARCT,OPDF,ORCT,OVPM");
			dataTable.Rows.Add(58, "ARC1,PDF1,RCT1,VPM1");
			dataTable.Rows.Add(59, "ARC2,PDF2,RCT2,VPM2");
			dataTable.Rows.Add(60, "ARC3,PDF3,RCT3,VPM3");
			dataTable.Rows.Add(61, "ARC4,PDF4,RCC4,RCT4,VPM4");
			dataTable.Rows.Add(62, "ABOE,OBOE");
			dataTable.Rows.Add(63, "ARC6,PDF6,RCT6,VPM6");
			dataTable.Rows.Add(64, "ARC7,PDF7,RCT7,VPM7");
			dataTable.Rows.Add(65, "OBNK");
			dataTable.Rows.Add(66, "BOE1");
			dataTable.Rows.Add(67, "OPBU");
			dataTable.Rows.Add(68, "ODPS");
			dataTable.Rows.Add(69, "OPBD");
			dataTable.Rows.Add(70, "OPYR");
			dataTable.Rows.Add(71, "OPPT");
			dataTable.Rows.Add(72, "OPRS");
			dataTable.Rows.Add(74, "AITT,OITT,APZ_OITT");
			dataTable.Rows.Add(75, "ATT1,ITT1,APZ_ITT1");
			dataTable.Rows.Add(76, "AWOR,OWOR");
			dataTable.Rows.Add(77, "AWO1,WOR1");
			dataTable.Rows.Add(78, "OWKO,UWKO");
			dataTable.Rows.Add(79, "UWKO1,WKO1");
			dataTable.Rows.Add(80, "OFCT");
			dataTable.Rows.Add(81, "FCT1");
			dataTable.Rows.Add(82, "ORCM");
			dataTable.Rows.Add(83, "AWO3,WOR3");
			dataTable.Rows.Add(85, "AJDT,OBTF,OJDT");
			dataTable.Rows.Add(86, "AJD1,BTF1,JDT1");
			dataTable.Rows.Add(87, "CASE1");
			dataTable.Rows.Add(89, "OACR");
			dataTable.Rows.Add(90, "ODIM");
			dataTable.Rows.Add(91, "APRC,OPRC");
			dataTable.Rows.Add(92, "OCCT");
			dataTable.Rows.Add(93, "AMDR,AOCR,OMDR,OOCR");
			dataTable.Rows.Add(95, "OBGS");
			dataTable.Rows.Add(97, "AOPR,OOPR");
			dataTable.Rows.Add(98, "OPR1");
			dataTable.Rows.Add(99, "OPR2");
			dataTable.Rows.Add(100, "OPR3");
			dataTable.Rows.Add(101, "OPR4");
			dataTable.Rows.Add(102, "OPR5");
			dataTable.Rows.Add(104, "ASCL,OSCL");
			dataTable.Rows.Add(105, "ASC1,SCL1");
			dataTable.Rows.Add(106, "ASC5,SCL5");
			dataTable.Rows.Add(107, "ASC2,SCL2");
			dataTable.Rows.Add(108, "ASC3,SCL3");
			dataTable.Rows.Add(109, "OCTR");
			dataTable.Rows.Add(110, "CTR1");
			dataTable.Rows.Add(111, "OCTT");
			dataTable.Rows.Add(112, "OSLT");
			dataTable.Rows.Add(113, "AINS,OINS");
			dataTable.Rows.Add(114, "ASC6,SCL6");
			dataTable.Rows.Add(115, "ASC7,SCL7");
			dataTable.Rows.Add(117, "APKL,OPKL");
			dataTable.Rows.Add(118, "AKL1,PKL1");
			dataTable.Rows.Add(119, "OSCN");
			dataTable.Rows.Add(120, "AMRV,OMRV");
			dataTable.Rows.Add(121, "AMR1,MRV1");
			dataTable.Rows.Add(122, "AMR2,MRV2");
			dataTable.Rows.Add(123, "AMR3,MRV3");
			dataTable.Rows.Add(124, "AIQI,OIOD,OIQI");
			dataTable.Rows.Add(125, "AQI1,IOD1,IQI1");
			dataTable.Rows.Add(126, "AINC,AIQR,OICD,OINC,OIPD,OIQR");
			dataTable.Rows.Add(127, "AIN1,AQR1,ICD1,INC1,INC1V,IPD1,IQR1");
			dataTable.Rows.Add(128, "AIN2,AQR2,ICD2,INC2,INC2V,IPD2,IQR2");
			dataTable.Rows.Add(131, "ASPP,OSPP");
			dataTable.Rows.Add(132, "ASP1,SPP1");
			dataTable.Rows.Add(133, "ASP2,SPP2");
			dataTable.Rows.Add(134, "ABTN,ASRN,OBTN,ODBN,ODSN,OSRN,UBTN,USRN");
			dataTable.Rows.Add(135, "OAIM");
			dataTable.Rows.Add(136, "ITM10");
			dataTable.Rows.Add(139, "ADTP,ODTP");
			dataTable.Rows.Add(140, "ADPA,ODPA");
			dataTable.Rows.Add(141, "AACS,OACS");
			dataTable.Rows.Add(142, "AFAD,OACD,OACQ,OFTR,OMDP,ORTI");
			dataTable.Rows.Add(143, "ACD1,ACQ1,AFA1,FTR1,MDP1,RTI1");
			dataTable.Rows.Add(144, "OFAR");
			dataTable.Rows.Add(145, "FAR1");
			dataTable.Rows.Add(147, "OWFI");
			dataTable.Rows.Add(148, "AWLS,OWLS");
			dataTable.Rows.Add(150, "APMG,OPMG");
			dataTable.Rows.Add(151, "APH1,APM1,PHA1,PMG1");
			dataTable.Rows.Add(152, "APH2,APM2,PHA2,PMG2");
			dataTable.Rows.Add(153, "APHA,OPHA");
			dataTable.Rows.Add(154, "APZ_OSAL,APZ_AOSAL");
			dataTable.Rows.Add(155, "APZ_OPKL");
			dataTable.Rows.Add(156, "APZ_PKL1");
			dataTable.Rows.Add(157, "APZ_PKL2");
			dataTable.Rows.Add(158, "APZ_OPAK");
			dataTable.Rows.Add(159, "APZ_PAK1");
			dataTable.Rows.Add(160, "APZ_SHIP");
			dataTable.Rows.Add(161, "APZ_SHIP1");
			dataTable.Rows.Add(162, "APZ_OCLG,APZ_ACLG");
			dataTable.Rows.Add(164, "APZ_OPRG,APZ_AOPRG");
			dataTable.Rows.Add(165, "APZ_PRG1,APZ_APRG1");
			dataTable.Rows.Add(166, "APZ_PRG4,APZ_APRG4");
			dataTable.Rows.Add(167, "APZ_TGIE");
			dataTable.Rows.Add(172, "APZ_OFCT");
			dataTable.Rows.Add(173, "APZ_FCT1");
			dataTable.Rows.Add(174, "APZ_HPT1,APZ_AHPT1");
			dataTable.Rows.Add(175, "APZ_ORTC,APZ_OPJC");
			dataTable.Rows.Add(176, "APZ_RTC1,APZ_PJC1");
			dataTable.Rows.Add(177, "APZ_PRG6,APZ_APRG6");
			dataTable.Rows.Add(178, "APZ_OINC");
			dataTable.Rows.Add(179, "APZ_INC1");
			dataTable.Rows.Add(180, "APZ_INC2");
			dataTable.Rows.Add(181, "APZ_OCRQ");
			dataTable.Rows.Add(182, "APZ_OINS");
			dataTable.Rows.Add(183, "APZ_INS1");
			dataTable.Rows.Add(184, "APZ_OSLA");
			dataTable.Rows.Add(185, "APZ_SLA1");
			dataTable.Rows.Add(186, "APZ_SLA2");
			dataTable.Rows.Add(187, "APZ_OSMC");
			dataTable.Rows.Add(188, "APZ_SMC1");
			dataTable.Rows.Add(189, "APZ_SMC2");
			dataTable.Rows.Add(190, "APZ_SMC3");
			dataTable.Rows.Add(191, "APZ_SMC4");
			dataTable.Rows.Add(192, "APZ_SMC5");
			dataTable.Rows.Add(193, "APZ_OHJV");
			dataTable.Rows.Add(194, "APZ_HJV1");
			dataTable.Rows.Add(195, "APZ_HJV2");
			dataTable.Rows.Add(196, "APZ_HJV3");
			dataTable.Rows.Add(197, "APZ_HJV4");
			dataTable.Rows.Add(198, "APZ_HJV5");
			dataTable.Rows.Add(199, "APZ_HJV6");
			dataTable.Rows.Add(200, "APZ_HJV7");
			dataTable.Rows.Add(201, "APZ_HJV8");
			dataTable.Rows.Add(210, "APZ_OHAP");
			dataTable.Rows.Add(211, "APZ_OSCL");
			dataTable.Rows.Add(212, "APZ_OWBS");
			dataTable.Rows.Add(213, "APZ_WBS1");
			dataTable.Rows.Add(214, "APZ_TASK");
			dataTable.Rows.Add(215, "APZ_OTSL,APZ_AOTSL");
			dataTable.Rows.Add(216, "APZ_OHSI,APZ_AOHSI");
			dataTable.Rows.Add(217, "APZ_OKPN");
			dataTable.Rows.Add(218, "APZ_KPN1");
			dataTable.Rows.Add(219, "APZ_OKSE");
			dataTable.Rows.Add(220, "APZ_KSE1");
			dataTable.Rows.Add(221, "APZ_OKIR");
			dataTable.Rows.Add(222, "APZ_KIR1");
			dataTable.Rows.Add(223, "APZ_OKGP");
			dataTable.Rows.Add(224, "APZ_KGP1");
			dataTable.Rows.Add(225, "APZ_KGP2");
			dataTable.Rows.Add(226, "APZ_OKSP");
			dataTable.Rows.Add(227, "APZ_KSP1");
			dataTable.Rows.Add(228, "APZ_OKSC");
			dataTable.Rows.Add(229, "APZ_KSC1");
			dataTable.Rows.Add(230, "APZ_OPCN");
			dataTable.Rows.Add(231, "APZ_PCN1");
			dataTable.Rows.Add(231, "APZ_OPBF");
			dataTable.Rows.Add(233, "APZ_PBF1");
			dataTable.Rows.Add(234, "APZ_OPPR");
			dataTable.Rows.Add(235, "APZ_OHWT");
			dataTable.Rows.Add(236, "APZ_OHWC");
			dataTable.Rows.Add(237, "APZ_OHWP");
			dataTable.Rows.Add(238, "APZ_OHOT");
			dataTable.Rows.Add(239, "APZ_OHLD");
			dataTable.Rows.Add(240, "APZ_HLD1");
			dataTable.Rows.Add(241, "APZ_OIOM");
			dataTable.Rows.Add(242, "APZ_OWSC");
			dataTable.Rows.Add(243, "APZ_WSC1");
			dataTable.Rows.Add(244, "APZ_WSC2");
			dataTable.Rows.Add(245, "APZ_WSC3");
			dataTable.Rows.Add(246, "APZ_OWSR");
			dataTable.Rows.Add(247, "APZ_OHAT");
			dataTable.Rows.Add(248, "APZ_OHDT");
			dataTable.Rows.Add(249, "APZ_HDT1");
			dataTable.Rows.Add(250, "APZ_OPIT");
			dataTable.Rows.Add(251, "APZ_PIT1");
			dataTable.Rows.Add(252, "APZ_OHSP");
			dataTable.Rows.Add(253, "APZ_OSLP");
			dataTable.Rows.Add(254, "APZ_OHPT");
			dataTable.Rows.Add(255, "APZ_OIOD");
			dataTable.Rows.Add(256, "APZ_IOD1");
			dataTable.Rows.Add(257, "APZ_IOD2");
			dataTable.Rows.Add(258, "APZ_OHER");
			dataTable.Rows.Add(259, "APZ_OHEH");
			dataTable.Rows.Add(260, "APZ_OHEC");
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public DataTable CheckEnumConnect(string dbName)
	{
		try
		{
			return ExecuteDataTable("SELECT  spid,\r\n                                    sp.[status],\r\n                                    loginame [Login],\r\n                                    hostname, \r\n                                    blocked BlkBy,\r\n                                    sd.name DBName, \r\n                                    cmd Command,\r\n                                    cpu CPUTime,\r\n                                    physical_io DiskIO,\r\n                                    last_batch LastBatch,\r\n                                    [program_name] ProgramName   \r\n                            FROM master.dbo.sysprocesses sp \r\n                            JOIN master.dbo.sysdatabases sd ON sp.dbid = sd.dbid\r\n                            where sd.name=@DBName\r\n                            ORDER BY spid ", CommandType.Text, new SqlParameter("@DBName", dbName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public HttpResult UpdateDataUdf(UserFieldsImpl udfField, QueryParamater keyCondition, string tableName)
	{
		try
		{
			if (udfField == null || string.IsNullOrEmpty(udfField.Name) || !udfField.Name.StartsWith("U_") || keyCondition == null || string.IsNullOrEmpty(keyCondition.ParaName) || string.IsNullOrEmpty(tableName))
			{
				return new HttpResult(Enums.MessageCode.Error, "Data update does not enough");
			}
			string query = string.Format("IF EXISTS(SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS  WHERE  TABLE_NAME = '{0}'  AND COLUMN_NAME = '{1}') AND \r\n                                             EXISTS(SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS  WHERE  TABLE_NAME = '{0}'  AND COLUMN_NAME = '{2}')\r\n                            BEGIN \r\n\t                            Update {0} set {1} = @valueUpdate where {2} = @valueCondition\r\n                            END", tableName, udfField.Name, keyCondition.ParaName);
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@valueUpdate", udfField.Value ?? ""), new SqlParameter("@valueCondition", keyCondition.Value ?? ""));
				dbTransaction.Commit();
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public List<string> GetListFieldNotValidated(string tableID)
	{
		try
		{
			return (from r in ExecuteDataTable(GetListFieldNotValidatedQuery(), CommandType.Text, new SqlParameter("@tableID", tableID)).AsEnumerable()
				select Function.ToString(r[0])).ToList();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	protected virtual string GetListFieldNotValidatedQuery()
	{
		return "select 'U_' + AliasID from CUFD a where TableID = @tableID and isnull(RTable,'')='' and isnull(RelUDO,'')='' and isnull(RelSO,'')=''\r\nand not exists(select 1 from UFD1 where TableID=a.TableID and FieldID=a.FieldID)";
	}
}
