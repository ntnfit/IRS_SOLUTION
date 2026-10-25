using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class UoMGroupDefinitionRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IUoMGroupDefinition
{
	public string TempTable = "#UGP1";

	public DataTable LoadDoc(int ugpEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@UgpEntry", Function.ToString(ugpEntry))
			};
			return ExecuteDataTable("APZ_UoMGroup_GetDefinition", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Doc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataTable dt)
	{
		return Process(ApzObjectTypes.oUoMDefGroups, -1, Enums.TransactionType.U, dt);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTable);
			BulkCopy(document, TempTable);
			object obj = ExecuteScalar("SP_APZ_UGP1_SAVE", CommandType.StoredProcedure);
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
}
