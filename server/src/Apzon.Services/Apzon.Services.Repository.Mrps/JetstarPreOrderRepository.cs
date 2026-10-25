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
using Apzon.Services.Interfaces.Mrps;

namespace Apzon.Services.Repository.Mrps;

public class JetstarPreOrderRepository : BaseService<DataTable>, IJetstarPreOrder
{
	private const string TableTemp = "#tempPreOrder";

	public JetstarPreOrderRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable Search(string type, string searchData)
	{
		try
		{
			return ExecuteDataTable("APZ_JetstarPreOrder_Search", CommandType.StoredProcedure, new SqlParameter("@searchDate", Function.ParseDateTime(searchData, "yyyyMMdd")), new SqlParameter("@type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable Run(string type)
	{
		try
		{
			return ExecuteDataTable("APZ_JetstarPreOrder_Run", CommandType.StoredProcedure, new SqlParameter("@type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public HttpResult SaveData(DataTable dt, Enums.TransactionType tranType)
	{
		try
		{
			return Process(ApzObjectTypes.oNone, -1, tranType, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		DataTable tableStruct = GetTableStruct("APZ_OJPO");
		if (!tableStruct.IsNotNull())
		{
			throw new Exception("Could not get data structure");
		}
		IEnumerable<string> source = from DataColumn t in tableStruct.Columns
			select t.ColumnName;
		DataTable dt = document.DefaultView.ToTable(distinct: false, source.ToArray());
		CreateTempTable(dt, "#tempPreOrder");
		BulkCopy(dt, "#tempPreOrder");
		DataTable dataTable = ExecuteDataTable("APZ_JetstarPreOrder_Save", CommandType.StoredProcedure, new SqlParameter("@tranType", Function.ToString(transtype)));
		if (!dataTable.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Can not update data");
		}
		if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
		{
			return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable.Rows[0][1]));
		}
		return new HttpResult(Enums.MessageCode.Success, "", dataTable.Rows[0][0]);
	}

	public DataSet GetOpenPoByMrpCode(int mrpCode)
	{
		return ExecuteData("APZ_JetstarPreOrder_GetOpenPoByMrpCode", CommandType.StoredProcedure, new SqlParameter("@mrpCode", mrpCode));
	}

	public DataTable GetBp()
	{
		try
		{
			return ExecuteDataTable("APZ_JetstarPreOrder_GetBp", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
