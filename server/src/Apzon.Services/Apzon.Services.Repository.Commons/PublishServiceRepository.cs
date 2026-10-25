using System;
using System.Data;
using System.Data.SqlClient;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class PublishServiceRepository : BaseService<DataSet>, IPublishService
{
	private const string TempTableDoc = "#APZ_OCPS";

	private const string TableDoc = "APZ_OCPS";

	private const string TempTableInput = "#APZ_CPS1";

	private const string TableInput = "APZ_CPS1";

	public PublishServiceRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public HttpResult Save(DataSet ds)
	{
		return Process(ApzObjectTypes.oPublishService, -1, Enums.TransactionType.U, ds);
	}

	public DataTable GetByName(string name, string method)
	{
		return ExecuteDataTable("SELECT * FROM APZ_OCPS WHERE \"ControllerName\"=@Name AND \"ActionName\"=@Method;", CommandType.Text, new SqlParameter("@Name", name), new SqlParameter("@Method", method));
	}

	public DataTable GetJsonStructureById(int id)
	{
		return ExecuteDataTable("select * from APZ_CPS1 where \"AbsEntry\"=@id;", CommandType.Text, new SqlParameter("@id", id));
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
		{
			HttpResult result = SaveData(document, ref transtype, ref objId);
			objNum = objId;
			return result;
		}
		case Enums.TransactionType.D:
			if (ExecuteNonQuery(GetDeleteQuery(), CommandType.Text, new SqlParameter("@AbsEntry", Function.ParseInt(objId))) == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "No row affected");
			}
			return new HttpResult(Enums.MessageCode.Success, "", objId);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Unknown error");
		}
	}

	protected virtual string GetDeleteQuery()
	{
		return "DECLARE @ActionEntry int\r\n                                DECLARE @ActionFormId nvarchar(100)\r\n                                DECLARE @ActionItemId nvarchar(100)\r\n                                SET @ActionFormId=isnull((SELECT ActionFormId FROM APZ_OCPS WHERE AbsEntry=@AbsEntry),'')\r\n                                SET @ActionEntry=isnull((SELECT ActionEntry FROM APZ_OCPS WHERE AbsEntry=@AbsEntry),0)\r\n                                SET @ActionItemId=isnull((SELECT ActionItemId FROM APZ_OCPS WHERE AbsEntry=@AbsEntry),'')\r\n\r\n                                DELETE APZ_OCTA WHERE AbsEntry=@ActionEntry and FormId=@ActionFormId and ItemId=@ActionItemId\r\n                                DELETE APZ_OCPS WHERE AbsEntry=@AbsEntry\r\n                                DELETE APZ_CPS1 WHERE AbsEntry=@AbsEntry";
	}

	private HttpResult SaveData(DataSet doc, ref Enums.TransactionType transtype, ref string objId)
	{
		if (doc.IsNotNull())
		{
			transtype = Enums.TransactionType.U;
			BulkToTempTable(doc.Tables["APZ_OCPS"], "APZ_OCPS", "#APZ_OCPS");
			BulkToTempTable(doc.Tables["APZ_CPS1"], "APZ_CPS1", "#APZ_CPS1");
			int num = Function.ParseInt(doc.Tables["APZ_OCPS"].Rows[0]["AbsEntry"]);
			if (num == 0)
			{
				num = Function.ParseInt(ExecuteScalar(SaveDataGetQueryDocEntry(), CommandType.Text));
			}
			objId = num.ToString();
			string query = "apz_publishservice_process";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@transtype", transtype.ToString()));
			if (dataTable.IsNotNull())
			{
				int num2 = Function.ParseInt(dataTable.Rows[0][0]);
				if (num2 > 0)
				{
					string text = Function.ToString(dataTable.Rows[0][1]);
					throw new Exception(num2 + "-" + text);
				}
			}
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	protected virtual string SaveDataGetQueryDocEntry()
	{
		return "SELECT ISNULL((SELECT MAX(AbsEntry) FROM APZ_OCPS),0)+1";
	}

	public DataTable Search(string controllerName, string actionName, string description)
	{
		try
		{
			SqlParameter[] array = new SqlParameter[3];
			SqlParameter sqlParameter = new SqlParameter();
			sqlParameter.ParameterName = "@Controller";
			sqlParameter.SqlDbType = SqlDbType.NVarChar;
			sqlParameter.Value = "%" + controllerName + "%";
			array[0] = sqlParameter;
			SqlParameter sqlParameter2 = new SqlParameter();
			sqlParameter2.ParameterName = "@ActionName";
			sqlParameter2.SqlDbType = SqlDbType.NVarChar;
			sqlParameter2.Value = "%" + actionName + "%";
			array[1] = sqlParameter2;
			SqlParameter sqlParameter3 = new SqlParameter();
			sqlParameter3.ParameterName = "@Description";
			sqlParameter3.SqlDbType = SqlDbType.NVarChar;
			sqlParameter3.Value = "%" + description + "%";
			array[2] = sqlParameter3;
			return ExecuteDataTable("SELECT * FROM APZ_OCPS WHERE \"ControllerName\" LIKE @Controller AND \"ActionName\" LIKE @ActionName AND \"Description\" LIKE @Description;", CommandType.Text, array);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public DataSet GetById(int id)
	{
		try
		{
			return ExecuteData("SELECT * FROM APZ_OCPS WHERE \"AbsEntry\"=@Id;\r\n                                     SELECT * FROM APZ_CPS1 WHERE \"AbsEntry\"=@Id;", CommandType.Text, new SqlParameter("@Id", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataSet();
	}

	public string Navigate(int absEntry, Enums.NavigateType type)
	{
		try
		{
			DataTable dataTable = new DataTable();
			switch (type)
			{
			case Enums.NavigateType.Last:
				dataTable = ExecuteDataTable("SELECT TOP 1 \"AbsEntry\" FROM APZ_OCPS ORDER BY \"AbsEntry\" DESC;", CommandType.Text);
				break;
			case Enums.NavigateType.First:
				dataTable = ExecuteDataTable("SELECT TOP 1 \"AbsEntry\" FROM APZ_OCPS ORDER BY \"AbsEntry\"; ", CommandType.Text);
				break;
			case Enums.NavigateType.Next:
				dataTable = ExecuteDataTable(NavigateGetNextQuery(), CommandType.Text, new SqlParameter("@AbsEntry", absEntry));
				break;
			case Enums.NavigateType.Previous:
				dataTable = ExecuteDataTable(NavigateGetPreviousQuery(), CommandType.Text, new SqlParameter("@AbsEntry", absEntry));
				break;
			}
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0][0]).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return string.Empty;
	}

	protected virtual string NavigateGetNextQuery()
	{
		return "DECLARE @MaxId INT\r\n                                    SET @MaxId=ISNULL((SELECT MAX(AbsEntry) FROM APZ_OCPS),0)\r\n                                    IF(@MaxId=@AbsEntry)\r\n                                    BEGIN\r\n                                        SELECT TOP 1 AbsEntry FROM APZ_OCPS ORDER BY AbsEntry\r\n                                    END\r\n                                    ELSE\r\n                                    BEGIN\r\n                                        SELECT TOP 1 AbsEntry FROM APZ_OCPS WHERE AbsEntry>@AbsEntry ORDER BY AbsEntry\r\n                                    END";
	}

	protected virtual string NavigateGetPreviousQuery()
	{
		return "DECLARE @MinId INT\r\n                                    SET @MinId=ISNULL((SELECT MIN(AbsEntry) FROM APZ_OCPS),0)\r\n                                    IF(@MinId=@AbsEntry)\r\n                                    BEGIN\r\n                                        SELECT TOP 1 AbsEntry FROM APZ_OCPS ORDER BY AbsEntry DESC\r\n                                    END\r\n                                    ELSE\r\n                                    BEGIN\r\n                                        SELECT TOP 1 AbsEntry FROM APZ_OCPS WHERE AbsEntry<@AbsEntry ORDER BY AbsEntry DESC\r\n                                    END";
	}

	public HttpResult Remove(int absEntry)
	{
		try
		{
			return Process(ApzObjectTypes.oPublishService, -1, Enums.TransactionType.D, new DataSet(), absEntry.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
