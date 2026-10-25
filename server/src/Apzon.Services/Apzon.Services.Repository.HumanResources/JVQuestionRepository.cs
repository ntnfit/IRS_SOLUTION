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
using Apzon.Services.Interfaces.HumanResources;

namespace Apzon.Services.Repository.HumanResources;

public class JVQuestionRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IJVQuestion
{
	private string processType = "";

	private string TempTableTab = "#dtAPZ_OQLG";

	private string TempBenefitTable = "#dtAPZ_OBNF";

	public DataTable LoadQuestionGroup()
	{
		try
		{
			string query = "select * from APZ_OQLG;";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadQuestion()
	{
		try
		{
			string query = "select * from APZ_OQLQ;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable.IsNotNull())
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadAnswer(int id)
	{
		try
		{
			string query = "select * from APZ_OQLA WHERE \"QuesCode\"=@id;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@id", id));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable.IsNotNull())
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckExist(Tuple<int, string, string> ID)
	{
		try
		{
			return GetUnitOfWork().DefineNew.Check(ID.Item1, ID.Item2, ID.Item3);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckQuestion(int ID)
	{
		try
		{
			string query = "SELECT * FROM APZ_OQLI WHERE \"QuesCode\"=@ID;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ID", ID));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string DeleteQuestion(int ID)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "DELETE FROM APZ_OQLA WHERE \"QuesCode\"=@ID;\r\n                                         DELETE FROM APZ_OQLQ WHERE \"QuesCode\"=@ID;";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@ID", ID));
				if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "No rows deleted";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable CheckQuestionGroup(int ID)
	{
		try
		{
			string query = "SELECT TOP 1 QuesGroup FROM APZ_OQLQ WHERE QuesGroup=@ID\r\n                  UNION ALL\r\n                                    select TOP 1 QuestionsLibrary from APZ_HJV4 WHERE QuestionsLibrary=@ID";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ID", ID));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadBenefit()
	{
		try
		{
			string query = "select * from APZ_OBNF;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckBenefit(int ID)
	{
		try
		{
			string query = "SELECT * FROM APZ_OBNO WHERE \"BenefitID\"=@ID;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ID", ID));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveBenefit(Tuple<DataTable, string> dt)
	{
		processType = "SaveBenefit";
		return Process(ApzObjectTypes.oBenefits, -1, Enums.TransactionType.A, dt.Item1);
	}

	public HttpResult SaveData(Tuple<DataTable, string> dt)
	{
		processType = "SaveQuestionGroup";
		return Process(ApzObjectTypes.oQuestions, -1, Enums.TransactionType.A, dt.Item1);
	}

	protected virtual string ProcessBenefitQuery(string tblName)
	{
		return string.Format("declare @i int  = (SELECT ISNULL(MAX(Id),0) FROM APZ_OBNF) \r\n\t                                UPDATE {0} SET Id  = @i ,@i = @i + 1\r\n\t                                where Id = 0 OR Id is null\r\n                                    DELETE APZ_OBNF\r\n                                    INSERT INTO APZ_OBNF\r\n                                    SELECT * FROM {0}", tblName);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		string text = "";
		try
		{
			string text2 = processType;
			if (!(text2 == "SaveQuestionGroup"))
			{
				if (text2 == "SaveBenefit")
				{
					TempTableTab = "#DTAPZ_OBNF";
					CreateTempTable(document, TempTableTab);
					BulkCopy(document, TempTableTab);
					text = ProcessBenefitQuery(TempTableTab);
					ExecuteScalar(text, CommandType.Text);
				}
			}
			else
			{
				CreateTempTable(document, TempTableTab);
				BulkCopy(document, TempTableTab);
				text = string.Format("declare @i int  = (SELECT ISNULL(MAX(Id),0) FROM APZ_OQLG) \r\n\t                                            UPDATE {0} SET Id  = @i ,@i = @i + 1\r\n\t                                            where Id = 0 OR Id is null\r\n                                                DELETE APZ_OQLG\r\n                                                INSERT INTO APZ_OQLG\r\n                                                SELECT * FROM {0}", TempTableTab);
				ExecuteScalar(text, CommandType.Text);
			}
			objNum = objId;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
