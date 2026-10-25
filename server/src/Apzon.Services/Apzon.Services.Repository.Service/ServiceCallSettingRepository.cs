using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Service;

namespace Apzon.Services.Repository.Service;

public class ServiceCallSettingRepository : BaseService<IServiceCallSeting>, IServiceCallSeting
{
	public ServiceCallSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetSetting(string user)
	{
		try
		{
			string query = "APZ_ServiceCallSetting_GetSetting";
			SqlParameter sqlParameter = new SqlParameter("@user", user);
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, sqlParameter);
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

	public DataTable GetService(string user)
	{
		try
		{
			string query = "APZ_ServiceCallSetting_GetService";
			SqlParameter sqlParameter = new SqlParameter("@user", user);
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, sqlParameter);
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

	public string Save(string name, string email, string pass, int service, string userName)
	{
		try
		{
			string query = "APZ_ServiceCallSetting_Save";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@user", userName));
			list.Add(new SqlParameter("@name", name));
			list.Add(new SqlParameter("@email", email));
			list.Add(new SqlParameter("@pass", pass));
			list.Add(new SqlParameter("@service", service));
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return string.Empty;
				}
			}
			return "loi";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return ex.Message;
		}
	}
}
