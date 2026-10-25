using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.DashBoard;

namespace Apzon.Services.Repository.DashBoard;

public class DashBoardAuthRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IDashboardAuthorization
{
	public string TempTable = "#APZ_DTB2";

	public DataTable GetAllAuth(string user)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@user", user));
			new DataTable();
			string query = "SELECT a.CatId,a.TabId,a.Name as TabName,b.Name as CatName,@user as USER_CODE,c.tabcode,c.authoriza FROM  APZ_ODTB a JOIN APZ_ODCT b ON a.CatId = b.CatId LEFT JOIN APZ_DTB2 c ON c.tabcode = a.TabId AND c.USER_CODE = @user";
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	public HttpResult SaveDoc(DataTable dt)
	{
		return Process(ApzObjectTypes.oDashboardAuth, -1, Enums.TransactionType.U, dt);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTable);
			BulkCopy(document, TempTable);
			object obj = ExecuteScalar("SP_APZ_DTB2_SAVE", CommandType.Text);
			objId = Function.ToString(obj);
			objNum = objId;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetFormAuth(string user, string menu)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@tab", Function.ToString(menu)));
			list.Add(new SqlParameter("@user", Function.ToString(user)));
			new DataTable();
			string query = " select a.menucode from OMNI left join OPRM a  on OMNI.menucode=a.menucode and\r\n                a.USER_CODE = @user where a.authoriza='F' and a.menucode=@menu";
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Form Auth  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAllAuthForm(string user)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@user", Function.ToString(user)));
			new DataTable();
			string query = "Select a.menucode, a.authoriza, b.MenuName From\r\n                            OPRM a left join OMNI b on a.menucode = b.menucode where USER_CODE = @user\r\n                            AND (authoriza='F' OR authoriza='R')";
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetAllAuthForm  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string CopyAuth(List<string> lstUser)
	{
		string result = "";
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string value = lstUser[0];
				lstUser.RemoveAt(0);
				string value2 = string.Join(",", lstUser.ToArray());
				string query = "Apz_DashboardAuth_Copy";
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, new SqlParameter("@source", value), new SqlParameter("@target", value2)) > 0)
				{
					dbTransaction.Commit();
				}
				else
				{
					result = "ERROR";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CopyAuth  FAILED ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public DataTable GetUserForCopy(string user)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@user", Function.ToString(user)));
			string query = "select  USER_CODE,U_NAME ,isnull(b.Name,'') as Department from OUSR a\r\n                    left join OUDP b on b.Code = a.Department  where user_code <> @user";
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetUserForCopy  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
