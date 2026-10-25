using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class ThirdPartyTokenRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IThirdPartyToken
{
	private readonly string _tempTableOtpt = "#APZ_OTPT";

	public virtual DataTable GetData(string tokenName, string remarks, string status)
	{
		try
		{
			string query = "APZ_ThirdPartyToken_GetToken";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@tokenName", tokenName), new SqlParameter("@remarks", remarks), new SqlParameter("@status", status));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public virtual HttpResult Saves(DataTable dt, string action)
	{
		try
		{
			string query = "APZ_ThirdPartyToken_Save";
			string message = "Save Failed!";
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dt, _tempTableOtpt);
				BulkCopy(dt, _tempTableOtpt);
				DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@trantype", action));
				if (Function.ToString(dataTable.Rows[0][0]) == "1")
				{
					message = Function.ToString(dataTable.Rows[0][1]);
					dbTransaction.Commit();
					return new HttpResult
					{
						MessageCode = Enums.MessageCode.SaveSuccess,
						Message = message
					};
				}
				return new HttpResult
				{
					MessageCode = Enums.MessageCode.Error,
					Message = message
				};
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				MessageCode = Enums.MessageCode.Error,
				Message = ex.Message
			};
		}
	}

	public virtual HttpResult DeleteDoc(int absEntry)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = $"delete from APZ_OTPT where \"AbsEntry\" = {absEntry}";
				if (ExecuteNonQuery(query, CommandType.Text) == 1)
				{
					dbTransaction.Commit();
					return new HttpResult
					{
						Message = "Delete successfully!",
						MessageCode = Enums.MessageCode.Success
					};
				}
				dbTransaction.Rollback();
				return new HttpResult
				{
					Message = "Delete Failed!",
					MessageCode = Enums.MessageCode.Failed
				};
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual HttpResult GetMaxEntry()
	{
		string query = "select max(AbsEntry) from APZ_OTPT";
		try
		{
			return new HttpResult
			{
				Content = ExecuteScalar(query, CommandType.Text),
				MessageCode = Enums.MessageCode.Success
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				Message = "Failed!",
				MessageCode = Enums.MessageCode.Success
			};
		}
	}
}
