using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.ProductionAddon;

namespace Apzon.Services.Repository.ProdutionAddon;

public class MasterOrderReplanRepository : BaseService<DataTable>, IMasterOrderReplan
{
	public MasterOrderReplanRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable Search(string moNo, DateTime fromDate, DateTime toDate)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_MasterOrderReplan_Search";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@masterOrderNo", moNo));
			list.Add(new SqlParameter("@fromDate", fromDate));
			list.Add(new SqlParameter("@toDate", toDate));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data MO Search", ex.Message);
		}
		return result;
	}

	public string Save(DataTable dt)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			List<SqlParameter> list = new List<SqlParameter>();
			try
			{
				list.Add(new SqlParameter("@dt", dt));
				if (ExecuteNonQuery("APZ_MasterOrderReplan_Save", CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "Error!";
			}
			catch (Exception ex)
			{
				dbTransaction.Rollback();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save Stage  FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}
}
