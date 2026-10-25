using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources;

namespace Apzon.Services.Repository.HumanResources;

public class HREducationRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IHREducation
{
	private string TempTableTab = "#dtOHED";

	public DataTable LoadDoc()
	{
		try
		{
			string query = "select * from OHED;";
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

	public DataTable GetAutoKey(string ObjCode)
	{
		try
		{
			string query = "SELECT \"AutoKey\" FROM ONNM WHERE \"ObjectCode\"=@ObjCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ObjCode", ObjCode));
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

	public DataTable GetManagerName()
	{
		new DataTable();
		try
		{
			string managerNameQuery = GetManagerNameQuery();
			return ExecuteDataTable(managerNameQuery, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetManagerNameQuery()
	{
		return "select a.manager,\r\n\t\t\t\t\t\t\tcase\r\n\t\t\t\t\t\t\twhen LTRIM(RTRIM(firstname))='' OR firstname is null then   isnull(firstname,'')\r\n\t\t\t\t\t\t\telse\r\n\t\t\t\t\t\t\tcase \r\n\t\t\t\t\t\t\twhen LTRIM(RTRIM(lastname)) ='' OR lastname IS null  then isnull(lastname,'')\r\n\t\t\t\t\t\t\telse\r\n\t\t\t\t\t\t\tisnull(firstname,'')+ ', '+ isnull(lastname,'')\r\n\t\t\t\t\t\t\tend\r\n\t\t\t\t\t\t\tend\r\n\t\t\t\t\t\t\tmanagerName\r\n\t\t\t\t\t\t\tfrom hem3 a left join ohem b on a.manager=b.empid";
	}

	public DataTable CheckEducationType(int edType)
	{
		try
		{
			string query = "SELECT * FROM HEM2 WHERE \"type\"=@edType;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@edType", edType));
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

	public HttpResult Save(DataTable dtDoc)
	{
		try
		{
			if (dtDoc.IsNotNull())
			{
				int num = dtDoc.AsEnumerable().Max((DataRow s) => s.Field<int>("edType")) + 1;
				string query = "UPDATE ONNM SET \"AutoKey\"=@AutoKey WHERE \"ObjectCode\"=@ObjCode;";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@AutoKey", num));
				list.Add(new SqlParameter("@ObjCode", "175"));
				ExecuteDataTable(query, CommandType.Text, list.ToArray());
			}
			return Process(ApzObjectTypes.oEducationTypes, -1, Enums.TransactionType.A, dtDoc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		string text = "";
		try
		{
			int num = Function.ParseInt(document.AsEnumerable().Max((DataRow t) => t["edType"]));
			for (int num2 = 0; num2 < document.Rows.Count; num2++)
			{
				if (Function.ParseInt(document.Rows[num2]["edType"]) == 0)
				{
					document.Rows[num2]["edType"] = num + 1;
					num++;
				}
			}
			CreateTempTable(document, TempTableTab);
			BulkCopy(document, TempTableTab);
			text = "APZ_SaveEducationType";
			DataTable dataTable = ExecuteDataTable(text, CommandType.StoredProcedure, (SqlParameter[])null);
			objId = Function.ToString(dataTable);
			objNum = objId;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public int GetMaxID()
	{
		try
		{
			return Function.ParseInt(ExecuteScalar(GetMaxIDQuery(), CommandType.Text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return -1;
		}
	}

	protected virtual string GetMaxIDQuery()
	{
		return "select isnull(max(empID), 0) from OHEM";
	}
}
