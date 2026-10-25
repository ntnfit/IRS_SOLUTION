using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.Production_Addon;
using Apzon.Services.Interfaces.ProductionAddon;

namespace Apzon.Services.Repository.ProdutionAddon;

public class ProductionControlRepository : BaseService<IProductionControl>, IProductionControl
{
	public ProductionControlRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetDocs(string cardcode, string cardname, string fromdate, string todate)
	{
		try
		{
			string query = "APZ_ProductionAddon_ProductionControl_GetDocs";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CustomerCode", cardcode));
			list.Add(new SqlParameter("@CustomerName", cardname));
			list.Add(new SqlParameter("@FromDate", fromdate));
			list.Add(new SqlParameter("@ToDate", todate));
			DataSet dataSet = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				return dataSet;
			}
			return new DataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetDocsWhenProPlan(string ids)
	{
		try
		{
			string query = "APZ_ProductionControl_GetDocsWhenProPlan";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ids", ids));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable Search(ProductionControlImpl obj)
	{
		try
		{
			string query = "APZ_ProductionContriol_Search";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardcode", obj.CardCode));
			list.Add(new SqlParameter("@cardname", obj.CardName));
			list.Add(new SqlParameter("@fromdate", obj.FromDate));
			list.Add(new SqlParameter("@todate", obj.ToDate));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDocsTabPlaned(ProductionOder_Impl_OWOR po)
	{
		try
		{
			string query = "APZ_ProductionControl_GetStatusProduction";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Type", po.Type));
			list.Add(new SqlParameter("@Status", po.Status));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
