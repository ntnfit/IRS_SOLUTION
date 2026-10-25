using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.PackingManagement;

namespace Apzon.Services.Repository.PackingManagement;

public class TraceabilityRepository : BaseService<DataSet>, ITraceability
{
	private const string TableTempCase = "#DMCT";

	private const string TableTempItem = "#DMIT";

	public TraceabilityRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetCase()
	{
		try
		{
			return ExecuteDataTable("APZ_PackingManagement_GetCase", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetCaseTracibility()
	{
		try
		{
			return ExecuteDataTable("APZ_PackingManagement_GetCaseforTracibility", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetUser()
	{
		try
		{
			return ExecuteDataTable("APZ_PackingManagement_GetUser", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetLastLineNum()
	{
		try
		{
			return ExecuteDataTable("APZ_PackingManagement_selectMaxLinenum", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable CaseGetDataDoc(string CaseCode, Enums.NavigateType type)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CaseCode", CaseCode));
			list.Add(new SqlParameter("@type", type));
			return ExecuteDataTable("APZ_PackingManagement_CaseGetDataDoc", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetItemByLineNumber(int LineNumber)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@LineNumber", LineNumber));
			return ExecuteDataTable("APZ_PackingManagement_GetItemByIdCase", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string Update(DataSet ds)
	{
		try
		{
			Process(ApzObjectTypes.oNone, -1, Enums.TransactionType.U, ds);
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		try
		{
			CreateTempTable(ds.Tables["dtCase"], "#DMCT");
			BulkCopy(ds.Tables["dtCase"], "#DMCT");
			CreateTempTable(ds.Tables["dtItem"], "#DMIT");
			BulkCopy(ds.Tables["dtItem"], "#DMIT");
			return new HttpResult(Enums.MessageCode.Success, "", ExecuteDataTable("APZ_PackingManagement_UpdateTraceability", CommandType.StoredProcedure, (SqlParameter[])null));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable Search(string CaseCode, string TypeCode, DateTime PostingDate)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CaseCode", CaseCode));
			list.Add(new SqlParameter("@TypeCode", TypeCode));
			list.Add(new SqlParameter("@PostingDate", PostingDate));
			return ExecuteDataTable("APZ_PackingManagement_SearchCase", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
