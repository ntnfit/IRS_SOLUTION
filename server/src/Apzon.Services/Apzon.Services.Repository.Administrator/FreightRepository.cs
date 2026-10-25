using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class FreightRepository : BaseService<IFreight>, IFreight
{
	public FreightRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetFreight()
	{
		try
		{
			string query = "APZ_Freight_GetAll";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
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

	public DataTable GetMaxExpnsCode()
	{
		try
		{
			string query = "APZ_Freight_GetMax";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
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

	public DataTable GetOutputTaxGroup()
	{
		try
		{
			string query = "APZ_Freight_GetVATGO";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
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

	public DataTable GetVatGroupI()
	{
		try
		{
			string query = "APZ_Freight_GetVATGI";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
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

	public string DeleteFreight(int ExpnsCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@ExpnsCode", ExpnsCode);
			string query = "APZ_Freight_Delete";
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, sqlParameter) > 0)
			{
				return null;
			}
			return "Delete Error";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string UpdateFreight(DataTable dt)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			string empty = string.Empty;
			try
			{
				SqlParameter sqlParameter = new SqlParameter("@dt", dt);
				empty = "APZ_Freight_Save";
				if (ExecuteNonQuery(empty, CommandType.StoredProcedure, sqlParameter) > 0)
				{
					dbTransaction.Commit();
					return null;
				}
				return "Can't Update!";
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}
}
