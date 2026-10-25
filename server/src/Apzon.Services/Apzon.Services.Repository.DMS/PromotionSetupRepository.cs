using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

public class PromotionSetupRepository : BaseService<DataSet>, IPromotionSetup
{
	public PromotionSetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetData()
	{
		try
		{
			string query = "select * from APZ_D_OPRC;\r\n                            select * from APZ_D_OPRT;\r\n                            select * from APZ_D_OPRG;";
			return ExecuteData(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public HttpResult SaveData(DataSet ds)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(ds.Tables[0], "#APZ_D_OPRC");
				BulkCopy(ds.Tables[0], "#APZ_D_OPRC");
				CreateTempTable(ds.Tables[1], "#APZ_D_OPRT");
				BulkCopy(ds.Tables[1], "#APZ_D_OPRT");
				CreateTempTable(ds.Tables[2], "#APZ_D_OPRG");
				BulkCopy(ds.Tables[2], "#APZ_D_OPRG");
				DataTable dataTable = ExecuteDataTable("APZ_PromotionSetup_Save", CommandType.StoredProcedure);
				if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
				{
					dbTransaction.Commit();
				}
				return new HttpResult
				{
					Message = Function.ToString(dataTable.Rows[0][1]),
					Content = dataTable.Rows[0][0]
				};
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string CheckUsed(string type, int id)
	{
		try
		{
			string query = "APZ_PromotionSetup_CheckUsed";
			SqlParameter[] parameters = new SqlParameter[2]
			{
				new SqlParameter("@type", type),
				new SqlParameter("@id", id)
			};
			return Function.ToString(ExecuteDataTable(query, CommandType.StoredProcedure, parameters).Rows[0][0]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
