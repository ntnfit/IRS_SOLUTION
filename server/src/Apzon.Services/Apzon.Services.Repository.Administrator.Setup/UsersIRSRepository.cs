using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class UsersIRSRepository : BaseService<IUsersIRS>, IUsersIRS
{
	public UsersIRSRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public HttpResult Save(UsersImpl user)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				string query = "if @user not in (Select USER_CODE From APZ_OUSR where USER_CODE = @user)  INSERT INTO APZ_OUSR (USER_CODE, PASSWORD, U_NAME, E_Mail, Fax, Locked, createDate)  values (@user, @pass, @Name, @Email, @Fax, @Locked, @createdate)Else UPDATE APZ_OUSR SET USER_CODE = @user , PASSWORD = @pass, U_NAME = @Name, E_Mail = @Email, Fax = @Fax, Locked = @Locked where USER_CODE = @user";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@user", user.USER_CODE));
				list.Add(new SqlParameter("@pass", Function.GetCodePass(user.PASSWORD)));
				list.Add(new SqlParameter("@Name", user.U_NAME));
				list.Add(new SqlParameter("@Email", user.E_Mail));
				list.Add(new SqlParameter("@Fax", user.Fax));
				list.Add(new SqlParameter("@Locked", Function.ParseChar(user.Locked)));
				list.Add(new SqlParameter("@createdate", user.createDate));
				if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return new HttpResult
					{
						Message = ""
					};
				}
				dbTransaction.Rollback();
				return new HttpResult
				{
					Message = "Error"
				};
			}
			catch (Exception ex)
			{
				dbTransaction.Dispose();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
				return new HttpResult
				{
					Message = "error"
				};
			}
		}
	}

	public DataTable GetData()
	{
		try
		{
			string query = "Select USER_CODE, U_NAME from APZ_OUSR";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetPocuse(string user)
	{
		try
		{
			string query = "Select * from APZ_OUSR where USER_CODE = @user";
			SqlParameter sqlParameter = new SqlParameter("@user", user);
			return ExecuteDataTable(query, CommandType.Text, sqlParameter);
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}
}
