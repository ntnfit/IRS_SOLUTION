using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class ProcurementConfirmRepository : Repository<DataTable>, IProcurementConfirm, IRepository<DataTable>, IDisposable
{
	public ProcurementConfirmRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public DataSet LoadPreferredVendor(string baseType, string userName, string branch)
	{
		try
		{
			string query = "Apz_ProcurementConfirm_LoadPreferredVendor";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(baseType))
			{
				list.Add(new SqlParameter("@BaseType", baseType));
			}
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet LoadItemLine(string baseType, string docEntry, string userName, string branch)
	{
		try
		{
			string query = "Apz_ProcurementConfirm_GetLine";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(baseType))
			{
				list.Add(new SqlParameter("@BaseType", baseType));
			}
			if (!string.IsNullOrEmpty(baseType))
			{
				list.Add(new SqlParameter("@DocEntry", docEntry));
			}
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet LoadSalesOrderLine(string docEntry, string userName, string branch)
	{
		try
		{
			string query = "Apz_ProcurementConfirm_LoadSalesOrderLine";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			if (!string.IsNullOrEmpty(docEntry))
			{
				list.Add(new SqlParameter("@docEntry", docEntry));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
