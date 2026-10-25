using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Service;

namespace Apzon.Services.Repository.Service;

public class ServiceCallsRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IServiceCalls
{
	private const string TempServiceCall = "#dtAPZ_OSCL";

	private string TempSolution = "#dtSCL1";

	private string TempDocumentLine = "#dtAPZ_SCL3";

	public DataTable GetServiceCalls(string cardCode, int handleBy, int callType, int callId, string tag, DateTime? fromDate, DateTime? toDate)
	{
		try
		{
			string query = "APZ_ServiceCalls_GetServiceCalls";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CardCode", Function.ToString(cardCode)));
			list.Add(new SqlParameter("@HandleBy", handleBy));
			list.Add(new SqlParameter("@CallType", callType));
			list.Add(new SqlParameter("@CallId", callId));
			list.Add((fromDate != DateTime.MinValue) ? new SqlParameter("@FromDate", fromDate) : new SqlParameter("@FromDate", DBNull.Value));
			list.Add((toDate != DateTime.MinValue) ? new SqlParameter("@ToDate", toDate) : new SqlParameter("@ToDate", DBNull.Value));
			list.Add(new SqlParameter("@Tag", tag));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetContactPerson(string cardCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@CardCode", (!string.IsNullOrEmpty(cardCode)) ? cardCode : "");
			return ExecuteDataTable("SELECT \"CntctCode\",\"Name\",\"CardCode\",\"Tel1\",\"E_MailL\" from OCPR where \"CardCode\" = @CardCode OR @CardCode='';", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetSolutionByCallId(int callId)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@callId", callId);
			return ExecuteDataTable("select  a.\"srvcCallID\",a.\"line\",a.\"solutionID\", a.\"userSign\",a.\"userSign2\", a.\"createDate\", a.\"updateDate\", b.\"Subject\", b.\"StatusNum\"\r\n\r\n                     from scl1 a INNER JOIN oslt b ON a.\"solutionID\"=b.\"SltCode\" where a.\"srvcCallID\" = @callId;", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDocumentByCallId(int callId)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@callId", callId);
			return ExecuteDataTable("select * from APZ_SCL3 where \"srvcCallID\" = @callId;", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string RemoveDoc(int callId)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@CallId", callId));
				if (ExecuteNonQuery("APZ_ServiceCalls_RemoveDoc", CommandType.StoredProcedure, list.ToArray()) <= 0)
				{
					return "Error!";
				}
				dbTransaction.Commit();
				return string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveDoc(DataSet dt)
	{
		return Process(ApzObjectTypes.oServiceCallsManagement, -1, Enums.TransactionType.A, dt).Message;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		string text = "";
		object obj = null;
		try
		{
			CreateTempTable(document.Tables[0], "#dtAPZ_OSCL");
			BulkCopy(document.Tables[0], "#dtAPZ_OSCL");
			if (document.Tables.Count > 1 && document.Tables[1].IsNotNull())
			{
				CreateTempTable(document.Tables[1], TempSolution);
				BulkCopy(document.Tables[1], TempSolution);
			}
			if (document.Tables.Count > 2 && document.Tables[2].IsNotNull())
			{
				CreateTempTable(document.Tables[2], TempDocumentLine);
				BulkCopy(document.Tables[2], TempDocumentLine);
			}
			text = "APZ_ServiceCalls_SaveDoc";
			obj = ExecuteScalar(text, CommandType.StoredProcedure);
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
}
