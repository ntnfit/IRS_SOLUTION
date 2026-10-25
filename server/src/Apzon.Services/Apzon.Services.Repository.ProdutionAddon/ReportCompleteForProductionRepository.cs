using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.ProductionAddon;

namespace Apzon.Services.Repository.ProdutionAddon;

public class ReportCompleteForProductionRepository : BaseService<IReportCompleteForProduction>, IReportCompleteForProduction
{
	public ReportCompleteForProductionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDocsByBarCode(string batIssue)
	{
		try
		{
			string query = "APZ_ProductionAddon_CheckBarCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@batIssue", batIssue));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocsByBarCode  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
