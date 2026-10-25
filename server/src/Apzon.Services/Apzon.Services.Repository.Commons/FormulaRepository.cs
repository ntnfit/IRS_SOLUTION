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

namespace Apzon.Services.Repository.Commons;

public class FormulaRepository : BaseService<IFormula>, IFormula
{
	public FormulaRepository(IDatabaseService databaseService)
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
					string text = Function.ToString(row["ItemId"]);
					SqlParameter[] parameters = new SqlParameter[6]
					{
						new SqlParameter("@Formula", Function.ToString(row["Formula"])),
						new SqlParameter("@Description", Function.ToString(row["Description"])),
						new SqlParameter("@FormID", Function.ToString(row["FormID"])),
						new SqlParameter("@MethodID", Function.ToString(row["MethodID"])),
						new SqlParameter("@Active", Function.ToString(row["Active"])),
						new SqlParameter("@itemId", text)
					};
					string query = ((!Exists(formId, medthodId, text)) ? "INSERT INTO APZ_FORMULA(\"FormID\", \"ItemId\",\"MethodID\",\"Description\",\"Formula\",\"Active\") VALUES (@FormID, @itemId,@MethodID,@Description,@Formula,@Active);" : "UPDATE APZ_FORMULA SET \"Formula\"=@Formula,\"Description\"=@Description,\"Active\"=@Active WHERE \"FormID\"=@FormID and \"MethodID\"=@MethodID and \"ItemId\" = @itemId;");
					if (ExecuteNonQuery(query, CommandType.Text, parameters) <= 0)
					{
						return new HttpResult(Enums.MessageCode.Error, "No record effected");
					}
				}
				dbTransaction.Commit();
				return new HttpResult(Enums.MessageCode.Success);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public bool Exists(string formId, string medthodId, string itemId)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT 1 FROM APZ_FORMULA WHERE \"FormID\"=@FormId and \"MethodID\"=@MethodId and \"ItemId\" = @itemId;", CommandType.Text, new SqlParameter("@FormId", formId), new SqlParameter("@MethodId", medthodId), new SqlParameter("@itemId", itemId));
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

	public string GetFormulaText(string formId, Enums.EventId eventId, string itemEvent)
	{
		try
		{
			if (string.IsNullOrEmpty(formId) || string.IsNullOrEmpty(itemEvent))
			{
				return "";
			}
			return Function.ToString(ExecuteScalar("select \"Formula\" from APZ_Formula where \"FormID\" = @formid and \"MethodID\" = @methodid and \"ItemId\" = @itemId and \"Active\"='Y';", CommandType.Text, new SqlParameter("@formid", formId), new SqlParameter("@methodid", (int)eventId), new SqlParameter("@itemId", itemEvent)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public DataTable GetCategory()
	{
		try
		{
			return ExecuteDataTable("apz_formula_getcategory", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetFormula(string formCode, Enums.EventId enumEvent, string itemId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@formid", formCode),
				new SqlParameter("@methodid", (int)enumEvent),
				new SqlParameter("@itemId", itemId)
			};
			return ExecuteDataTable("select * from APZ_Formula where \"FormID\" = @formid and \"MethodID\" = @methodid and \"ItemId\" = @itemId;", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string DeleteFormula(string formId, string itemId, Enums.EventId eventId)
	{
		try
		{
			if (string.IsNullOrEmpty(formId))
			{
				return "Form Item can not be null";
			}
			if (string.IsNullOrEmpty(itemId))
			{
				return "Item id can not be null";
			}
			if (eventId == 0)
			{
				return "Event can not be null";
			}
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@formid", formId),
				new SqlParameter("@methodid", (int)eventId),
				new SqlParameter("@itemId", itemId)
			};
			ExecuteNonQuery("DELETE FROM APZ_Formula where \"FormID\" = @formid and \"MethodID\" = @methodid and \"ItemId\" = @itemId;", CommandType.Text, list.ToArray());
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
