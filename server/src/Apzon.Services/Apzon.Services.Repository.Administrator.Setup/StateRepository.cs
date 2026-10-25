using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Administrator;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator.Setup;

public class StateRepository(IDatabaseService databaseService) : BaseService<StateImpl>(databaseService), IState
{
	private DataTable dt = new DataTable();

	public string Save(DataTable dt)
	{
		string empty = string.Empty;
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			new List<SqlParameter>();
			try
			{
				empty = Remove(dt.Rows[0]["Country"].ToString());
				if (string.IsNullOrEmpty(empty))
				{
					try
					{
						for (int i = 0; i < dt.Rows.Count; i++)
						{
							string text = Update(dt.Rows[i]);
							if (!string.IsNullOrEmpty(text))
							{
								empty = text;
								dbTransaction.Rollback();
								break;
							}
						}
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save  FAILED ", ex.Message);
						dbTransaction.Rollback();
					}
				}
				if (string.IsNullOrWhiteSpace(empty))
				{
					dbTransaction.Commit();
					return empty;
				}
				return empty;
			}
			catch (Exception ex2)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save  FAILED ", ex2.Message);
				return ex2.Message;
			}
		}
	}

	private string Update(DataRow dataRow)
	{
		_ = string.Empty;
		List<SqlParameter> list = new List<SqlParameter>();
		try
		{
			bool flag = CheckData(dataRow["Code"].ToString(), dataRow["Country"].ToString());
			string empty = string.Empty;
			if (!flag)
			{
				empty = "INSERT INTO OCST  (\"Code\",\"Country\",\"Name\",\"UserSign\",\"eCode\",\"GNRECode\")\r\n                                                     VALUES \r\n                                        ( ";
				empty += " @Code ";
				list.Add(new SqlParameter("@Code", Function.ToString(dataRow["Code"])));
				empty += ", @Country ";
				list.Add(new SqlParameter("@Country", Function.ToString(dataRow["Country"])));
				empty += ", @Name ";
				list.Add(new SqlParameter("@Name", Function.ToString(dataRow["Name"])));
				empty += ", @UserSign ";
				list.Add(new SqlParameter("@UserSign", Function.ParseInt(dataRow["UserSign"])));
				empty += ", @eCode ";
				list.Add(new SqlParameter("@eCode", Function.ToString(dataRow["eCode"])));
				empty += ", @GNRECode ";
				list.Add(new SqlParameter("@GNRECode", Function.ToString(dataRow["GNRECode"])));
				empty += " )";
			}
			else
			{
				list = new List<SqlParameter>();
				empty = "UPDATE OCST  \r\n                            SET ";
				empty += " \"Country\"=@Country ";
				list.Add(new SqlParameter("@Country", Function.ToString(dataRow["Country"])));
				empty += ", \"Name\"=@Name ";
				list.Add(new SqlParameter("@Name", Function.ToString(dataRow["Name"])));
				empty += ",\"UserSign\"= @UserSign ";
				list.Add(new SqlParameter("@UserSign", Function.ParseInt(dataRow["UserSign"])));
				empty += ",\"eCode\"= @eCode ";
				list.Add(new SqlParameter("@eCode", Function.ToString(dataRow["eCode"])));
				empty += ",\"GNRECode\"= @GNRECode ";
				list.Add(new SqlParameter("@GNRECode", Function.ToString(dataRow["GNRECode"])));
				empty += " WHERE \"Code\" =@Code";
				list.Add(new SqlParameter("@Code", Function.ToString(dataRow["Code"])));
			}
			if (ExecuteNonQuery(empty, CommandType.Text, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public bool CheckData(string key, string country)
	{
		try
		{
			string query = "select * from OCST where Code = '" + key + "' and Country = '" + country + "' ";
			dt = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dt != null && dt.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckData  FAILED ", ex.Message);
		}
		return false;
	}

	public DataTable GetCountry(int user)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserSign", user));
			string query = "APZ_Country_GetData";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetListAllState()
	{
		try
		{
			string query = "select A.\"Code\" ,A.\"Country\", A.\"Name\"  from OCST A  ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string Remove(string country)
	{
		try
		{
			string query = "Delete from OCST where \"Country\" = '" + country + "' ";
			ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null);
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Remove  FAILED ", ex.Message);
			return ex.Message;
		}
	}
}
