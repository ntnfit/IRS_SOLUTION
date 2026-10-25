using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Util;

namespace Apzon.Services.Repository.Util;

public class AdvancePeriodClosingRepository : BaseService<DataTable>, IAdvancePeriodClosing
{
	public AdvancePeriodClosingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetPeriod()
	{
		try
		{
			return ExecuteDataTable("SELECT AbsEntry,Code,Name,F_RefDate,T_RefDate,F_DueDate,T_DueDate,F_TaxDate,T_TaxDate FROM OFPR WHERE PeriodStat='N'", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
