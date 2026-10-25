using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Services.Interfaces.DashBoard;

namespace Apzon.Services.Repository.DashBoard;

public class DashBoardRepository : BaseService<DataTable>, IDashBoard
{
	public DashBoardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetDataCategories(string userCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@usercode", userCode);
			return ExecuteData("APZ_DashBoard_GetDataCategories", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetDataController(int tabId)
	{
		try
		{
			return ExecuteDataTable("APZ_DashBoard_GetDataController", CommandType.StoredProcedure, new SqlParameter("@tabId", tabId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataQuery(string sql, string type, DataTable dtPara)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (dtPara != null && dtPara.Rows.Count > 0)
			{
				for (int i = 0; i < dtPara.Columns.Count; i++)
				{
					list.Add(new SqlParameter("@" + dtPara.Columns[i].ColumnName, dtPara.Rows[0][i]));
				}
			}
			DataTable result;
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (type == "T")
				{
					result = ExecuteDataTable(sql, CommandType.Text, (list.Count == 0) ? null : list.ToArray());
				}
				else
				{
					result = ((!(type == "S")) ? ExecuteDataTable(sql, CommandType.TableDirect, (list.Count == 0) ? null : list.ToArray()) : ExecuteDataTable(sql, CommandType.StoredProcedure, (list.Count == 0) ? null : list.ToArray()));
				}
				dbTransaction.Rollback();
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDataChidController(int p, DataTable dtPara)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select * from APZ_DTB1 where FatherId = @fatherId", CommandType.Text, new SqlParameter("@fatherId", p));
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return new DataSet();
			}
			List<SqlParameter> list = new List<SqlParameter>();
			if (dtPara != null && dtPara.Rows.Count > 0)
			{
				for (int i = 0; i < dtPara.Columns.Count; i++)
				{
					list.Add(new SqlParameter("@" + dtPara.Columns[i].ColumnName, dtPara.Rows[0][i]));
				}
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string text = Function.ToString(dataTable.Rows[0]["QueryType"]);
				if (!(text == "T"))
				{
					if (text == "S")
					{
						dataSet.Tables.Add(ExecuteDataTable(Function.ToString(dataTable.Rows[0]["QueryText"]), CommandType.StoredProcedure, (list.Count == 0) ? null : list.ToArray()));
					}
					else
					{
						dataSet.Tables.Add(ExecuteDataTable(Function.ToString(dataTable.Rows[0]["QueryText"]), CommandType.TableDirect, (list.Count == 0) ? null : list.ToArray()));
					}
				}
				else
				{
					dataSet.Tables.Add(ExecuteDataTable(Function.ToString(dataTable.Rows[0]["QueryText"]), CommandType.Text, (list.Count == 0) ? null : list.ToArray()));
				}
				dbTransaction.Rollback();
			}
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetDataObj(int tabId)
	{
		try
		{
			return ExecuteDataTable("Select ObjType, TargetType from APZ_ODTB where TabId = @tabId", CommandType.Text, new SqlParameter("@tabId", tabId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetListDashboard()
	{
		try
		{
			return ExecuteDataTable("APZ_Dashboard_GetListDashboard", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
			return new DataTable();
		}
	}

	public List<int> GetListDashboardByUser(string userName)
	{
		try
		{
			return (from r in ExecuteDataTable("APZ_Dashboard_GetListDashboardByUser", CommandType.StoredProcedure, new SqlParameter("@userName", userName)).AsEnumerable()
				select Function.ParseInt(r[0])).ToList();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
			return new List<int>();
		}
	}
}
