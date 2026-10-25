using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class WeightRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IWeight
{
	private DataTable dt = new DataTable();

	private string TempTableTab = "#dtOWGT";

	public HttpResult Save(DataTable dtObj)
	{
		return Process(ApzObjectTypes.oWeightMeasures, -1, Enums.TransactionType.A, dtObj);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		try
		{
			objId = "";
			objNum = "";
			CreateTempTable(dt, TempTableTab);
			BulkCopy(dt, TempTableTab);
			object obj = ExecuteScalar("DELETE dbo.OWGT\r\n                                     INSERT INTO dbo.OWGT\r\n                                     SELECT * FROM #dtOWGT\r\n                                     UPDATE ONNM SET AutoKey = (Select MAX(UnitCode) FROM OWGT) +1 WHERE ObjectCode = '51'", CommandType.Text);
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

	public DataTable GetData()
	{
		try
		{
			string query = "SELECT * FROM OWGT;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Weight Search - Doc ", ex.Message);
			return new DataTable();
		}
	}

	public string RemoveWeights(List<string> lstWeight)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			string result = "";
			try
			{
				string query = "Delete from OWGT where \"UnitCode\" in (select data in Split (@param,',');\r\n                              UPDATE ONNM SET \"AutoKey\" = (Select MAX(\"UnitCode\") FROM OWGT) +1 WHERE \"ObjectCode\" = '51')";
				string value = string.Join(",", lstWeight);
				ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@param", value));
				dbTransaction.Commit();
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				dbTransaction.Rollback();
				result = ex.Message;
			}
			return result;
		}
	}

	public int GetMax()
	{
		try
		{
			string maxQuery = GetMaxQuery();
			DataTable dataTable = ExecuteDataTable(maxQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["UnitCode"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	protected virtual string GetMaxQuery()
	{
		return "SELECT MAX(isnull(UnitCode,0)) as UnitCode FROM  OWGT";
	}

	public bool Check(string key)
	{
		try
		{
			string query = "SELECT   A.\"UnitCode\" \r\n                            FROM OWGT A \r\n                            WHERE A.\"UnitCode\" = @key; ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@key", key));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Check data Weight ", ex.Message);
			return true;
		}
	}

	public DataTable CheckUnitDisply(string key)
	{
		DataTable result = new DataTable();
		try
		{
			string query = " SELECT \"UnitCode\", IFNULL(\"UnitDisply\", '') AS \"UnitDisply\", IFNULL(\"UnitName\", '') AS \"UnitName\", IFNULL(\"WightInMG\", 0) AS \"WightInMG\", IFNULL(\"Locked\", 'N') AS \"Locked\", IFNULL(\"DataSource\", 'I') AS \"DataSource\", \"UserSign\" FROM OWGT WHERE \"UnitDisply\" = @key; ";
			return result = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@key", key));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckUnitDisply ", ex.Message);
			return result;
		}
	}
}
