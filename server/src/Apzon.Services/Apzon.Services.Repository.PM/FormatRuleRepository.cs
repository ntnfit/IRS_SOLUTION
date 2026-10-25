using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.PM;

namespace Apzon.Services.Repository.PM;

public class FormatRuleRepository : BaseService<DataTable>, IFormatRule
{
	protected const string Temp = "#APZ_FCRE";

	public FormatRuleRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetFormatRule(string formId, string itemId, string user)
	{
		try
		{
			string query = "APZ_FormatRule_Get";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@FormId", formId ?? ""), new SqlParameter("@Itemid", itemId ?? ""), new SqlParameter("@UserCode", user ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public virtual string Saves(DataTable dt)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return "Data must not null!";
			}
			string value = Function.ToString(dt.Rows[0]["UserCode"]);
			string value2 = Function.ToString(dt.Rows[0]["FormId"]);
			string value3 = Function.ToString(dt.Rows[0]["ItemId"]);
			if (string.IsNullOrEmpty(value))
			{
				return "User save setting can not empty";
			}
			if (string.IsNullOrEmpty(value2))
			{
				return "Form save setting can not empty";
			}
			if (string.IsNullOrEmpty(value3))
			{
				return "Item save setting can not empty";
			}
			DataTable tableStruct = GetTableStruct("APZ_FCRE");
			if (!tableStruct.HasColumn())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dt = dt.DefaultView.ToTable(distinct: false, source.ToArray());
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dt, "#APZ_FCRE");
				BulkCopy(dt, "#APZ_FCRE");
				ExecuteNonQuery("APZ_FormatRule_Saves", CommandType.StoredProcedure, new SqlParameter("@userCode", value), new SqlParameter("@formId", value2), new SqlParameter("@itemId", value3));
				dbTransaction.Commit();
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string CopyFormatRule(DataTable dt)
	{
		try
		{
			if (dt.IsNotNull())
			{
				using (DatabaseService.OpenConnection())
				{
					using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
					ExecuteNonQuery("APZ_FormatRule_Copy", CommandType.StoredProcedure, new SqlParameter("@ItemId", Function.ToString(dt.Rows[0]["ItemId"])), new SqlParameter("@FormId", Function.ToString(dt.Rows[0]["FormId"])), new SqlParameter("@UserName", Function.ToString(dt.Rows[0]["UserName"])), new SqlParameter("@CopyUser", Function.ToString(dt.Rows[0]["CopyUser"])), new SqlParameter("@CopyAll", Function.ToString(dt.Rows[0]["CopyAll"])));
					dbTransaction.Commit();
					return string.Empty;
				}
			}
			return "Selected data is null";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Delete(string formId, string itemId, string userId)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = DeleteQuery();
				ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@itemId", itemId), new SqlParameter("@formId", formId), new SqlParameter("@userCode", userId));
				dbTransaction.Commit();
				return string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected virtual string DeleteQuery()
	{
		return "DELETE FROM APZ_FCRE where UserCode = @userCode and FormId = @formId and ItemId = @itemId";
	}
}
