using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup.Purchasing;

namespace Apzon.Services.Repository.Administrator.Setup.Purchasing;

public class LandedCostsSetupRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), ILandedCostsSetup
{
	private string processType = "";

	private string Temp_OALC = "#OALC";

	public DataTable GetLandedCostsSetup()
	{
		try
		{
			string query = "select * from OALC;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string RemoveLandedCostsSetup(string code)
	{
		string empty = string.Empty;
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				SqlParameter sqlParameter = new SqlParameter("@code", code);
				if (ExecuteNonQuery("Delete from OALC where \"AlcCode\" NOT IN (select \"AlcCode\" from IPF2) and \"AlcCode\" = @code;", CommandType.Text, sqlParameter) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "Cannot remove code because it has been used in Landed Costs";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public HttpResult SaveLandedCostsSetup(DataTable dt)
	{
		processType = "SaveLandedCostsSetup";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oTimeSheeting, -1, Enums.TransactionType.A, dataSet);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		string text = "";
		try
		{
			if (processType == "SaveLandedCostsSetup")
			{
				CreateTempTable(document.Tables[0], Temp_OALC);
				BulkCopy(document.Tables[0], Temp_OALC);
				text = "APZ_SaveLandedCosts";
				object obj = ExecuteScalar(text, CommandType.StoredProcedure);
				objId = Function.ToString(obj);
				objNum = objId;
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
