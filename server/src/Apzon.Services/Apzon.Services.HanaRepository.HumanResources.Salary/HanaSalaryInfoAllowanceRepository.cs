using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.HanaRepository.HumanResources.Salary;

public class HanaSalaryInfoAllowanceRepository : SalaryInfoAllowanceRepository
{
	public HanaSalaryInfoAllowanceRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override DataTable GetMaxLineNum(int empId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@empID", empId)
			};
			return ExecuteDataTable("Select ifnull(Max(\"LineNum\"),0) as \"MaxLineNum\" from APZ_HSI1 where \"EmpId\" = @empID;", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetLineNum  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
