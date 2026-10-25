using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.HumanResources.Salary;

public class SalaryFormulaRepository : BaseService<ISalaryFormula>, ISalaryFormula
{
	public SalaryFormulaRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetData(string sql)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				DataSet result = ExecuteData(sql, CommandType.Text, (SqlParameter[])null);
				dbTransaction.Rollback();
				return result;
			}
			catch (Exception)
			{
				dbTransaction.Rollback();
				return new DataSet();
			}
		}
	}

	public HttpResult Save(DataTable model)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				foreach (DataRow row in model.Rows)
				{
					string formId = Function.ToString(row["FormID"]);
					string medthodId = Function.ToString(row["MethodID"]);
					int num = Function.ParseInt(row["branch"]);
					int num2 = Function.ParseInt(row["dept"]);
					SqlParameter[] parameters = new SqlParameter[7]
					{
						new SqlParameter("@Formula", Function.ToString(row["Formula"])),
						new SqlParameter("@Description", Function.ToString(row["Description"])),
						new SqlParameter("@FormID", Function.ToString(row["FormID"])),
						new SqlParameter("@MethodID", Function.ToString(row["MethodID"])),
						new SqlParameter("@branch", num),
						new SqlParameter("@dept", num2),
						new SqlParameter("@Active", Function.ToString(row["Active"]))
					};
					string query = ((!Exists(formId, medthodId, num, num2)) ? "INSERT INTO APZ_SAL4(\"FormID\",\"MethodID\",\"branch\",\"dept\",\"Description\",\"Formula\",\"Active\") VALUES (@FormID,@MethodID,@branch,@dept,@Description,@Formula,@Active);" : "UPDATE APZ_SAL4 SET \"Formula\"=@Formula,\"Description\"=@Description,\"Active\"=@Active WHERE \"FormID\"=@FormID and \"MethodID\"=@MethodID and \"branch\" = @branch and \"dept\" = @dept;");
					if (ExecuteNonQuery(query, CommandType.Text, parameters) <= 0)
					{
						return new HttpResult(Enums.MessageCode.Error, "Update Error!");
					}
				}
				dbTransaction.Commit();
				return new HttpResult();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public bool Exists(string formId, string medthodId, int branch, int dept)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT 1 FROM APZ_SAL4 WHERE \"FormID\"=@FormId and \"MethodID\"=@MethodId and \"branch\" = @branch and \"dept\" = @dept;", CommandType.Text, new SqlParameter("@FormId", formId), new SqlParameter("@MethodId", medthodId), new SqlParameter("@branch", branch), new SqlParameter("@dept", dept));
			if (dataTable.IsNotNull() && dataTable.Rows.Count > 0)
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public string GetFormulaText(string formId, Enums.EventId eventId, int branch, int dept)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@formid", formId),
				new SqlParameter("@methodid", (int)eventId),
				new SqlParameter("@branch", branch),
				new SqlParameter("@dept", dept)
			};
			DataTable dataTable = ExecuteDataTable("APZ_SalaryFormula_GetFormulaText", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0]["Formula"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public DataTable GetFormula(string formCode, Enums.EventId enumEvent, int branch, int dept)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@formid", formCode),
				new SqlParameter("@methodid", (int)enumEvent),
				new SqlParameter("@branch", branch),
				new SqlParameter("@dept", dept)
			};
			return ExecuteDataTable("select * from APZ_SAL4 where \"FormID\" = @formid and \"MethodID\" = @methodid and \"branch\" = @branch and \"dept\" = @dept;", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string DeleteFormula(string formId, Enums.EventId eventId, int branch, int dept)
	{
		try
		{
			if (string.IsNullOrEmpty(formId))
			{
				return "Form Item can not be null";
			}
			if (eventId == 0)
			{
				return "Event can not be null";
			}
			if (string.IsNullOrEmpty(Function.ToString(branch)))
			{
				return "Branch can not be null";
			}
			if (string.IsNullOrEmpty(Function.ToString(dept)))
			{
				return "Department can not be null";
			}
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@formid", formId),
				new SqlParameter("@methodid", (int)eventId),
				new SqlParameter("@branch", branch),
				new SqlParameter("@dept", dept)
			};
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				ExecuteNonQuery("DELETE FROM APZ_SAL4 where \"FormID\" = @formid and \"MethodID\" = @methodid and \"branch\" = @branch and \"dept\" = @dept;", CommandType.Text, list.ToArray());
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
}
