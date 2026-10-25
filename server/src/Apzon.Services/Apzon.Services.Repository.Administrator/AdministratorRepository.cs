using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class AdministratorRepository : BaseService<IAdministrator>, IAdministrator
{
	public AdministratorRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable LoadDocUOM()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT \r\n\t                                        ISNULL(ItmsTypCod,0) AS ItmsTypCod,\r\n\t                                        ISNULL(ItmsGrpNam,'') AS ItmsGrpNam,\r\n\t                                        ISNULL(UserSign,1) AS UserSign\t\r\n\t                                 FROM dbo.oitg ", CommandType.Text);
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

	public string SaveUOM(DataTable dtDoc)
	{
		try
		{
			if (dtDoc.IsNotNull())
			{
				using (DatabaseService.OpenConnection())
				{
					using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
					for (int i = 0; i < dtDoc.Rows.Count; i++)
					{
						string code = Function.ToString(dtDoc.Rows[i]["GroupCode"]);
						if (ExistsUOM(code))
						{
							string text = UpdateDataUOM(dtDoc.Rows[i]);
							if (!string.IsNullOrEmpty(text))
							{
								return text;
							}
						}
						else
						{
							string text2 = InsertDataUOM(dtDoc.Rows[i]);
							if (!string.IsNullOrEmpty(text2))
							{
								return text2;
							}
						}
					}
					string query = "SELECT CONVERT(NVARCHAR,GroupCode)+',' FROM OCRG where GroupType='S' FOR XML PATH('')";
					DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
					if (dataTable.IsNotNull())
					{
						string text3 = Function.ToString(dataTable.Rows[0][0]);
						if (!string.IsNullOrEmpty(text3))
						{
							List<string> list = (from t in dtDoc.AsEnumerable()
								select Function.ToString(t["GroupCode"])).ToList();
							List<string> list2 = text3.Split(',').ToList();
							for (int num = 0; num < list2.Count; num++)
							{
								if (!list.Contains(list2[num]) && !string.IsNullOrEmpty(list2[num]))
								{
									string text4 = DeleteDataUOM(list2[num]);
									if (!string.IsNullOrEmpty(text4))
									{
										return text4;
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

	private string DeleteDataUOM(string groupcode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@GroupCode", groupcode);
			if (ExecuteNonQuery("DELETE OCRG WHERE GroupCode=@GroupCode", CommandType.Text, sqlParameter) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string InsertDataUOM(DataRow dataRow)
	{
		try
		{
			List<SqlParameter> list = MappingSqlParameterUOM(dataRow);
			if (ExecuteNonQuery("INSERT INTO OCRG\r\n                                        ( GroupCode ,GroupName ,GroupType ,Locked , DataSource , UserSign ,PriceList , DiscRel)\r\n                                VALUES  (@GroupCode ,@GroupName ,@GroupType ,@Locked , @DataSource , @UserSign ,@PriceList , @DiscRel)", CommandType.Text, list.ToArray()) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private List<SqlParameter> MappingSqlParameterUOM(DataRow dataRow)
	{
		List<SqlParameter> list = new List<SqlParameter>();
		if (dataRow.IsExistsColumn("GroupCode"))
		{
			list.Add(new SqlParameter("@GroupCode", Function.ParseInt(dataRow["GroupCode"])));
		}
		else
		{
			list.Add(new SqlParameter("@GroupCode", ""));
		}
		if (dataRow.IsExistsColumn("GroupName"))
		{
			list.Add(new SqlParameter("@GroupName", Function.ToString(dataRow["GroupName"])));
		}
		else
		{
			list.Add(new SqlParameter("@GroupName", ""));
		}
		if (dataRow.IsExistsColumn("GroupType"))
		{
			list.Add(new SqlParameter("@GroupType", Function.ToString(dataRow["GroupType"])));
		}
		else
		{
			list.Add(new SqlParameter("@GroupType", ""));
		}
		if (dataRow.IsExistsColumn("Locked"))
		{
			list.Add(new SqlParameter("@Locked", Function.ToString(dataRow["Locked"])));
		}
		else
		{
			list.Add(new SqlParameter("@Locked", ""));
		}
		if (dataRow.IsExistsColumn("DataSource"))
		{
			list.Add(new SqlParameter("@DataSource", Function.ToString(dataRow["DataSource"])));
		}
		else
		{
			list.Add(new SqlParameter("@DataSource", ""));
		}
		if (dataRow.IsExistsColumn("UserSign"))
		{
			list.Add(new SqlParameter("@UserSign", Function.ParseInt(dataRow["UserSign"])));
		}
		else
		{
			list.Add(new SqlParameter("@UserSign", ""));
		}
		if (dataRow.IsExistsColumn("PriceList"))
		{
			list.Add(new SqlParameter("@PriceList", Function.ParseInt(dataRow["PriceList"])));
		}
		else
		{
			list.Add(new SqlParameter("@PriceList", ""));
		}
		if (dataRow.IsExistsColumn("DiscRel"))
		{
			list.Add(new SqlParameter("@DiscRel", Function.ToString(dataRow["DiscRel"])));
		}
		else
		{
			list.Add(new SqlParameter("@DiscRel", ""));
		}
		return list;
	}

	private string UpdateDataUOM(DataRow dataRow)
	{
		try
		{
			List<SqlParameter> list = MappingSqlParameterUOM(dataRow);
			if (ExecuteNonQuery("UPDATE [OCRG]\r\n                                       SET [GroupName] = @GroupName\r\n                                          ,[GroupType] = @GroupType\r\n                                          ,[Locked] = @Locked\r\n                                          ,[DataSource] = @DataSource\r\n                                          ,[UserSign] = @UserSign\r\n                                          ,[PriceList] = @PriceList\r\n                                          ,[DiscRel] = @DiscRel\r\n                                     WHERE GroupCode=@GroupCode", CommandType.Text, list.ToArray()) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public bool ExistsUOM(string code)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@GroupCode", code);
			DataTable dataTable = ExecuteDataTable("select GroupCode from OCRG where GroupCode=@GroupCode", CommandType.Text, sqlParameter);
			if (dataTable.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["GroupCode"])))
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

	public DataTable LoadDoc(string tableName)
	{
		try
		{
			DataTable structureTable = GetStructureTable(tableName);
			string sql = GetSql(structureTable, tableName);
			DataTable dataTable = ExecuteDataTable(sql, CommandType.Text);
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

	public DataTable GetStructureTable(string tableName)
	{
		try
		{
			string query = "SELECT COLUMN_NAME,DATA_TYPE,COLUMN_DEFAULT,(SELECT COUNT(*)\r\n                FROM INFORMATION_SCHEMA.COLUMNS\r\n                WHERE TABLE_NAME ='OUOM') AS COLUMNCOUT\r\n                FROM INFORMATION_SCHEMA.COLUMNS\r\n                WHERE TABLE_NAME = '" + tableName + "'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
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

	public string GetSql(DataTable dt, string tableName)
	{
		try
		{
			string text = "SELECT ";
			for (int i = 0; i < Function.ParseInt(dt.Rows[0]["COLUMNCOUT"]); i++)
			{
				switch (Function.ToString(dt.Rows[i]["DATA_TYPE"]).ToUpper())
				{
				case "NVARCHAR":
					if (Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).ToUpper() == "NULL")
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text4 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text4 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "DATETIME":
					if (Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).ToUpper() == "NULL")
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text5 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text5 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "CHAR":
					if (Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).ToUpper() == "NULL")
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text3 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text3 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "BIT":
					if (Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).ToUpper() == "NULL")
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text6 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text6 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				default:
					if (Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).ToUpper() == "NULL")
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text2 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(2, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text2 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				}
			}
			text = text.Substring(0, text.Length - 1);
			return text + Environment.NewLine + "FROM " + tableName;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "Function get SQL ERROR ";
		}
	}

	public DataTable GetAddNewRow(string tableName)
	{
		try
		{
			DataTable structureTable = GetStructureTable(tableName);
			string sqlAddNew = GetSqlAddNew(structureTable, tableName);
			DataTable dataTable = ExecuteDataTable(sqlAddNew, CommandType.Text);
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

	public string GetSqlAddNew(DataTable dt, string tableName)
	{
		try
		{
			string text = "SELECT TOP 1";
			for (int i = 0; i < Function.ParseInt(dt.Rows[0]["COLUMNCOUT"]); i++)
			{
				switch (Function.ToString(dt.Rows[i]["DATA_TYPE"]).ToUpper())
				{
				case "NVARCHAR":
					if (Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).ToUpper() == "NULL")
					{
						text = text + Environment.NewLine + "'' AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text4 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text4 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "DATETIME":
					if (Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).ToUpper() == "NULL")
					{
						text = text + Environment.NewLine + "'' AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text3 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text3 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "CHAR":
					if (Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).ToUpper() == "NULL")
					{
						text = text + Environment.NewLine + "'' AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text5 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text5 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "BIT":
					if (Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).ToUpper() == "NULL")
					{
						text = text + Environment.NewLine + "0 AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text6 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text6 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				default:
					if (Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).ToUpper() == "NULL")
					{
						text = text + Environment.NewLine + "0 AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text2 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text2 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				}
			}
			text = text.Substring(0, text.Length - 1);
			return text + Environment.NewLine + "FROM " + tableName;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "Function get SQL ERROR ";
		}
	}

	public DataTable GetCodeOcyc()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select Code,Name from OCYC", CommandType.Text);
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

	public DataTable GetListCountry()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT Code,Name FROM dbo.OCRY", CommandType.Text);
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

	public DataTable GetListState(string country)
	{
		try
		{
			string query = "SELECT Code,Name FROM dbo.OCST where Country=@Country";
			SqlParameter sqlParameter = new SqlParameter("@Country", country);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
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

	public DataTable GetCodeOcycItemGroups()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select \"Code\",\"Name\" from OCYC where \"Type\"='M';", CommandType.Text);
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
}
