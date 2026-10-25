using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Repository.Production;

namespace Apzon.Services.HanaRepository.Production;

public class HanaOutsourcingRepository : OutsourcingRepository
{
	public HanaOutsourcingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override int GetRecord(string position, int code)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_Outsourcing_GetRecord", CommandType.StoredProcedure, new SqlParameter("@Pos", position), new SqlParameter("@Code", code));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public override bool ExistsCode(int code)
	{
		try
		{
			string query = "SELECT 1 FROM \"@TBL_OUTS_DOCNUM\" WHERE \"Code\" = @code;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@code", code));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}
}
