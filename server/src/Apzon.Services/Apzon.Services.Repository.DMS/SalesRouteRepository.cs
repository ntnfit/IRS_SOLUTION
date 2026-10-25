using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

public class SalesRouteRepository : BaseService<DataSet>, ISalesRoute
{
	private const string TempTableDoc = "#APZ_D_OSTR";

	private const string TempTableDetail = "#APZ_D_STR1";

	private const string TempTableEmp = "#APZ_D_STR4";

	private const string TempTableOutlet = "#APZ_D_STR2";

	private const string TempTableDistributor = "#APZ_D_STR3";

	private const string TableDoc = "APZ_D_OSTR";

	private const string TableDetail = "APZ_D_STR1";

	private const string TableEmp = "APZ_D_STR4";

	private const string TableOutlet = "APZ_D_STR2";

	private const string TableDistributor = "APZ_D_STR3";

	public SalesRouteRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDataDoc(string routeCode, string routeName, string status)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@routeCode", Function.ToString(routeCode)));
			list.Add(new SqlParameter("@routeName", Function.ToString(routeName)));
			list.Add(new SqlParameter("@status", Function.ToString(status)));
			return ExecuteDataTable("APZ_D_SalesRoute_GetData", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataDetails(string routeCode)
	{
		try
		{
			return ExecuteDataTable("APZ_D_SalesRoute_GetDetail", CommandType.StoredProcedure, new SqlParameter("@routeCode", Function.ToString(routeCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataOutlets(int id, string routeCode, int empId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@id", Function.ParseInt(id)));
			list.Add(new SqlParameter("@routeCode", Function.ToString(routeCode)));
			list.Add(new SqlParameter("@empId", Function.ParseInt(empId)));
			string query = "select a.\"Id\", a.\"RouteCode\", a.\"EmpId\", a.\"CardCode\", c.\"CardName\", c.\"CardType\", c.\"Address\", a.\"Remarks\" \r\n                                from APZ_D_STR2 a\r\n                                inner join APZ_D_STR1 b on a.\"Id\" = b.\"Id\" and a.\"EmpId\" = b.\"Code\" and b.\"Type\" = 'E'\r\n                                inner join OCRD c on a.\"CardCode\" = c.\"CardCode\" \r\n                                where a.\"Id\" = @id and a.\"RouteCode\" = @routeCode and a.\"EmpId\" = @empId;";
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetListDistributors(int id, string routeCode, int empId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@id", Function.ParseInt(id)));
			list.Add(new SqlParameter("@routeCode", Function.ToString(routeCode)));
			list.Add(new SqlParameter("@empId", Function.ParseInt(empId)));
			return ExecuteDataTable("APZ_D_SalesRoute_GetDistributors", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBpMasterData(string type, string cardCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@type", Function.ToString(type)));
			list.Add(new SqlParameter("@cardCode", Function.ToString(cardCode)));
			string query = "select \"CardCode\", \"CardName\", \"CardType\", \"Address\" from OCRD where \"U_SubType\" = @type and \"CardCode\" = @cardCode;";
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetRecord(string position, string routeCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@position", Function.ToString(position)));
			list.Add(new SqlParameter("@routeCode", Function.ToString(routeCode)));
			return ExecuteDataTable("APZ_D_SalesRoute_GetRecord", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveData(DataSet ds, string type)
	{
		try
		{
			if (type == "A")
			{
				return Process(ApzObjectTypes.oSalesRoute, -1, Enums.TransactionType.A, ds);
			}
			return Process(ApzObjectTypes.oSalesRoute, -1, Enums.TransactionType.U, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		try
		{
			objId = Function.ToString(document.Tables[0].Rows[0]["RouteCode"]);
			BulkToTempTable(document.Tables[0], "APZ_D_OSTR", "#APZ_D_OSTR");
			BulkToTempTable(document.Tables[1], "APZ_D_STR1", "#APZ_D_STR1");
			BulkToTempTable(document.Tables[2], "APZ_D_STR4", "#APZ_D_STR4");
			BulkToTempTable(document.Tables[3], "APZ_D_STR2", "#APZ_D_STR2");
			BulkToTempTable(document.Tables[4], "APZ_D_STR3", "#APZ_D_STR3");
			DataTable dataTable = ExecuteDataTable("APZ_D_Process_SalesRoute", CommandType.StoredProcedure, new SqlParameter("@type", (transtype == Enums.TransactionType.A) ? "A" : "U"));
			if (dataTable != null && dataTable.Rows.Count > 0 && Function.ParseInt(dataTable.Rows[0][0]) != 0)
			{
				return new HttpResult(Enums.MessageCode.Error, 20600004 + " - " + Function.ToString(dataTable.Rows[0]["ErrorMessage"]));
			}
			return new HttpResult(Enums.MessageCode.Success, "", objId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Remove(string routeCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_D_SalesRoute_Remove", CommandType.StoredProcedure, new SqlParameter("@routeCode", Function.ToString(routeCode)));
			if (dataTable.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][0])))
			{
				return new HttpResult(Enums.MessageCode.Error, 20600004 + " - " + Function.ToString(dataTable.Rows[0][0]));
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetEmployees(string routeCode)
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM APZ_D_STR4 WHERE \"RouteCode\"=@RouteCode;", CommandType.Text, new SqlParameter("@RouteCode", routeCode ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetOutlets(string routeCode)
	{
		try
		{
			return ExecuteDataTable("SELECT T0.*,T1.\"CardName\",T1.\"Address\" FROM APZ_D_STR2 T0 INNER JOIN OCRD T1 ON T0.\"CardCode\"=T1.\"CardCode\" WHERE \"RouteCode\"=@RouteCode;", CommandType.Text, new SqlParameter("@RouteCode", routeCode ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDistributors(string routeCode)
	{
		try
		{
			return ExecuteDataTable("SELECT T0.*,T1.\"CardName\",T1.\"Address\" FROM APZ_D_STR3 T0 INNER JOIN OCRD T1 ON T0.\"CardCode\"=T1.\"CardCode\" WHERE \"RouteCode\"=@RouteCode;", CommandType.Text, new SqlParameter("@RouteCode", routeCode ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
