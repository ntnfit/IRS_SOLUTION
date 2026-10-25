using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class IrsConfigRepository : BaseService<DataTable>, IIrsConfig
{
	public IrsConfigRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetTableStructure(string tblName)
	{
		try
		{
			string query = "Select * from IRS_DATA_MAPPING";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
