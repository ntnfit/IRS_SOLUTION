using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Integrated;

namespace Apzon.Services.Repository.Integrated;

public class IntegratedBPRepository : BaseService<DataTable>, IIntegratedBP
{
	public IntegratedBPRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet Search(DataTable doc)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "APZ_Integrated_BP_Search";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CardCode", Function.ToString(doc.Rows[0]["CardCode"])));
			list.Add(new SqlParameter("@CardName", Function.ToString(doc.Rows[0]["CardName"])));
			list.Add(new SqlParameter("@CardType", Function.ToString(doc.Rows[0]["CardType"])));
			list.Add(new SqlParameter("@IsImported", Function.ToString(doc.Rows[0]["IsImported"])));
			result = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetContactEmps(string cardCode)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "APZ_Integrated_BP_GetContactEmps";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CardCode", cardCode));
			result = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetAddresses(string cardCode)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "APZ_Integrated_BP_GetAddresses";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CardCode", cardCode));
			result = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable CheckExists(DataTable dt)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_Integrated_BP_CheckExists";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CardCode", Function.ToString(dt.Rows[0]["CardCode"])));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}
}
