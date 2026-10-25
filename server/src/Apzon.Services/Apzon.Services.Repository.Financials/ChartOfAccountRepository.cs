using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.MasterData;
using Apzon.Services.Interfaces.Financials;

namespace Apzon.Services.Repository.Financials;

public class ChartOfAccountRepository : BaseService<ChartOfAccountsImpl>, IChartOfAccount, IService<ChartOfAccountsImpl>
{
	public ChartOfAccountRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public List<ChartOfAccountsImpl> Cfl()
	{
		List<ChartOfAccountsImpl> list = new List<ChartOfAccountsImpl>();
		try
		{
			DataTable dataTable = ExecuteDataTable("select \"AcctCode\",\"AcctName\",\"CurrTotal\" from OACT where \"Postable\"='Y';", CommandType.Text);
			if (dataTable != null)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					list.Add(new ChartOfAccountsImpl
					{
						AcctCode = Function.ToString(dataTable.Rows[i]["AcctCode"]),
						AcctName = Function.ToString(dataTable.Rows[i]["AcctName"]),
						Balance = Function.ParseDouble(dataTable.Rows[i]["CurrTotal"])
					});
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}

	public string Insert(ChartOfAccountsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(ChartOfAccountsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(ChartOfAccountsImpl entity)
	{
		throw new NotImplementedException();
	}

	public ChartOfAccountsImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable GetStruct()
	{
		try
		{
			return ExecuteDataTable("SELECT TOP 0 * FROM OACT;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}
}
