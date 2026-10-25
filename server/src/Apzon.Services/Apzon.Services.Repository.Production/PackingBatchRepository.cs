using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Production;

namespace Apzon.Services.Repository.Production;

public class PackingBatchRepository : BaseService<DataTable>, IPackingBatch
{
	public PackingBatchRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetBatch(string batchNo)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@BatchNo", batchNo);
			return ExecuteData("Apz_PackingBatch_Get", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetIssue(int id)
	{
		try
		{
			if (id > 0)
			{
				SqlParameter sqlParameter = new SqlParameter("@docentry", id);
				return ExecuteData("Apz_PackingBatch_GetIssue", CommandType.StoredProcedure, sqlParameter);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetReceipt(int id)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@docentry", id);
			return ExecuteData("Apz_PackingBatch_GetReceipt", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public int GetPage(int current, int type, string username, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Current", current));
			list.Add(new SqlParameter("@Type", type));
			if (!string.IsNullOrEmpty(username))
			{
				list.Add(new SqlParameter("@Username", username));
			}
			if (branch != -1)
			{
				list.Add(new SqlParameter("@Branch", branch));
			}
			DataTable dataTable = ExecuteDataTable("Apz_PackingBatch_GetPage", CommandType.StoredProcedure, list.ToArray());
			if (dataTable.IsNotNull() && dataTable.IsExistsColumn("DocEntry"))
			{
				return Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	public DataSetResult GetDataByBarcode(string barcode, string boxCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@barcode", barcode),
				new SqlParameter("@boxCode", boxCode)
			};
			return new DataSetResult("", ExecuteData("Apz_PackingBox_GetDataByBarcode", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}
}
