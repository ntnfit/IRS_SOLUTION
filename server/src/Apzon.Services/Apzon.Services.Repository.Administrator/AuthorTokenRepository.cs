using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class AuthorTokenRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IAuthorToken
{
	public string TempHet1 = "#APZ_HET1";

	public string Save(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				string text = "";
				DataTable dataTable = ExecuteDataTable("SELECT \"EmpId\" FROM APZ_OHET;", CommandType.Text, (SqlParameter[])null);
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					if (Function.ParseBool(dt.Rows[i]["DelAll"]))
					{
						continue;
					}
					int num = 0;
					for (int j = 0; j < dataTable.Rows.Count; j++)
					{
						if (Function.ToString(dataTable.Rows[j]["EmpId"]) == Function.ToString(dt.Rows[i]["EmpId"]))
						{
							num++;
						}
					}
					if (num == 0)
					{
						text = text + " INSERT INTO APZ_OHET (\"EmpId\") VALUES (@Emp" + i + "); ";
						list.Add(new SqlParameter("@Emp" + i, Function.ParseInt(dt.Rows[i]["EmpId"])));
					}
				}
				text += " DELETE from APZ_HET1 WHERE \"UserId\" = @UserIdDel; ";
				list.Add(new SqlParameter("@UserIdDel", Function.ParseInt(dt.Rows[0]["UserId"])));
				for (int k = 0; k < dt.Rows.Count; k++)
				{
					text = text + " INSERT INTO APZ_HET1 (\"EmpId\" ,\"UserId\") VALUES (@EmpId" + k + ",@UserId" + k + "); ";
					list.Add(new SqlParameter("@EmpId" + k, Function.ParseInt(dt.Rows[k]["EmpId"])));
					list.Add(new SqlParameter("@UserId" + k, Function.ParseInt(dt.Rows[0]["UserId"])));
				}
				if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return string.Empty;
				}
				return "Save Error!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetAllEmployees()
	{
		try
		{
			return ExecuteDataTable("APZ_AuthorToken_GetEmps", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetAuthorizedEmp(int userId)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@UserId", userId);
			DataTable dataTable = ExecuteDataTable("SELECT a.* FROM ohem a INNER JOIN ousr b ON a.\"userId\"=b.USERID WHERE b.USERID= @UserId;", CommandType.Text, sqlParameter);
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (dataTable.IsNotNull())
				{
					try
					{
						List<SqlParameter> list = new List<SqlParameter>();
						string authorizedEmp = GetAuthorizedEmp();
						list.Add(new SqlParameter("@EmpId", Function.ParseInt(dataTable.Rows[0]["EmpId"])));
						list.Add(new SqlParameter("@UserId", Function.ParseInt(dataTable.Rows[0]["UserId"])));
						if (ExecuteNonQuery(authorizedEmp, CommandType.Text, list.ToArray()) > 0)
						{
							dbTransaction.Commit();
						}
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
						dbTransaction.Rollback();
					}
				}
			}
			return ExecuteDataTable("SELECT m.\"EmpId\" FROM OHEM m JOIN APZ_HET1 t ON m.\"EmpId\" = t.\"EmpId\" WHERE t.\"UserId\" = @UserId; ", CommandType.Text, sqlParameter);
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
		}
		return new DataTable();
	}

	protected virtual string GetAuthorizedEmp()
	{
		return "DELETE FROM APZ_HET1 WHERE \"EmpId\" = @EmpId AND \"UserId\" = @UserId;  INSERT INTO APZ_HET1 (\"EmpId\" ,\"UserId\") VALUES (@EmpId,@UserId);IF @EmpId NOT IN (SELECT \"EmpId\" FROM APZ_OHET) BEGIN INSERT INTO APZ_OHET (\"EmpId\" ,\"TokenStr\") VALUES (@EmpId,'');  END;";
	}
}
