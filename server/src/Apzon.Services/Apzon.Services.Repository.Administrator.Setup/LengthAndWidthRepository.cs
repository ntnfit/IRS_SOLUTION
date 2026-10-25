using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class LengthAndWidthRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), ILengthAndWidth
{
	private DataTable dt = new DataTable();

	private string TempTableTab = "#dtOLGT";

	public DataTable GetData()
	{
		try
		{
			string query = "SELECT * FROM OLGT;";
			dt = ExecuteDataTable(query, CommandType.Text);
			if (dt != null && dt.Rows.Count > 0)
			{
				return dt;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMax  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataTable dt)
	{
		return Process(ApzObjectTypes.oLengthMeasures, -1, Enums.TransactionType.A, dt);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		try
		{
			objId = "";
			objNum = "";
			CreateTempTable(dt, TempTableTab);
			BulkCopy(dt, TempTableTab);
			object obj = ExecuteScalar("DELETE dbo.OLGT\r\n                                     INSERT INTO dbo.OLGT\r\n                                     SELECT * FROM #dtOLGT \r\n                                     UPDATE ONNM SET AutoKey = (Select MAX(UnitCode) FROM OLGT) +1 WHERE ObjectCode = '50'", CommandType.Text);
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

	public bool CheckData(string key)
	{
		try
		{
			string query = "select top 1 * from OLGT where \"UnitDisply\" = @key; ";
			dt = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@key", key));
			if (dt != null && dt.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckData  FAILED ", ex.Message);
		}
		return false;
	}

	public int GetMax()
	{
		try
		{
			string maxQuery = GetMaxQuery();
			dt = ExecuteDataTable(maxQuery, CommandType.Text, (SqlParameter[])null);
			if (dt != null && dt.Rows.Count > 0)
			{
				return Function.ParseInt(dt.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMax  FAILED ", ex.Message);
			return -1;
		}
	}

	protected virtual string GetMaxQuery()
	{
		return "SELECT MAX (ISNULL(UnitCode,0)) as UnitCode from OLGT";
	}

	public DataTable CheckExistUnitCode(int unitCode)
	{
		try
		{
			string query = "SELECT \"SVolUnit\" FROM OITM WHERE \"SVolUnit\"=@SVolUnit;";
			SqlParameter sqlParameter = new SqlParameter("@SVolUnit", unitCode);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckExistItemGroupCode  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Remove(DataTable dt)
	{
		try
		{
			string query = "delete from OLGT where \"UnitCode\"=@untCode;";
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				foreach (DataRow row in dt.Rows)
				{
					if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@untCode", row["UnitCode"])) < 0)
					{
						dbTransaction.Rollback();
						return new HttpResult(Enums.MessageCode.Error, "Delete Failed");
					}
				}
				dbTransaction.Commit();
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DELETE  FAILED ", ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
