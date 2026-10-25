using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Administrator;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class SalesPersonsRepository : BaseService<SalesPersonsImpl>, ISalesPersons, IService<SalesPersonsImpl>
{
	public SalesPersonsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public List<SalesPersonsImpl> Cfl()
	{
		List<SalesPersonsImpl> list = new List<SalesPersonsImpl>();
		try
		{
			DataTable dataTable = ExecuteDataTable("select \"SlpCode\", \"SlpName\" from OSLP;", CommandType.Text);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list.Add(new SalesPersonsImpl
				{
					SalesEmployeeCode = Function.ParseInt(dataTable.Rows[i]["SlpCode"]),
					SalesEmployeeName = Function.ToString(dataTable.Rows[i]["SlpName"])
				});
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}

	public string Insert(SalesPersonsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(SalesPersonsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(SalesPersonsImpl entity)
	{
		throw new NotImplementedException();
	}

	public SalesPersonsImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable GetStructure()
	{
		try
		{
			return ExecuteDataTable("select top 0 * from OSLP;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}
}
