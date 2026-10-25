using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.DashBoard;

namespace Apzon.Services.Repository.DashBoard;

public class DashBoardTabsRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IDashBoardTabs
{
	public string TempTableTab = "#APZ_ODTB";

	public string TempTableController = "#APZ_DTB1";

	public DataTable GetAllDashBoardTabs()
	{
		try
		{
			return ExecuteDataTable("SELECT CatId,TabId,Name as TabName,Description as TabDescription,Valid as TabValid,ObjType,TargetType FROM  APZ_ODTB", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetTabByTabId(int tabId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@tabId", tabId));
			return ExecuteDataTable("SELECT CatId,TabId,Name as TabName,Description as TabDescription,Valid as TabValid,ObjType,TargetType FROM  APZ_ODTB WHERE TabId =@tabId", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetAllDashBoardControllers()
	{
		new List<SqlParameter>();
		try
		{
			new DataTable();
			return ExecuteDataTable("SELECT ControllerId,TabId,Code as ControllerCode,Description as ControllerDescription,Remarks,QueryText,QueryType,ColumnKey,ObjectKey,FatherId FROM APZ_DTB1", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetControllersByTab(int tabId)
	{
		List<SqlParameter> list = new List<SqlParameter>();
		try
		{
			list.Add(new SqlParameter("@tabId", tabId));
			return ExecuteDataTable("SELECT a.CatId,a.TabId,a.Name as TabName,a.Description as TabDescription,a.Valid as TabValid,a.ObjType,a.TargetType,b.ControllerId,b.Code as ControllerCode,b.Description as ControllerDescription,b.Remarks,b.QueryText,b.QueryType,b.ColumnKey,b.ObjectKey,b.FatherId FROM  APZ_ODTB a JOIN APZ_DTB1 b ON a.TabId = b.TabId WHERE a.TabId = @tabId", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public HttpResult SaveDoc(DataSet ds)
	{
		return Process(ApzObjectTypes.oDashboardTabs, -1, Enums.TransactionType.U, ds);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document.Tables[0], TempTableTab);
			BulkCopy(document.Tables[0], TempTableTab);
			CreateTempTable(document.Tables[1], TempTableController);
			BulkCopy(document.Tables[1], TempTableController);
			ExecuteDataTable("select * from " + TempTableTab, CommandType.Text);
			ExecuteDataTable("select * from " + TempTableController, CommandType.Text);
			object obj = ExecuteScalar("SP_APZ_ODTB_SAVE", CommandType.Text);
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

	public DataTable GetAllTabsByCategory(int catId)
	{
		List<SqlParameter> list = new List<SqlParameter>();
		try
		{
			list.Add(new SqlParameter("@catId", catId));
			return ExecuteDataTable("SELECT a.CatId,a.TabId,a.Name as TabName,a.Description as TabDescription,a.Valid as TabValid,a.ObjType,a.TargetType,b.Name as CatName,b.Valid as CatValid FROM  APZ_ODTB a JOIN APZ_ODCT b ON a.CatId = b.CatId WHERE a.CatId = @catId", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetMaxControllerId()
	{
		try
		{
			return ExecuteDataTable("SELECT ISNULL( MAX(ControllerId ),0) as ControllerId FROM APZ_DTB1", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetMaxTabId()
	{
		try
		{
			return ExecuteDataTable("SELECT ISNULL( MAX(TabId ),0) as TabId FROM APZ_ODTB", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetTabByTabName(string tabName)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@tabName", tabName));
			return ExecuteDataTable("SELECT CatId,TabId,Name as TabName,Description as TabDescription,Valid as TabValid,ObjType,TargetType FROM  APZ_ODTB WHERE Name Like '%' + @tabName +'%'", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public string DeleteTab(int tabId)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@tabId", tabId));
				if (ExecuteNonQuery("SP_APZ_ODTB_DELTAB", CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				return "Delete error!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}
}
