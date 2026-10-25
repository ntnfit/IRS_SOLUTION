using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Retail;

namespace Apzon.Services.Repository.Retail;

public class RetailStoreRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IRetailStore
{
	public string TempTable = "#APZ_STORE";

	public DataTable LoadDoc(string code)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@StoreCode", Function.ToString(code)));
			return ExecuteDataTable("APZ_RetailStore_GetDoc", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable LoadAllDoc()
	{
		try
		{
			return ExecuteDataTable("Select * from APZ_STORE;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string Save(DataTable dt)
	{
		return Process(ApzObjectTypes.oRetailStore, -1, Enums.TransactionType.U, dt).Message;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTable);
			BulkCopy(document, TempTable);
			DataTable dataTable = ExecuteDataTable("SP_APZ_STORE_SAVE", CommandType.StoredProcedure, (SqlParameter[])null);
			objId = Function.ToString(dataTable);
			objNum = objId;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string RemoveStore(string code)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@Code", code));
				DataTable dataTable = ExecuteDataTable("SP_APZ_STORE_REMOVE", CommandType.StoredProcedure, list.ToArray());
				string result;
				if (dataTable == null || dataTable.Rows.Count == 0)
				{
					dbTransaction.Commit();
					result = "";
				}
				else
				{
					result = Function.ToString(dataTable.Rows[0][0]);
				}
				return result;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}
}
