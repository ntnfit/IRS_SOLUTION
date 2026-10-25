using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.DashBoard;

namespace Apzon.Services.Repository.DashBoard;

public class MobileDashboardRepository : BaseService<DataTable>, IMobileDashboard
{
	public MobileDashboardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetCustomerDashboard(string username)
	{
		try
		{
			string query = "Apz_MobileDashboard_GetCustomerDashboard";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@userName", username));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet GetCustomerDashboardDetail(string username, string dashboardItem)
	{
		try
		{
			string query = "Apz_MobileDashboard_GetCustomerDashboardDetail";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@userName", username));
			list.Add(new SqlParameter("@dashboardItem", dashboardItem));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetFormStructure(string cardCode)
	{
		try
		{
			string query = "Apz_MobileDashboard_GetFormStructure";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardCode", cardCode));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetCustomerAvailableProducts(string cardCode)
	{
		try
		{
			string query = "Apz_MobileDashboard_GetCustomerAvailableProducts";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardCode", cardCode));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string AddProductToFavorite(string cardCode, string itemCode)
	{
		try
		{
			string query = "Apz_MobileDashboard_AddProductToFavorite";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardCode", cardCode));
			list.Add(new SqlParameter("@itemCode", itemCode));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) == 0)
			{
				return "Add Error!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}

	public string RemoveProductFromFavorite(string cardCode, string itemCode)
	{
		try
		{
			string query = "Apz_MobileDashboard_RemoveProductFromFavorite";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardCode", cardCode));
			list.Add(new SqlParameter("@itemCode", itemCode));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) == 0)
			{
				return "Remove Error!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}

	public DataTable GetAPISetting(string key)
	{
		try
		{
			string query = "Apz_MobileDashboard_GetAPISetting";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@key", key));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public int GetBPBranch(string username)
	{
		try
		{
			string query = "Apz_MobileDashboard_GetBPBranch";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@userName", username));
			return Function.ParseInt(ExecuteScalar(query, CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	public string GetLastProductUpdatedDate()
	{
		try
		{
			string query = "Apz_MobileDashboard_LastProductUpdatedDate";
			return Function.ToString(ExecuteScalar(query, CommandType.StoredProcedure));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public DataSet GetOrderRequest(int docentry, string dashboardItem = "")
	{
		try
		{
			string query = "Apz_MobileDashboard_GetOrderRequest";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docEntry", docentry));
			list.Add(new SqlParameter("@dashboardItem", dashboardItem));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string ConfirmOrder(string OrderId)
	{
		try
		{
			string query = "Apz_MobileDashboard_ConfirmOrder";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docentry", OrderId));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) == 0)
			{
				return "Confirm Error!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}

	public DataTable GetCustomerOrders(string cardCode)
	{
		try
		{
			string query = "Apz_MobileDashboard_GetCustomerOrders";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardCode", cardCode));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetServiceUserDeliveryOrder(string username, string status)
	{
		try
		{
			string query = "Apz_MobileDashboard_GetServiceUserDeliveryOrder";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@userName", username));
			list.Add(new SqlParameter("@status", status));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet GetServiceUserDeliOrdersDetail(string docentry)
	{
		try
		{
			string query = "Apz_MobileDashboard_GetServiceUserDeliOrdersDetail";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docentry", docentry));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string UpdateDeliveryOrderStatus(string docentry, string status)
	{
		try
		{
			string query = "Apz_MobileDashboard_UpdateDeliveryOrderStatus";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@status", status));
			list.Add(new SqlParameter("@docentry", docentry));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) == 0)
			{
				return "Update Error!";
			}
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
		return "";
	}
}
