using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class UserDefaultRepository(IDatabaseService databaseService) : BaseService<IUserDefault>(databaseService), IUserDefault
{
	private DataTable dt = new DataTable();

	public DataTable GetOUDG(string code)
	{
		DataTable dataTable = new DataTable();
		try
		{
			string query = "APZ_GetOUDG";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", code));
			dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetOUDG ", ex.Message);
			return dataTable;
		}
	}

	public DataTable GetUDG1(string code, string type)
	{
		DataTable dataTable = new DataTable();
		try
		{
			string query = "APZ_GetUDG1";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", code));
			dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetUDG1 ", ex.Message);
			return dataTable;
		}
	}

	public DataTable GetUDG2(string code)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_GetUDG2";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", code));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (result.IsNotNull())
			{
				return result;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetUDG2 ", ex.Message);
		}
		return result;
	}

	public string Save(DataSet ds)
	{
		DataTable dataTable = new DataTable();
		DataTable dataTable2;
		DataTable dataTable3;
		if (ds.Tables.Count == 2)
		{
			dataTable2 = ds.Tables[0];
			dataTable3 = ds.Tables[1];
		}
		else
		{
			dataTable2 = ds.Tables[0];
			dataTable3 = ds.Tables[1];
			dataTable = ds.Tables[2];
		}
		string empty = string.Empty;
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				DataTable structureTable = GetStructureTable("OUDG");
				DataTable structureTable2 = GetStructureTable("UDG1");
				DataTable structureTable3 = GetStructureTable("UDG2");
				if (dataTable2.IsNotNull())
				{
					string code = Function.ToString(dataTable2.Rows[0]["Code"]);
					if (Exists(code, "OUDG"))
					{
						string text = UpdateData(dataTable2.Rows[0], structureTable);
						if (!string.IsNullOrWhiteSpace(text))
						{
							return text;
						}
					}
					else
					{
						string text2 = InsertData(dataTable2.Rows[0], structureTable);
						if (!string.IsNullOrWhiteSpace(text2))
						{
							return text2;
						}
					}
					if (dataTable3.IsNotNull())
					{
						for (int i = 0; i < dataTable3.Rows.Count; i++)
						{
							code = Function.ToString(dataTable3.Rows[i]["Code"]);
							string key = Function.ToString(dataTable3.Rows[i]["ObjType"]);
							if (ExistsUdg1(code, key))
							{
								string text3 = Update_UDG1(dataTable3.Rows[i], structureTable2);
								if (!string.IsNullOrEmpty(text3))
								{
									return text3;
								}
							}
							else
							{
								string text4 = Insert_UDG1(dataTable3.Rows[i], structureTable2);
								if (!string.IsNullOrEmpty(text4))
								{
									return text4;
								}
							}
						}
					}
					if (dataTable.IsNotNull())
					{
						for (int j = 0; j < dataTable.Rows.Count; j++)
						{
							code = Function.ToString(dataTable.Rows[j]["Code"]);
							string key2 = Function.ToString(dataTable.Rows[j]["CardName"]);
							if (ExistsUdg2(code, key2))
							{
								string text5 = Update_UDG2(dataTable.Rows[j], structureTable3);
								if (!string.IsNullOrEmpty(text5))
								{
									return text5;
								}
							}
							else
							{
								string text6 = Insert_UDG2(dataTable.Rows[j], structureTable3);
								if (!string.IsNullOrEmpty(text6))
								{
									return text6;
								}
							}
						}
					}
				}
				dbTransaction.Commit();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE ", ex.Message);
		}
		return empty;
	}

	private string InsertData(DataRow dataRow, DataTable dtStrucrOudg)
	{
		try
		{
			string text = "INSERT INTO OUDG (";
			for (int i = 0; i < dtStrucrOudg.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStrucrOudg.Rows[i]["COLUMN_NAME"]);
				if (dataRow.IsExistsColumn(text2))
				{
					text = text + text2 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += ") VALUES ( ";
			for (int j = 0; j < dtStrucrOudg.Rows.Count; j++)
			{
				string text3 = Function.ToString(dtStrucrOudg.Rows[j]["COLUMN_NAME"]);
				if (dataRow.IsExistsColumn(text3))
				{
					text = text + "@" + text3 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += ")";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStrucrOudg);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
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

	private string Insert_UDG1(DataRow dataRow, DataTable dtStrucr_UDG1)
	{
		try
		{
			string text = "INSERT INTO UDG1 (";
			for (int i = 0; i < dtStrucr_UDG1.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStrucr_UDG1.Rows[i]["COLUMN_NAME"]);
				if (dataRow.IsExistsColumn(text2))
				{
					text = text + text2 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += ") VALUES ( ";
			for (int j = 0; j < dtStrucr_UDG1.Rows.Count; j++)
			{
				string text3 = Function.ToString(dtStrucr_UDG1.Rows[j]["COLUMN_NAME"]);
				if (dataRow.IsExistsColumn(text3))
				{
					text = text + "@" + text3 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += ")";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStrucr_UDG1);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
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

	private string Insert_UDG2(DataRow dataRow, DataTable dtStrucr_UDG2)
	{
		try
		{
			string text = "INSERT INTO UDG2 (";
			for (int i = 0; i < dtStrucr_UDG2.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStrucr_UDG2.Rows[i]["COLUMN_NAME"]);
				if (dataRow.IsExistsColumn(text2) && !string.IsNullOrEmpty(text2))
				{
					text = text + text2 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += ") VALUES ( ";
			for (int j = 0; j < dtStrucr_UDG2.Rows.Count; j++)
			{
				string text3 = Function.ToString(dtStrucr_UDG2.Rows[j]["COLUMN_NAME"]);
				if (dataRow.IsExistsColumn(text3) && !string.IsNullOrEmpty(text3))
				{
					text = text + "@" + text3 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += ")";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStrucr_UDG2);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
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

	private string UpdateData(DataRow dataRow, DataTable dtStrucr_OUDG)
	{
		try
		{
			string text = "UPDATE OUDG SET";
			for (int i = 0; i < dtStrucr_OUDG.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStrucr_OUDG.Rows[i]["COLUMN_NAME"]);
				if (dataRow.IsExistsColumn(text2) && text2 != "Code")
				{
					text = text + Environment.NewLine + text2 + "=@" + text2 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += " where Code= @Code";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStrucr_OUDG);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
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

	private string Update_UDG1(DataRow dataRow, DataTable dtStrucrUdg1)
	{
		try
		{
			string text = "UPDATE UDG1 SET";
			for (int i = 0; i < dtStrucrUdg1.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStrucrUdg1.Rows[i]["COLUMN_NAME"]);
				if (dataRow.IsExistsColumn(text2) && text2 != "Code" && text2 != "ObjType")
				{
					text = text + Environment.NewLine + text2 + "=@" + text2 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += " where Code= @Code and ObjType =@ObjType ";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStrucrUdg1);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
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

	private string Update_UDG2(DataRow dataRow, DataTable dtStrucr_UDG2)
	{
		try
		{
			string text = "UPDATE UDG2 SET";
			for (int i = 0; i < dtStrucr_UDG2.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStrucr_UDG2.Rows[i]["COLUMN_NAME"]);
				if (dataRow.IsExistsColumn(text2) && text2 != "Code" && text2 != "CreditCard")
				{
					text = text + Environment.NewLine + text2 + "=@" + text2 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += " where Code= @Code and CreditCard =@CreditCard ";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStrucr_UDG2);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
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

	private List<SqlParameter> MappingSqlParameter(DataRow dataRow, DataTable dtStr)
	{
		List<SqlParameter> list = new List<SqlParameter>();
		try
		{
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
						list.Add(new SqlParameter("@" + text, Function.ParseDateTime(dataRow[text])));
						break;
					case "CHAR":
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					case "NTEXT":
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					case "SMALLINT":
						list.Add(new SqlParameter("@" + text, dataRow[text]));
						break;
					case "BIT":
						list.Add(new SqlParameter("@" + text, Function.ParseBool(dataRow[text])));
						break;
					case "NUMERIC":
						list.Add(new SqlParameter("@" + text, Function.ParseDecimal(dataRow[text])));
						break;
					default:
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return list;
	}

	public string Remove(string code)
	{
		try
		{
			string text = "Delete  from UDG2 WHERE Code = @Code ";
			text += "Delete  from UDG1 WHERE Code = @Code ";
			text += "Delete  from OUDG WHERE Code = @Code ";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", code));
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "Erro";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Remove ", ex.Message);
		}
		return "Erro";
	}

	public bool Exists(string code, string tableName)
	{
		try
		{
			string query = "select Code from " + tableName + " where Code=@Code";
			SqlParameter sqlParameter = new SqlParameter("@Code", code);
			if (ExecuteDataTable(query, CommandType.Text, sqlParameter).IsNotNull())
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

	public bool ExistsUdg1(string key1, string key2)
	{
		try
		{
			string query = "select Code from UDG1 where Code=@Code And ObjType =@ObjType";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", key1));
			list.Add(new SqlParameter("@ObjType", key2));
			if (ExecuteDataTable(query, CommandType.Text, list.ToArray()).IsNotNull())
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

	public bool ExistsUdg2(string key1, string key2)
	{
		try
		{
			string query = "select Code from UDG2 where Code=@Code And CreditCard =@CreditCard";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", key1));
			list.Add(new SqlParameter("@CreditCard", key2));
			if (ExecuteDataTable(query, CommandType.Text, list.ToArray()).IsNotNull())
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

	public DataTable GetStructureTable(string tableName)
	{
		try
		{
			if (!tableName.Contains(" "))
			{
				string query = "SELECT COLUMN_NAME,DATA_TYPE\r\n                                    FROM INFORMATION_SCHEMA.COLUMNS\r\n                                    WHERE TABLE_NAME = '" + tableName + "'";
				DataTable result = ExecuteDataTable(query, CommandType.Text);
				if (result.IsNotNull())
				{
					return result;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable Search(DataTable dtFind)
	{
		DataTable dataTable = new DataTable();
		try
		{
			string value = Function.ToString(dtFind.Rows[0]["Code"]);
			string value2 = Function.ToString(dtFind.Rows[0]["Name"]);
			string query = "APZ_SearchOUDG";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", value));
			list.Add(new SqlParameter("@Name", value2));
			dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search ", ex.Message);
			return dataTable;
		}
	}

	public DataTable GetTableColumns(string tableName)
	{
		DataTable dataTable = new DataTable();
		try
		{
			if (ExistsTable(tableName))
			{
				string query = "select top 0 * From " + tableName;
				dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetColumTable ", ex.Message);
			return new DataTable();
		}
		return dataTable;
	}

	private bool ExistsTable(string tableName)
	{
		bool result = false;
		try
		{
			string query = "DECLARE @TableName VARCHAR(100) = @Table;\r\n                            DECLARE @Exists BIT = ( SELECT CAST( COUNT(1) AS BIT ) FROM sys.tables WHERE name = @TableName AND type = 'U' );\r\n                            select @Exists";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Table", tableName));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ToString(dataTable.Rows[0][0]).Trim().ToUpper() == "TRUE";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetRecord(string tbl, string key, string TypeRecord)
	{
		DataTable result = new DataTable();
		try
		{
			if (ExistsTable(tbl))
			{
				string query = "APZ_GetRecord";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@tbl", tbl));
				list.Add(new SqlParameter("@key", key));
				list.Add(new SqlParameter("@TypeRecord", TypeRecord));
				result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
				if (result != null && result.Rows.Count > 0)
				{
					return result;
				}
				return new DataTable();
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetColumTable ", ex.Message);
			return new DataTable();
		}
	}
}
