using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Mrps;
using Apzon.Services.Interfaces.Mrps;

namespace Apzon.Services.Repository.Mrps;

public class SalesForeCastRepository : BaseService<SalesForeCastImpl>, ISalesForeCast
{
	public SalesForeCastRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDocs()
	{
		try
		{
			string query = "SELECT AbsID,Code,Name,UserSign,StartDate,EndDate,FormView FROM dbo.OFCT";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public int GetMaxAbsID()
	{
		try
		{
			string query = "APZ_SalesForeCast_GetMaxAbsID";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxAbsID FAILED ", ex.Message);
			return 0;
		}
	}

	public DataTable GetYearForQuickAnalysis()
	{
		try
		{
			string query = "APZ_SalesforeCast_GetYear";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxAbsID FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDoc(int id)
	{
		return new DataTable();
	}

	public DataTable Search(DataTable dts)
	{
		try
		{
			DataRow dataRow = dts.Rows[0];
			string query = "APZ_SalesForeCast_Search";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@BPCode", Function.ToString(dataRow["BPCode"])));
			list.Add(new SqlParameter("@ForecastType", Function.ToString(dataRow["ForecastType"])));
			list.Add(new SqlParameter("@Year", Function.ToString(dataRow["Year"])));
			list.Add(new SqlParameter("@BPName", Function.ToString(dataRow["BPName"])));
			list.Add(new SqlParameter("@Employee", Function.ParseInt(dataRow["Employee"])));
			list.Add(new SqlParameter("@Whse", Function.ToString(dataRow["Whse"])));
			list.Add(new SqlParameter("@Division", Function.ToString(dataRow["Division"])));
			list.Add(new SqlParameter("@Status", Function.ToString(dataRow["Status"])));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetQuickAnalysis(DataTable dts)
	{
		try
		{
			DataRow dataRow = dts.Rows[0];
			string query = "APZ_SalesForeCast_Analysis";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@BPCode", Function.ToString(dataRow["BPCode"])));
			list.Add(new SqlParameter("@ForecastType", Function.ToString(dataRow["ForecastType"])));
			list.Add(new SqlParameter("@Year", Function.ToString(dataRow["Year"])));
			list.Add(new SqlParameter("@BPName", Function.ToString(dataRow["BPName"])));
			list.Add(new SqlParameter("@Employee", Function.ToString(dataRow["Employee"])));
			list.Add(new SqlParameter("@Whse", Function.ToString(dataRow["Whse"])));
			list.Add(new SqlParameter("@Division", Function.ToString(dataRow["Division"])));
			list.Add(new SqlParameter("@Status", Function.ToString(dataRow["Status"])));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDetail(Tuple<int, DateTime, DateTime> tup)
	{
		try
		{
			return ExecuteDataTable("apz_forecast_get_row", CommandType.StoredProcedure, new SqlParameter("@absId", tup.Item1));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetInfor FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetInfor(int AbsID)
	{
		try
		{
			string query = "APZ_SalesForeCast_GetDay";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@AbsID", AbsID)
			};
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetInfor FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string SaveDocs(DataSet ds)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			List<SqlParameter> list = new List<SqlParameter>();
			try
			{
				string query = "APZ_SalesForeCast_SaveData";
				list.Add(new SqlParameter("@dt", ds.Tables[0]));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					if (ds.Tables[1].Rows.Count > 0)
					{
						if (!string.IsNullOrEmpty(Savedoc(ds.Tables[1])))
						{
							dbTransaction.Rollback();
							return "ERROR";
						}
						dbTransaction.Commit();
						return "";
					}
					return "";
				}
				dbTransaction.Rollback();
				return "ERROR";
			}
			catch (Exception ex)
			{
				dbTransaction.Rollback();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save Stage  FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}

	public string Savedoc(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				string query = "APZ_SalesForeCast_SaveChild";
				list.Add(new SqlParameter("@dt", dt));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "ERROR";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save Stage  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteAllDocByAbsID(string ids)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "APZ_SalesForCast_DeleteAllDoc";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@ids", ids));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "No rows deleted";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDocs  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDocs(string ids)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "APZ_SalesForCast_DeleteDocs";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@ids", ids));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					DeleteAllDocByAbsID(ids);
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "No rows deleted";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDocs  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteRowInGeneral(int AbsID)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "APZ_SalesForeCast_DeleteDocInGeneral";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@AbsID", AbsID));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "ERROR";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save Stage  FAILED ", ex.Message);
			return ex.Message;
		}
	}
}
