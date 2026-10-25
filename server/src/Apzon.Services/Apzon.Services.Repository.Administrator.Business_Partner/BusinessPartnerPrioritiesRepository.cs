using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Business_Partners;

namespace Apzon.Services.Repository.Administrator.Business_Partner;

public class BusinessPartnerPrioritiesRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IBusinessPrioritiesSetup
{
	private string TempTableTab = "#dtOBPP";

	public DataTable LoadDoc()
	{
		try
		{
			return ExecuteDataTable("SELECT *, \"PrioCode\" \"OldPrioCode\" FROM OBPP ORDER BY \"PrioCode\" ASC;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataTable dtDoc)
	{
		return Process(ApzObjectTypes.oBusinessPartnerPriorities, -1, Enums.TransactionType.A, dtDoc);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTableTab);
			BulkCopy(document, TempTableTab);
			object obj = ExecuteScalar("DELETE OBPP\r\n                                     INSERT INTO OBPP\r\n                                     SELECT * FROM #dtOBPP", CommandType.Text);
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

	public DataTable CheckExistBPP(int prioCode)
	{
		try
		{
			return ExecuteDataTable("SELECT \"CardCode\" FROM OCRD where \"Priority\"=@prioCode;", CommandType.Text, new SqlParameter("@prioCode", prioCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Remove(DataTable dt)
	{
		try
		{
			string query = "delete from OBPP where \"PrioCode\"=@PrioCode;";
			new List<SqlParameter>();
			foreach (DataRow row in dt.Rows)
			{
				if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@PrioCode", Function.ParseInt(row["PrioCode"]))) <= 0)
				{
					return new HttpResult(Enums.MessageCode.Error, "Delete Failed");
				}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
