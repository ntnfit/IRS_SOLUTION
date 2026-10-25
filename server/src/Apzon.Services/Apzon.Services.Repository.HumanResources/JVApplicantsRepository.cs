using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources;

namespace Apzon.Services.Repository.HumanResources;

public class JVApplicantsRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IJVApplicants
{
	private string TempTableTab = "#dtAPZ_OHAP";

	public DataSet LoadDoc(int id, string appNo)
	{
		try
		{
			string text = "";
			if (id > 0)
			{
				text = " WHERE \"ApplicantID\"=" + id + ";";
			}
			else if (!string.IsNullOrEmpty(appNo))
			{
				text = " WHERE \"ApplicantNo\"=" + appNo + ";";
			}
			if (string.IsNullOrEmpty(text))
			{
				text = ";";
			}
			string query = string.Format("SELECT * FROM APZ_OHAP{0}\r\n                                 SELECT a.\"ApplicantID\", b.\"QuaCode\",b.\"Name\" FROM APZ_OHAQ a INNER JOIN APZ_OHQU b  ON a.\"QuaCode\"=b.\"QuaCode\" {0}", text);
			DataSet dataSet = ExecuteData(query, CommandType.Text, (SqlParameter[])null);
			if (dataSet.Tables.Count > 0)
			{
				return dataSet;
			}
			return new DataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public HttpResult Save(DataSet ds)
	{
		TempTableTab = "#dtAPZ_OHAP";
		return SaveTable(ds.Tables[0]);
	}

	private HttpResult SaveTable(DataTable dt)
	{
		return Process(ApzObjectTypes.oApplicants, -1, Enums.TransactionType.A, dt);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			string tempTableTab = TempTableTab;
			if (!(tempTableTab == "#dtAPZ_OHAP"))
			{
				if (!(tempTableTab == "#dtAPZ_OHAQ"))
				{
				}
				return new HttpResult();
			}
			CreateTempTable(document, TempTableTab);
			BulkCopy(document, TempTableTab);
			object obj = ExecuteScalar("APZ_JVApplicant_Save", CommandType.StoredProcedure, (SqlParameter[])null);
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

	public DataTable CheckApplicant(string id)
	{
		try
		{
			string query = "SELECT ApplicantNo FROM APZ_HJV5   WHERE ApplicantNo IN ( SELECT ApplicantNo from APZ_OHAP WHERE ApplicantID=@id)\r\n                                    UNION ALL\r\n                                   SELECT b.Data FROM APZ_HJV4 a CROSS APPLY dbo.Split(a.Applicants,';') b WHERE b.Data IN  ( SELECT ApplicantNo from APZ_OHAP WHERE ApplicantID=@id)";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@id", id));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public virtual string GetMaxIdApplicantQuery()
	{
		return "select isnull(max(ApplicantID),0) as ApplicantID from APZ_OHAP";
	}

	public int GetMaxIdApplicant()
	{
		try
		{
			string maxIdApplicantQuery = GetMaxIdApplicantQuery();
			return Function.ParseInt(ExecuteDataTable(maxIdApplicantQuery, CommandType.Text).Rows[0]["ApplicantID"]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxIdApplicant FAILED ", ex.Message);
			return -1;
		}
	}
}
