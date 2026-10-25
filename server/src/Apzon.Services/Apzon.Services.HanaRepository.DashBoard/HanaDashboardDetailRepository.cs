using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Repository.DashBoard;

namespace Apzon.Services.HanaRepository.DashBoard;

public class HanaDashboardDetailRepository : DashboardDetailRepository
{
	public HanaDashboardDetailRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override DataTable GetById(int id)
	{
		try
		{
			return ExecuteDataTable("SELECT T0.*,T1.\"Name\" AS \"Category\" FROM  APZ_DAB1 T0 LEFT JOIN APZ_ODAB T1 ON T0.\"CatId\"=T1.\"Id\" WHERE T0.\"Id\"=@Id \r\n                                                        AND IFNULL(\"MarkAsRemoved\",'N')<>'Y';", CommandType.Text, new SqlParameter("@Id", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public override DataTable LoadAll()
	{
		try
		{
			return ExecuteDataTable("SELECT T0.*,T1.\"Name\" AS \"Category\" FROM  APZ_DAB1 T0 INNER JOIN APZ_ODAB T1 ON T0.\"CatId\"=T1.\"Id\" and IFNULL(\"MarkAsRemoved\",'N')<>'Y';", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected override string GetNewIdQuery()
	{
		return "SELECT IFNULL(MAX(\"Id\"), 0) + 1 FROM APZ_DAB1;";
	}
}
