using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources;

namespace Apzon.Services.Repository.HumanResources;

public class HRMQualificationsRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IHRMQualifications
{
	private string processType = "";

	private string Temp_AdvSource = "#APZ_AdvSource";

	private string Temp_JobType = "#APZ_JobType";

	private string Temp_Qualifications = "#APZ_Qualifications";

	private string Temp_QualificationsGroup = "#APZ_QuaGroup";

	private string Temp_OCLG = "#OCLG";

	public DataTable GetAdvSource()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM APZ_OHAS;", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string RemoveAdvSource(int ID)
	{
		string result = string.Empty;
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				SqlParameter sqlParameter = new SqlParameter("@Id", ID);
				if (ExecuteNonQuery("DELETE  FROM APZ_OHAS Where \"Id\" = @Id;", CommandType.Text, sqlParameter) <= 0)
				{
					dbTransaction.Rollback();
					return Function.ToString("No record effected");
				}
				dbTransaction.Commit();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public DataTable GetJobType()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM APZ_OHJT;", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string RemoveJobType(int ID)
	{
		string result = string.Empty;
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				SqlParameter sqlParameter = new SqlParameter("@Id", ID);
				if (ExecuteNonQuery("DELETE  FROM APZ_OHJT Where \"Id\" = @Id;", CommandType.Text, sqlParameter) <= 0)
				{
					dbTransaction.Rollback();
					return Function.ToString("No record effected");
				}
				dbTransaction.Commit();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public DataTable GetQualificationsGroup()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM APZ_OHQG;", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string RemoveQualificationsGroup(int ID)
	{
		string result = string.Empty;
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				SqlParameter sqlParameter = new SqlParameter("@Id", ID);
				if (ExecuteNonQuery("DELETE  FROM APZ_OHQG where \"Id\"=@Id;", CommandType.Text, sqlParameter) <= 0)
				{
					dbTransaction.Rollback();
					return Function.ToString("No record effected");
				}
				dbTransaction.Commit();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public int GetMaxDocEntry()
	{
		try
		{
			string query = "select isnull( MAX(DocEntry),0) as DocEntry from OIGN";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxAbsID FAILED ", ex.Message);
			return 0;
		}
	}

	public DataTable GetQualifications()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM  APZ_OHQU;", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string RemoveQualifications(int QuaCodes)
	{
		string result = string.Empty;
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				SqlParameter sqlParameter = new SqlParameter("@QuaCode", QuaCodes);
				if (ExecuteNonQuery("DELETE  FROM APZ_OHQU WHERE \"QuaCode\" = @QuaCode and \"QuaCode\" NOT IN(select \"QuaCode\" from APZ_OHAQ);", CommandType.Text, sqlParameter) > 0)
				{
					dbTransaction.Commit();
				}
				else
				{
					result = "Cannot remove name because it has been used in applicants";
					dbTransaction.Rollback();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDoc FAILED ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public DataTable CheckQuaGroup(int QuaCodes)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@QuaCode", QuaCodes);
			DataTable dataTable = ExecuteDataTable("select* from APZ_OHAQ  WHERE QuaCode = @QuaCode", CommandType.Text, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDetail  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetQuaCode(int quaId)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@quacode", quaId);
			return ExecuteDataTable("SELECT * FROM  APZ_OHQU where QuaCode = @quacode", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public HttpResult SaveJobType(DataTable dt)
	{
		processType = "SaveJobType";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oJobType, -1, Enums.TransactionType.A, dataSet);
	}

	public HttpResult SaveAdvSource(DataTable dt)
	{
		processType = "SaveAdvSource";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oAdvSource, -1, Enums.TransactionType.A, dataSet);
	}

	public HttpResult SaveQualificationsGroup(DataTable dt)
	{
		processType = "SaveQualificationsGroup";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oQualificationsGroup, -1, Enums.TransactionType.A, dataSet);
	}

	public HttpResult SaveQualifications(DataTable dt)
	{
		processType = "SaveQualifications";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oQualifications, -1, Enums.TransactionType.A, dataSet);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		string text = "";
		try
		{
			switch (processType)
			{
			case "SaveJobType":
			{
				CreateTempTable(document.Tables[0], Temp_JobType);
				BulkCopy(document.Tables[0], Temp_JobType);
				text = "APZ_SaveJobTypes";
				object obj2 = ExecuteScalar(text, CommandType.StoredProcedure);
				objId = Function.ToString(obj2);
				objNum = objId;
				break;
			}
			case "SaveAdvSource":
			{
				CreateTempTable(document.Tables[0], Temp_AdvSource);
				BulkCopy(document.Tables[0], Temp_AdvSource);
				text = "APZ_SaveAdvertisementSource";
				object obj = ExecuteScalar(text, CommandType.StoredProcedure);
				objId = Function.ToString(obj);
				objNum = objId;
				break;
			}
			case "SaveQualifications":
			{
				CreateTempTable(document.Tables[0], Temp_Qualifications);
				BulkCopy(document.Tables[0], Temp_Qualifications);
				text = "APZ_SaveQualifications";
				DataTable dataTable = ExecuteDataTable(text, CommandType.StoredProcedure);
				objId = Function.ToString(Function.ToString(dataTable.Rows[0][1]));
				objNum = objId;
				break;
			}
			case "SaveQualificationsGroup":
			{
				CreateTempTable(document.Tables[0], Temp_QualificationsGroup);
				BulkCopy(document.Tables[0], Temp_QualificationsGroup);
				text = "APZ_SaveQualificationsGroup";
				int num = ExecuteNonQuery(text, CommandType.StoredProcedure);
				objId = Function.ToString(num);
				objNum = objId;
				break;
			}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
