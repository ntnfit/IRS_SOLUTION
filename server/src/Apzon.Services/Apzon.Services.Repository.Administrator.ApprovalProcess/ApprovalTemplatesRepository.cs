using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Administrator.ApprovalProcess;

namespace Apzon.Services.Repository.Administrator.ApprovalProcess;

public class ApprovalTemplatesRepository : BaseService<DataSet>, IApprovalTemplates
{
	public ApprovalTemplatesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetData()
	{
		try
		{
			return ExecuteDataTable("select top 1 * from OWTM;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDataLines(int templateCode)
	{
		try
		{
			return ExecuteData("APZ_ApprovalTemplates_GetDataLine", CommandType.StoredProcedure, new SqlParameter("@templateCode", templateCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	protected virtual string GetSearchDataQuery()
	{
		return "select * from OWTM where (ISNULL(\"Name\", '') = '' or \"Name\" like N'%' + @wtmName + '%') and (ISNULL(\"Remarks\", '') = '' or \"Remarks\" like N'%' + @wtmRemarks + '%')";
	}

	public DataTable SearchData(string templateName, string templateRemarks)
	{
		try
		{
			string searchDataQuery = GetSearchDataQuery();
			return ExecuteDataTable(searchDataQuery, CommandType.Text, new SqlParameter("@wtmName", Function.ToString(templateName)), new SqlParameter("@wtmRemarks", Function.ToString(templateRemarks)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetNewCodeQuery()
	{
		return "select ISNULL(MAX(WtmCode), 0) + 1 from OWTM";
	}

	public int GetNewCode()
	{
		try
		{
			string newCodeQuery = GetNewCodeQuery();
			return Function.ParseInt(ExecuteDataTable(newCodeQuery, CommandType.Text).Rows[0][0]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return -1;
		}
	}

	public DataTable GetUserInfo(string userName)
	{
		try
		{
			string query = "select a.USERID, a.USER_CODE as \"UserCode\", a.U_NAME as \"UserName\", b.\"Code\" as \"DepartmntCode\", b.\"Name\" as \"DepartmntName\" from OUSR a\r\n                                inner join OUDP b on a.\"Department\" = b.\"Code\"\r\n                                where a.U_NAME = @userName;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@userName", Function.ToString(userName)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetRecord(string position, string templateCode)
	{
		try
		{
			return ExecuteDataTable("APZ_ApprovalTemplates_GetRecord", CommandType.StoredProcedure, new SqlParameter("@position", position), new SqlParameter("@templateCode", templateCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetApprovalStageInfo(string stageName)
	{
		try
		{
			return ExecuteDataTable("select * from OWST where \"Name\" = @stageName;", CommandType.Text, new SqlParameter("@stageName", Function.ToString(stageName)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public int GetQueryId(string queryName)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select \"IntrnalKey\" from OUQR where \"QName\" = @queryName;", CommandType.Text, new SqlParameter("@queryName", Function.ToString(queryName)));
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return -1;
		}
	}
}
