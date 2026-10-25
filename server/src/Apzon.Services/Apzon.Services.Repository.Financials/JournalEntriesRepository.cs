using System;
using System.Data;
using System.Data.SqlClient;
using Apzon.Commons;
using Apzon.Entities.Models.Financials;
using Apzon.Services.Interfaces.Financials;

namespace Apzon.Services.Repository.Financials;

public class JournalEntriesRepository : BaseService<JournalEntriesImpl>, IJournalEntries, IService<JournalEntriesImpl>
{
	public JournalEntriesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string Insert(JournalEntriesImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(JournalEntriesImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(JournalEntriesImpl entity)
	{
		throw new NotImplementedException();
	}

	public JournalEntriesImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public int GetMax()
	{
		try
		{
			string query = "SELECT isnull(MAX(TRANSID),0)  MAX FROM OJDT";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null)
			{
				return Convert.ToInt32(dataTable.Rows[0]["MAX"]);
			}
			return 0;
		}
		catch (Exception)
		{
			return 0;
		}
	}

	public DataTable GetDoc(int transId)
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM OJDT WHERE \"TransId\"=@TransId;", CommandType.Text, new SqlParameter("@TransId", transId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public DataTable GetLines(int transId)
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM JDT1 WHERE \"TransId\"=@TransId;", CommandType.Text, new SqlParameter("@TransId", transId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public string Costing(JournalEntriesImpl doc)
	{
		throw new NotImplementedException();
	}

	public string Import(JournalEntriesImpl JeLuong, AccountingSalaryImpl AccountingSalary)
	{
		throw new NotImplementedException();
	}

	public string Cancel(int docEntry)
	{
		throw new NotImplementedException();
	}
}
