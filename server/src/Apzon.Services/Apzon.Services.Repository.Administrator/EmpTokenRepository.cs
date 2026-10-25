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

public class EmpTokenRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IEmpToken
{
	public string TempOhet = "#APZ_OHET";

	public string Save(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string text = "";
				List<SqlParameter> list = new List<SqlParameter>();
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					text = text + "DELETE from APZ_OHET WHERE \"EmpId\" = @EmpId" + i + "; ";
					text = text + " INSERT INTO APZ_OHET (\"EmpId\" ,\"TokenStr\") VALUES (@EmpId" + i + ",@TokenStr" + i + "); ";
					list.Add(new SqlParameter("@EmpId" + i, Function.ParseInt(dt.Rows[i]["EmpId"])));
					list.Add(new SqlParameter("@TokenStr" + i, Function.ToString(dt.Rows[i]["TokenStr"])));
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

	public DataTable GetEmpTokens(int userId)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@UserId", userId);
			return ExecuteDataTable("APZ_EmpToken_GetData", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string CheckToken(int empId, string token)
	{
		List<SqlParameter> list = new List<SqlParameter>();
		try
		{
			list.Add(new SqlParameter("@EmpId", empId));
			DataTable dataTable = ExecuteDataTable("SP_CHECK_TOKEN", CommandType.StoredProcedure, list.ToArray());
			if (dataTable.IsNotNull())
			{
				if (Function.ToString(Function.Decrypt(RSA.PrivateKey, dataTable.Rows[0]["TokenStr"].ToString())).Equals(token))
				{
					return string.Empty;
				}
				return "Token not match!";
			}
			return "Token has not been assigned to this employee!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
