using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.MasterData;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class AcctCodeRepository : BaseService<AcctCodeImpl>, IAcctCode, IService<AcctCodeImpl>
{
	public AcctCodeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public List<AcctCodeImpl> Cfl(string accountType)
	{
		List<AcctCodeImpl> list = new List<AcctCodeImpl>();
		try
		{
			accountType = (string.IsNullOrWhiteSpace(accountType) ? "N" : "Y");
			string query = "SELECT \"AcctCode\", \"AcctName\", \"CurrTotal\" FROM OACT WHERE \"Postable\" = 'Y' AND \"Finanse\" = '" + accountType + "'; ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					list.Add(new AcctCodeImpl
					{
						AcctCode = Function.ToString(dataTable.Rows[i]["AcctCode"]),
						AcctName = Function.ToString(dataTable.Rows[i]["AcctName"]),
						CurrTotal = Function.ParseDouble(dataTable.Rows[i]["CurrTotal"])
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

	public string Insert(AcctCodeImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(AcctCodeImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(AcctCodeImpl entity)
	{
		throw new NotImplementedException();
	}

	public AcctCodeImpl GetById(int id)
	{
		throw new NotImplementedException();
	}
}
