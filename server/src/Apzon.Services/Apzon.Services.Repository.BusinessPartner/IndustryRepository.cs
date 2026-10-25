using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.BusinessPartner;

namespace Apzon.Services.Repository.BusinessPartner;

public class IndustryRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IIndustry
{
	private string TableTemp = "#APZ_INDUST";

	public DataTable LoadData()
	{
		try
		{
			return ExecuteDataTable("Select * from OOND", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string SaveDoc(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oIndustries, -1, Enums.TransactionType.A, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			switch (transtype)
			{
			case Enums.TransactionType.A:
				CreateTempTable(dt, TableTemp);
				BulkCopy(dt, TableTemp);
				ExecuteDataTable("APZ_Industry_SaveDoc", CommandType.StoredProcedure);
				return new HttpResult();
			case Enums.TransactionType.D:
				ExecuteNonQuery("APZ_Industry_DeleteDoc", CommandType.StoredProcedure);
				return new HttpResult();
			default:
				return new HttpResult(Enums.MessageCode.Error, "Wrong transtype");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string DeleteDoc(int indCode)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("IndCode", typeof(int));
			dataTable.AcceptChanges();
			return Process(ApzObjectTypes.oIndustries, -1, Enums.TransactionType.D, dataTable).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public bool ExistsIndCode(int indCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select 1 from OOND where indCode = @indCode", CommandType.Text, new SqlParameter("@indCode", indCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}
}
