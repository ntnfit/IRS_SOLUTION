using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class ExternalRequestRepository : BaseService<DataTable>, IExternalRequest
{
	public ExternalRequestRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public bool CheckExternalRequestToken(string token)
	{
		try
		{
			return Function.ParseInt(ExecuteScalar("select top 1 1 from APZ_EXRL where \"Token\" = @token and Isnull(Status,'N') <> 'C'", CommandType.Text, new SqlParameter("@token", token))) == 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	public void SaveExternalRequestToken(string token, string status, string errMess)
	{
		try
		{
			string query = "UPDATE APZ_EXRL set [Status] = @status, ReqDate = @reqDate, ReqTime = @reqTime, ErrorMes = @mes where Token = @token";
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@token", token), new SqlParameter("@status", status ?? ""), new SqlParameter("@reqDate", DateTime.Now.Date), new SqlParameter("@reqTime", DateTime.Now.Hour * 100 + DateTime.Now.Minute), new SqlParameter("@mes", errMess ?? ""));
				dbTransaction.Commit();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}
}
