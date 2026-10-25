using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.BusinessPartner;

namespace Apzon.Services.Repository.Administrator.Business_Partner;

public class CountriesRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), ICountries
{
	private string TempTableTab = "#dtOCRY";

	public HttpResult Save(DataTable dtDoc)
	{
		return Process(ApzObjectTypes.oCountries, -1, Enums.TransactionType.A, dtDoc);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTableTab);
			BulkCopy(document, TempTableTab);
			object obj = ExecuteScalar("DELETE dbo.OCRY\r\n                                     INSERT INTO dbo.OCRY\r\n                                     SELECT * FROM #dtOCRY ", CommandType.Text);
			objId = Function.ToString(obj);
			objNum = objId;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable LoadOADF()
	{
		try
		{
			string query = "SELECT \"Code\",\"Name\" FROM OADF;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetOCRY()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT * FROM OCRY ORDER BY \"Code\";", CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable CheckExistCode(string code)
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM OCRY WHERE \"Code\" = @code;", CommandType.Text, new SqlParameter("@code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
