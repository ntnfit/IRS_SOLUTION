using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Administrator.Setup.Inventory;

namespace Apzon.Services.Repository.Administrator.Setup.Inventory;

public class WHSubSetGenPreRepository : BaseService<IWHSubSetGenPre>, IWHSubSetGenPre
{
	public WHSubSetGenPreRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetAllWarehouseSublevel()
	{
		try
		{
			string query = "SELECT DISTINCT c.DispName,s.FldAbs from OBSL s join OBFC c on s.FldAbs = c.AbsEntry";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetWarehouseSublevelByFldAbs(long abs)
	{
		try
		{
			string query = "SELECT c.DispName,s.SLCode,s.Descr,s.FldAbs from OBSL s join OBFC c on s.FldAbs = c.AbsEntry WHERE s.FldAbs = @FldAbs";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@FldAbs", abs));
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAllBinLocations()
	{
		try
		{
			string query = "SELECT s.SLCode,b.SL1Code,b.SL2Code,b.SL3Code,b.SL4Code from OBSL s join OBFC c on s.FldAbs = c.AbsEntry join OBIN b on s.AbsEntry = b.SL1Abs";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
