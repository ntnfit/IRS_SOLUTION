using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class UDTRepository : BaseService<UDFsImpl>, IUDTs
{
	public UDTRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetAllUDT()
	{
		try
		{
			return ExecuteDataTable("SELECT * from tblUTable", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetUDT(string tableName)
	{
		try
		{
			return ExecuteDataTable("SELECT * from tblUTable where tablename=@table", CommandType.Text, new SqlParameter("@table", tableName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public string DeleteDoc(string tableName)
	{
		try
		{
			if (ExecuteNonQuery("delete tblUTable where TableName = @table", CommandType.Text, new SqlParameter("@table", tableName)) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public string SaveDoc(UDTsImpl doc)
	{
		try
		{
			string query = (checkDoc(doc.TableName) ? "UPDATE [dbo].[tblUTable]\r\n                                           SET  Description= @Description,TypeObject = @TypeObject  where TableName= @TableName)" : "INSERT INTO [dbo].[tblUTable]\r\n                                           ([TableName]\r\n                                           ,[Description]\r\n                                           ,[TypeObject])\r\n                                     VALUES\r\n                                           (@TableName\r\n                                           ,@Description\r\n                                           ,@TypeObject)");
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@TableName", doc.TableName), new SqlParameter("@Description", doc.Description), new SqlParameter("@TypeObject", doc.TypeObject)) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	private bool checkDoc(string table)
	{
		try
		{
			string query = "select tablename from tblUTable where tablename= @table";
			if (ExecuteDataTable(query, CommandType.Text, new SqlParameter("@table", table)).Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch
		{
			return true;
		}
	}

	protected virtual string GetTableStructureQuery()
	{
		return "SELECT COLUMN_NAME,DATA_TYPE,COLUMN_DEFAULT,IS_NULLABLE\r\n                FROM INFORMATION_SCHEMA.COLUMNS\r\n                WHERE TABLE_NAME = @table";
	}

	public DataTable GetStructureTable(string tableName)
	{
		try
		{
			string tableStructureQuery = GetTableStructureQuery();
			DataTable dataTable = ExecuteDataTable(tableStructureQuery, CommandType.Text, new SqlParameter("@table", tableName));
			if (dataTable.IsNotNull())
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public bool Exists(string code)
	{
		try
		{
			if (ExecuteDataTable(code, CommandType.Text).IsNotNull())
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	private List<SqlParameter> MappingSqlParameter(DataRow dataRow, DataTable dtStr)
	{
		List<SqlParameter> list = new List<SqlParameter>();
		for (int i = 0; i < dtStr.Rows.Count; i++)
		{
			string text = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
			string text2 = Function.ToString(dtStr.Rows[i]["DATA_TYPE"]);
			if (dataRow.IsExistsColumn(text))
			{
				switch (text2.ToUpper())
				{
				case "NVARCHAR":
					list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
					break;
				case "DATETIME":
					list.Add((!Function.ParseDateTime(dataRow[text]).HasValue) ? new SqlParameter("@" + text, "") : new SqlParameter("@" + text, Function.ParseDateTime(dataRow[text])));
					break;
				case "CHAR":
					list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
					break;
				case "NTEXT":
					list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
					break;
				case "SMALLINT":
					list.Add(new SqlParameter("@" + text, Function.ParseInt(dataRow[text])));
					break;
				case "INT":
					list.Add(new SqlParameter("@" + text, Function.ParseInt(dataRow[text])));
					break;
				case "BIT":
					list.Add(new SqlParameter("@" + text, Function.ParseBool(dataRow[text])));
					break;
				default:
					list.Add(new SqlParameter("@" + text, Function.ParseDouble(dataRow[text])));
					break;
				}
			}
		}
		return list;
	}

	private string UpdateData(DataRow dataRow, DataTable dtStr)
	{
		try
		{
			string text = "UPDATE OUTB SET ";
			for (int i = 0; i < dtStr.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
				if (text2 == "Descr")
				{
					text += string.Format("\"{0}\" = @{0},", text2);
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += " where \"TableName\" = @TableName;";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStr);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "Save Error!";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected virtual string GenerateTableQuery(string tableName, int objType)
	{
		try
		{
			string text = string.Empty;
			switch (objType)
			{
			case 0:
				text = text + " CREATE TABLE " + tableName + " ( [Code] NVARCHAR(50) NOT NULL, [Name] NVARCHAR(100) NOT NULL)";
				break;
			case 1:
				text = text + "CREATE TABLE " + tableName + " ([Code] [NVARCHAR](50) NOT NULL,[Name] [NVARCHAR](100) NULL,[DocEntry] [INT] NOT NULL,[Canceled] [CHAR](1) NULL,[Object] [NVARCHAR](20) NULL,[LogInst] [INT] NULL,[UserSign] [INT] NULL,[Transfered] [CHAR](1) NULL,[CreateDate] [DATETIME] NULL,[CreateTime] [SMALLINT] NULL,[UpdateDate] [DATETIME] NULL,[UpdateTime] [SMALLINT] NULL,[DataSource] [CHAR](1) NULL)";
				break;
			case 2:
				text = text + "CREATE TABLE " + tableName + " ([Code] [NVARCHAR](50) NOT NULL,[LineId] [INT] NOT NULL,[Object] [NVARCHAR](20) NULL,[LogInst] [INT] NULL)";
				break;
			case 3:
				text = text + "CREATE TABLE " + tableName + " ([DocEntry] [INT] NOT NULL,[DocNum] [INT] NULL,[Period] [INT] NULL,[Instance] [SMALLINT] NULL,[Series] [INT] NULL,[Handwrtten] [CHAR](1) NULL,[Canceled] [CHAR](1) NULL,[Object] [NVARCHAR](20) NULL,[LogInst] [INT] NULL,[UserSign] [INT] NULL,[Transfered] [CHAR](1) NULL,[Status] [CHAR](1) NULL,[CreateDate] [DATETIME] NULL,[CreateTime] [SMALLINT] NULL,[UpdateDate] [DATETIME] NULL,[UpdateTime] [SMALLINT] NULL,[DataSource] [CHAR](1) NULL,[RequestStatus] [CHAR](1) NULL,[Creator] [NVARCHAR](8) NULL,[Remark] [NTEXT] NULL)";
				break;
			case 4:
				text = text + "CREATE TABLE " + tableName + "([DocEntry] [INT] NOT NULL,[LineId] [INT] NOT NULL,[VisOrder] [INT] NULL,[Object] [NVARCHAR](20) NULL,[LogInst] [INT] NULL)";
				break;
			case 5:
				text = text + " CREATE TABLE " + tableName + " ( Code [int] IDENTITY(1,1) NOT NULL,Name NVARCHAR(100) NULL)";
				break;
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	private string InsertData(DataRow dataRow, DataTable dtStr)
	{
		try
		{
			string text = "";
			string tableUdt = GetTableUdt(Function.ToString(dataRow["TableName"]));
			text = GenerateTableQuery(tableUdt, Function.ParseInt(dataRow["ObjectType"]));
			ExecuteNonQuery(text, CommandType.Text, (SqlParameter[])null);
			string text2 = " INSERT INTO OUTB VALUES (";
			for (int i = 0; i < dtStr.Rows.Count; i++)
			{
				string text3 = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
				text2 = text2 + "@" + text3 + ",";
			}
			text2 = text2.Substring(0, text2.Length - 1);
			text2 += ");";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStr);
			if (ExecuteNonQuery(text2, CommandType.Text, list.ToArray()) <= 0)
			{
				return "Save Error!";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected virtual string GetTableOUTBQuery()
	{
		return "declare @a nvarchar(max)\r\n                     set @a = (SELECT CONVERT(NVARCHAR,TableName)+',' FROM OUTB  FOR XML PATH(''))\r\n                     select @a";
	}

	public string UpdateUdt(DataTable dtDoc)
	{
		try
		{
			if (dtDoc.IsNotNull())
			{
				DataTable structureTable = GetStructureTable("OUTB");
				using (DatabaseService.OpenConnection())
				{
					using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
					for (int i = 0; i < dtDoc.Rows.Count; i++)
					{
						string text = Function.ToString(dtDoc.Rows[i]["TableName"]);
						string code = "select \"TableName\" from OUTB  where \"TableName\" = '" + text + "';";
						if (Exists(code))
						{
							string text2 = UpdateData(dtDoc.Rows[i], structureTable);
							if (!string.IsNullOrEmpty(text2))
							{
								return text2;
							}
						}
						else
						{
							string text3 = InsertData(dtDoc.Rows[i], structureTable);
							if (!string.IsNullOrEmpty(text3))
							{
								return text3;
							}
						}
					}
					string tableOUTBQuery = GetTableOUTBQuery();
					DataTable dataTable = ExecuteDataTable(tableOUTBQuery, CommandType.Text);
					if (dataTable.IsNotNull())
					{
						string text4 = Function.ToString(dataTable.Rows[0][0]);
						if (!string.IsNullOrEmpty(text4))
						{
							List<string> list = (from t in dtDoc.AsEnumerable()
								select Function.ToString(t["TableName"])).ToList();
							foreach (string item in text4.Split(',').ToList())
							{
								if (!list.Contains(item) && !string.IsNullOrEmpty(item))
								{
									string text5 = DeleteData(item);
									if (!string.IsNullOrEmpty(text5))
									{
										return text5;
									}
								}
							}
						}
					}
					dbTransaction.Commit();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	private string DeleteData(string tableName)
	{
		try
		{
			string query = " DELETE FROM OUTB WHERE \"TableName\" = @TableName; \r\n                                DROP TABLE " + GetTableUdt(tableName) + ";DELETE FROM CUFD where \"TableID\" = @TableName;DELETE FROM UFD1 where \"TableID\" = @TableName;";
			SqlParameter sqlParameter = new SqlParameter("@TableName", tableName);
			if (ExecuteNonQuery(query, CommandType.Text, sqlParameter) <= 0)
			{
				return "Save Error!";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public List<UDTsImpl> LoadBarUdt()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT ISNULL(TableName,'') AS TableName ,\r\n                            ISNULL(Descr,'') AS  Descr FROM dbo.OUTB WHERE ObjectType=0 \r\n                            ORDER BY TableName", CommandType.Text, (SqlParameter[])null);
			List<UDTsImpl> list = new List<UDTsImpl>();
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					UDTsImpl item = new UDTsImpl
					{
						TableName = Function.ToString(dataTable.Rows[i]["TableName"]),
						Description = Function.ToString(dataTable.Rows[i]["Descr"])
					};
					list.Add(item);
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new List<UDTsImpl>();
	}

	public DataTable GetListUDT(string userName, string branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@UserName", userName));
			}
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@Branch", branch));
			}
			return ExecuteDataTable("Apz_UDT_GetListUDT", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public DataTable LoadDoc(string tableName)
	{
		try
		{
			DataTable structureTableUdt = GetStructureTableUdt(tableName);
			string sql = GetSql(structureTableUdt, tableName);
			return ExecuteDataTable(sql, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetSql(DataTable dt, string tableName)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			string empty = string.Empty;
			stringBuilder.Append("SELECT ");
			for (int i = 0; i < Function.ParseInt(dt.Rows.Count); i++)
			{
				switch (Function.ToString(dt.Rows[i]["DATA_TYPE"]).ToUpper())
				{
				case "NVARCHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						stringBuilder.Append(Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",");
					}
					else
					{
						empty = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						stringBuilder.Append(Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + empty + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",");
					}
					break;
				case "NTEXT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						stringBuilder.Append(Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",");
					}
					else
					{
						empty = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						stringBuilder.Append(Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + empty + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",");
					}
					break;
				case "DATETIME":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						stringBuilder.Append(Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",GETDATE()) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",");
					}
					else
					{
						empty = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						stringBuilder.Append(Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + empty + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",");
					}
					break;
				case "CHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						stringBuilder.Append(Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",");
					}
					else
					{
						empty = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						stringBuilder.Append(Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + empty + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",");
					}
					break;
				case "BIT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						stringBuilder.Append(Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",");
					}
					else
					{
						empty = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						stringBuilder.Append(Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + empty + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",");
					}
					break;
				default:
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						stringBuilder.Append(Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",");
					}
					else
					{
						empty = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(2, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 4);
						stringBuilder.Append(Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + empty + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",");
					}
					break;
				}
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append(Environment.NewLine + "FROM dbo.[@" + tableName + "]");
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "Function get SQL ERROR ";
		}
	}

	public DataTable AddNEW(string tableName)
	{
		try
		{
			DataTable structureTableUdt = GetStructureTableUdt(tableName);
			DataTable tableAddNew = GetTableAddNew(structureTableUdt);
			DataRow dataRow = tableAddNew.NewRow();
			for (int i = 0; i < structureTableUdt.Rows.Count; i++)
			{
				string text = Function.ToString(structureTableUdt.Rows[i]["COLUMN_NAME"]);
				switch (Function.ToString(structureTableUdt.Rows[i]["DATA_TYPE"]).ToUpper())
				{
				case "NVARCHAR":
					dataRow[text] = "";
					continue;
				case "DATETIME":
					dataRow[text] = DateTime.Today;
					continue;
				case "NTEXT":
					dataRow[text] = "";
					continue;
				case "CHAR":
					dataRow[text] = "N";
					continue;
				case "BIT":
					dataRow[text] = 0;
					continue;
				}
				if (text == "Code")
				{
					dataRow[text] = 1;
				}
				else
				{
					dataRow[text] = 0;
				}
			}
			tableAddNew.Rows.Add(dataRow);
			tableAddNew.AcceptChanges();
			return tableAddNew;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE bom  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetTableAddNew(DataTable dt)
	{
		try
		{
			DataTable dataTable = new DataTable();
			for (int i = 0; i < Function.ParseInt(dt.Rows.Count); i++)
			{
				string columnName = Function.ToString(dt.Rows[i]["COLUMN_NAME"]);
				switch (Function.ToString(dt.Rows[i]["DATA_TYPE"]).ToUpper())
				{
				case "NVARCHAR":
					dataTable.Columns.Add(columnName, typeof(string));
					break;
				case "DATETIME":
					dataTable.Columns.Add(columnName, typeof(DateTime));
					break;
				case "CHAR":
					dataTable.Columns.Add(columnName, typeof(string));
					break;
				case "NTEXT":
					dataTable.Columns.Add(columnName, typeof(string));
					break;
				case "BIT":
					dataTable.Columns.Add(columnName, typeof(string));
					break;
				case "INT":
					dataTable.Columns.Add(columnName, typeof(int));
					break;
				default:
					dataTable.Columns.Add(columnName, typeof(double));
					break;
				}
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string Save(Tuple<DataTable, string> lstTuple)
	{
		try
		{
			DataTable item = lstTuple.Item1;
			string item2 = lstTuple.Item2;
			if (item != null)
			{
				if (item.Columns.Contains("IsNewRow"))
				{
					item.Columns.Remove("IsNewRow");
				}
				DataTable structureTableUdt = GetStructureTableUdt(item2);
				string queryText = GetQueryText(structureTableUdt, item2, "#TempData");
				using (DatabaseService.OpenConnection())
				{
					using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
					CreateTempTable(item, "#TempData");
					BulkCopy(item, "#TempData");
					ExecuteNonQuery(queryText, CommandType.Text);
					dbTransaction.Commit();
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string DeleteDataAllTable(string tableName)
	{
		try
		{
			string query = "DELETE dbo.[@" + tableName + "]";
			SqlParameter sqlParameter = new SqlParameter("@table", tableName);
			if (ExecuteNonQuery(query, CommandType.Text, sqlParameter) <= 0)
			{
				return "Save Error!";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string UpdateTableUdt(DataRow dataRow, DataTable dtStr, string tableName)
	{
		try
		{
			string text = "UPDATE dbo.[@" + tableName + "] SET ";
			for (int i = 0; i < dtStr.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
				if (text2 != "Code")
				{
					text = text + Environment.NewLine + text2 + "=@" + text2 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += " where Code= @Code";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStr);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "Save Error!";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected virtual string GetQueryText(DataTable dtStr, string tableName, string tempName)
	{
		try
		{
			string text = $"delete from dbo.[@{tableName}] ";
			if (dtStr.AsEnumerable().Any((DataRow t) => Function.ToString(t["COLUMN_NAME"]).Equals("Code") && Function.ToString(t["DATA_TYPE"]).ToUpper().Equals("INT")))
			{
				text = $"delete a from dbo.[@{tableName}] a where not exists(select 1 from {tempName} where Code = a.Code) ";
				text += " UPDATE b SET ";
				for (int num = 0; num < dtStr.Rows.Count; num++)
				{
					string text2 = Function.ToString(dtStr.Rows[num]["COLUMN_NAME"]);
					if (text2 != "Code")
					{
						text = text + Environment.NewLine + "b." + text2 + "=a." + text2 + ",";
					}
				}
				text = text.Substring(0, text.Length - 1);
				text = text + " from " + tempName + " a INNER JOIN dbo.[@" + tableName + "] b ON a.Code = b.Code ";
				text += string.Format("Delete a from {1} a INNER JOIN dbo.[@{0}] b ON a.Code = b.Code ", tableName, tempName);
			}
			string text3 = "";
			string text4 = "";
			for (int num2 = 0; num2 < dtStr.Rows.Count; num2++)
			{
				string text5 = Function.ToString(dtStr.Rows[num2]["COLUMN_NAME"]);
				if (!(text5 == "Code") || !Function.ToString(dtStr.Rows[num2]["DATA_TYPE"]).ToUpper().Equals("INT"))
				{
					text3 += $"{text5},";
					text4 += $"nullif({text5}, ''),";
				}
			}
			text3 = text3.Substring(0, text3.Length - 1);
			text4 = text4.Substring(0, text4.Length - 1);
			return text + $" INSERT INTO dbo.[@{tableName}] ({text3}) select {text4} from {tempName}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected virtual string GetTableUdt(string tableName)
	{
		return $"dbo.[@{tableName}]";
	}

	private string InsertDataTableUdt(DataRow dataRow, DataTable dtStr, string table)
	{
		try
		{
			string text = "dbo.[@" + table + "] ";
			string text2 = " INSERT INTO " + text + " VALUES (";
			for (int i = 0; i < dtStr.Rows.Count; i++)
			{
				string text3 = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
				if (!(text3 == "Code") || !(Function.ToString(dtStr.Rows[i]["DATA_TYPE"]).ToUpper() == "INT"))
				{
					text2 = text2 + "@" + text3 + ",";
				}
			}
			text2 = text2.Substring(0, text2.Length - 1);
			text2 += ")";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStr);
			if (ExecuteNonQuery(text2, CommandType.Text, list.ToArray()) <= 0)
			{
				return "Save Error!";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string DeleteDataTableUdt(string tableName, string code)
	{
		try
		{
			string query = "DELETE dbo.[@" + tableName + "] WHERE Code=@Code";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", code));
			if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) <= 0)
			{
				return "Save Error!";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetStructureTableUdt(string tableName)
	{
		try
		{
			string structureTableUdtQuery = GetStructureTableUdtQuery();
			DataTable dataTable = ExecuteDataTable(structureTableUdtQuery, CommandType.Text, new SqlParameter("@table", tableName));
			if (dataTable.IsNotNull())
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetStructureTableUdtQuery()
	{
		return "SELECT COLUMN_NAME,DATA_TYPE,COLUMN_DEFAULT,IS_NULLABLE\r\n                    FROM INFORMATION_SCHEMA.COLUMNS \r\n                    WHERE TABLE_NAME = '@' + @table";
	}
}
