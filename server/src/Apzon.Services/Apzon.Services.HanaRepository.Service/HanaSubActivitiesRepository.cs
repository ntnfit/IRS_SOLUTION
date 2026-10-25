using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Repository.Service;

namespace Apzon.Services.HanaRepository.Service;

public class HanaSubActivitiesRepository : SubActivitiesRepository
{
	public HanaSubActivitiesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override string GetUserComment(string objType, int id)
	{
		try
		{
			return Function.ToString(ExecuteScalar("select string_agg(USER_CODE, ';') from OUSR a INNER JOIN (select distinct \"UserSign\" from APZ_CLG1 where \"ObjType\" = @objType and ID = @id) b ON a.\"USERID\" = b.\"UserSign\";", CommandType.Text, new SqlParameter("@id", id), new SqlParameter("@objType", objType)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public override DataTable GetActiveEmployees()
	{
		try
		{
			return ExecuteDataTable("Select Ifnull(\"lastName\",'') || \r\n                            Case When Ifnull(\"middleName\", '') != '' Then ' ' || Ifnull(\"middleName\", '') Else '' End ||\r\n                            Case When Ifnull(\"firstName\",'') != '' Then ', ' || Ifnull(\"firstName\", '') Else '' End as \"empName\" ,ifnull(\"empID\",0) as \"empID\",\"email\",\"userId\"\r\n                                FROM OHEM where \"Active\" = 'Y'; ", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRequesterByEmployee", ex.Message.ToString());
			return new DataTable();
		}
	}
}
