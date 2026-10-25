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

public class JVInterviewRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IJVInterview
{
	private string processType = "";

	private string TempTableTab = "#dtAPZ_OHIS";

	private string _table = "";

	public DataTable LoadDoc(int empID)
	{
		try
		{
			string query = "select a.*,b.empID,b.firstName,b.lastName, b.middleName,c.code as typeName from APZ_OHEC a INNER JOIN OHEM b ON a.empID=b.empID LEFT JOIN APZ_OCTY c ON a.[type]=c.typeID WHERE b.empID=@empID";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@empID", empID));
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

	public DataTable CheckInterviewStatus(int ID)
	{
		try
		{
			string query = "SELECT * FROM APZ_HJV3 WHERE \"InterviewStatus\" = @ID;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ID", ID));
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

	public DataTable CheckInterviewTypes(int ID)
	{
		try
		{
			string query = "SELECT * FROM APZ_HJV4 WHERE InterviewType=@ID";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ID", ID));
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

	public virtual string ProcessInterviewStatusQuery(string tblName)
	{
		return string.Format("declare @i int  = (SELECT ISNULL(MAX(Id),0) FROM APZ_OHIS) \r\n\t                               UPDATE {0} SET Id  = @i ,@i = @i + 1\r\n\t                               where Id = 0 OR Id is null\r\n                                   DELETE APZ_OHIS\r\n                                   INSERT INTO APZ_OHIS\r\n                                   SELECT * FROM {0}", tblName);
	}

	public virtual string ProcessInterviewTypeQuery(string tblName)
	{
		return string.Format("declare @i int  = (SELECT ISNULL(MAX(Id),0) FROM APZ_OHIT) \r\n\t                                UPDATE {0} SET Id  = @i ,@i = @i + 1\r\n\t                                where Id = 0 OR Id is null\r\n                                     DELETE APZ_OHIT\r\n                                     INSERT INTO APZ_OHIT\r\n                                     SELECT * FROM {0}", tblName);
	}

	public DataTable LoadInterviewStatus()
	{
		try
		{
			string query = "select * from APZ_OHIS order by \"Id\";";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
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

	public DataTable LoadInterviewTypes()
	{
		try
		{
			string query = "select * from APZ_OHIT;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
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

	public HttpResult SaveData(Tuple<DataTable, string> dt)
	{
		ApzObjectTypes objtype = ApzObjectTypes.oInterviewStatus;
		if (dt.Item2 == "APZ_OHIS")
		{
			TempTableTab = "#DTAPZ_OHIS";
			processType = "InterviewStatus";
			objtype = ApzObjectTypes.oInterviewStatus;
		}
		if (dt.Item2 == "APZ_OHIT")
		{
			TempTableTab = "#dtAPZ_OHIT";
			processType = "InterviewTypes";
			objtype = ApzObjectTypes.oInterviewTypes;
		}
		return Process(objtype, -1, Enums.TransactionType.A, dt.Item1);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		string text = "";
		object obj = null;
		try
		{
			string text2 = processType;
			if (!(text2 == "InterviewStatus"))
			{
				if (text2 == "InterviewTypes")
				{
					CreateTempTable(document, TempTableTab);
					BulkCopy(document, TempTableTab);
					text = ProcessInterviewTypeQuery(TempTableTab);
					obj = ExecuteNonQuery(text, CommandType.Text);
				}
			}
			else
			{
				CreateTempTable(document, TempTableTab);
				BulkCopy(document, TempTableTab);
				text = ProcessInterviewStatusQuery(TempTableTab);
				obj = ExecuteNonQuery(text, CommandType.Text);
			}
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
