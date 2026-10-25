using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.MasterData;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class RequesterRepository : BaseService<RequesterImpl>, IRequester, IService<RequesterImpl>
{
	public RequesterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public List<RequesterImpl> Cfl()
	{
		List<RequesterImpl> list = new List<RequesterImpl>();
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT \"Requester\", \"ReqName\" FROM OPRQ;", CommandType.Text);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list.Add(new RequesterImpl
				{
					Requester = Function.ToString(dataTable.Rows[i]["Requester"]),
					ReqName = Function.ToString(dataTable.Rows[i]["ReqName"])
				});
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListRequester", ex.Message.ToString());
		}
		return list;
	}

	public virtual DataTable GetRequesterByUser()
	{
		try
		{
			string query = "select isnull( a.USER_CODE,0) as Requester ,isnull( a.U_NAME,'') as ReqName ,a.Department, isnull(b.Name,'') as DepartmentName,a.Branch,isnull(c.Name ,'') as BranchName, ISNULL(a.UserSign,0) as UserSign\r\n                            from OUSR a left join OUDP b ON a.Department = b.Code \r\n                            left join OUBR c ON c.Code = a.Branch ";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRequesterByUser", ex.Message.ToString());
			return new DataTable();
		}
	}

	public virtual DataTable GetRequesterByEmployee()
	{
		try
		{
			string query = "select isnull(lastName,'') as LastName ,isnull (firstName,'') as FirstName,  lastName + ',' + firstName as ReqName ,isnull( empID,0) as EmpID , isnull (UserSign,0) as UserSign, dept, branch FROM OHEM  ";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRequesterByEmployee", ex.Message.ToString());
			return new DataTable();
		}
	}

	public string Insert(RequesterImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(RequesterImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(RequesterImpl entity)
	{
		throw new NotImplementedException();
	}

	public RequesterImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public virtual DataTable GetUser_tableOUSR()
	{
		try
		{
			string query = "select isnull(USERID,0) as USERID, isnull( U_NAME,'') as U_NAME  from OUSR;";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetUser_tableOUSR", ex.Message.ToString());
			return new DataTable();
		}
	}

	public DataTable GetAllUser_tableOUSR()
	{
		try
		{
			string allUser_tableOUSRQuery = GetAllUser_tableOUSRQuery();
			new DataTable();
			return ExecuteDataTable(allUser_tableOUSRQuery, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetAllUser_tableOUSR", ex.Message.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetAllUser_tableOUSRQuery()
	{
		return "select isnull( USERID,0) as USERID, isnull( U_NAME,'') as U_NAME  from OUSR \r\n                            Union All\r\n                            select 0,'All'";
	}
}
