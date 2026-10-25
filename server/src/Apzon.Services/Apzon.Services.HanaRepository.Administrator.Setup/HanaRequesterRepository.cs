using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaRequesterRepository : RequesterRepository
{
	public HanaRequesterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override DataTable GetRequesterByUser()
	{
		try
		{
			string query = "select ifnull(a.USER_CODE,'') as \"Requester\" ,ifnull(a.U_NAME,'') as \"ReqName\" ,a.\"Department\", ifnull(b.\"Name\",'') as \"DepartmentName\",a.\"Branch\",ifnull(c.\"Name\" ,'') as \"BranchName\", ifnull(a.\"userSign\",0) as \"UserSign\"\r\n                            from OUSR a left join OUDP b ON a.\"Department\" = b.\"Code\" \r\n                            left join OUBR c ON c.\"Code\" = a.\"Branch\"; ";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRequesterByUser", ex.Message.ToString());
			return new DataTable();
		}
	}

	public override DataTable GetRequesterByEmployee()
	{
		try
		{
			string query = "select ifnull(\"lastName\",'') as \"LastName\",ifnull (\"firstName\",'') as \"FirstName\",  \"lastName\" || ',' || \"firstName\" as \"ReqName\" ,ifnull(\"empID\",0) as \"EmpID\" , ifnull(\"UserSign\",0) as \"UserSign\", \"dept\", \"branch\" FROM OHEM;";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRequesterByEmployee", ex.Message.ToString());
			return new DataTable();
		}
	}

	public override DataTable GetUser_tableOUSR()
	{
		try
		{
			string query = "select ifnull(USERID,0) as USERID, ifnull(U_NAME,'') as U_NAME  from OUSR;";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetUser_tableOUSR", ex.Message.ToString());
			return new DataTable();
		}
	}

	protected override string GetAllUser_tableOUSRQuery()
	{
		return "select ifnull( USERID,0) as USERID, ifnull( U_NAME,'') as U_NAME  from OUSR \r\n                            Union All\r\n                            select 0,'All' from dummy;";
	}
}
