using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

public class OutletVisitingFrequencyRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IOutletVisitingFrequency
{
	private string Temp_APZ_D_OOVF = "#APZ_D_OOVF";

	public DataTable GetData(string userName, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@userName", userName),
				new SqlParameter("@branch", branch)
			};
			return ExecuteDataTable("APZ_D_OutletVisitingFrequency_GetData", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oOutletVisitingFrequency, -1, Enums.TransactionType.U, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dtSave, ref string objId, ref string objNum)
	{
		return SaveData(dtSave, transtype, ref objId, ref objNum);
	}

	public HttpResult SaveData(DataTable dt, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			DataTable tableStruct = GetTableStruct("APZ_D_OOVF");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			HttpResult httpResult = new HttpResult();
			if (!dt.IsNotNull())
			{
				httpResult.Message = "No data to save";
			}
			else
			{
				IEnumerable<string> source = from DataColumn t in tableStruct.Columns
					select t.ColumnName;
				dt = dt.DefaultView.ToTable(distinct: false, source.ToArray());
				CreateTempTable(dt, Temp_APZ_D_OOVF);
				BulkCopy(dt, Temp_APZ_D_OOVF);
				object obj = ExecuteScalar("Apz_D_OutletVisitingFrequency_SaveProcess", CommandType.StoredProcedure);
				objId = Function.ToString(obj);
				objNum = objId;
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
