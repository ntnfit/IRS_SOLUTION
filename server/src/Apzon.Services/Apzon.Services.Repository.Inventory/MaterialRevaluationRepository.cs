using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Inventory;
using Apzon.Services.Interfaces.Inventory;

namespace Apzon.Services.Repository.Inventory;

public class MaterialRevaluationRepository : BaseService<MaterialRevaluation>, IMaterialRevaluation, IService<MaterialRevaluation>
{
	public MaterialRevaluationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string Insert(MaterialRevaluation entity)
	{
		throw new NotImplementedException();
	}

	public string Update(MaterialRevaluation entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(MaterialRevaluation entity)
	{
		throw new NotImplementedException();
	}

	public MaterialRevaluation GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable LoadCBPeriod()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "USP_PRODUCTION_COSTING_PEROID";
			result = ExecuteDataTable(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable CostingTable(string period)
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("USP_PRODUCTION_COSTING_ALLOC_ITM", CommandType.StoredProcedure, new SqlParameter("@period", period));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable LastDate(string period)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select T_RefDate from OFPR where Code= '" + period + "'";
			return result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return result;
		}
	}

	public DataTable Costing(string period)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "USP_PRODUCTION_COSTING";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@period", period));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable Cogs(string period)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "USP_PRODUCTION_COSTING_COGS";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@period", period));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable KCSX(string period)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "USP_PRODUCTION_COSTING_62TO154";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@period", period));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable CostingAllocate(string period)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "USP_PRODUCTION_COSTING_ALLOC";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@period", period));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public bool CheckRevaluation(string period)
	{
		DataTable dataTable = new DataTable();
		_ = string.Empty;
		try
		{
			string query = "if exists(select DocEntry from OMRV WHERE U_PeriodCost= '" + period + "') select 'Y' as OK else select 'N' OK ";
			dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null)
			{
				if (dataTable.Rows[0]["OK"].ToString() == "N")
				{
					return false;
				}
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public bool CheckCogs(string period)
	{
		DataTable dataTable = new DataTable();
		_ = string.Empty;
		try
		{
			string query = "if exists(select TransId from OJDT WHERE U_PeriodCogs='" + period + "')   select 'Y' as OK  else select 'N' OK";
			dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null)
			{
				if (dataTable.Rows[0]["OK"].ToString() == "N")
				{
					return false;
				}
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public bool CheckProd(string period)
	{
		DataTable dataTable = new DataTable();
		_ = string.Empty;
		try
		{
			string query = "if exists(select TransId from OJDT WHERE U_PeriodProd='" + period + "') select 'Y' as OK    else select 'N' OK";
			dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null)
			{
				if (dataTable.Rows[0]["OK"].ToString() == "N")
				{
					return false;
				}
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public string Add(MaterialRevaluation doc)
	{
		throw new NotImplementedException();
	}

	public string AddDt(MaterialRevaluation doc, DataTable dt)
	{
		throw new NotImplementedException();
	}

	public string AddPart(MaterialRevaluation doc, List<MaterialRevaluation_linesImpl> list)
	{
		throw new NotImplementedException();
	}

	public DataTable GetAll()
	{
		try
		{
			return ExecuteDataTable("select * from OMRV;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
