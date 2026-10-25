using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class SearchSettingRepository : BaseService<DataTable>, ISearchSetting
{
	public SearchSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetSearchSetting(string formId, string itemId, string user)
	{
		try
		{
			string query = "apz_searchsetting_get";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@FormId", formId ?? ""), new SqlParameter("@Itemid", itemId ?? ""), new SqlParameter("@UserSign", user ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string SaveData(DataTable dt)
	{
		try
		{
			if (dt.IsNotNull())
			{
				DataRow dataRow = dt.Rows[0];
				ExecuteNonQuery("apz_searchsetting_save", CommandType.StoredProcedure, new SqlParameter("@FormId", dataRow["FormId"]), new SqlParameter("@ItemId", dataRow["ItemId"]), new SqlParameter("@UserSign", dataRow["UserSign"]), new SqlParameter("@Branch", dataRow["Branch"]), new SqlParameter("@Period", dataRow["Period"]), new SqlParameter("@EmpId", dataRow["EmpId"]), new SqlParameter("@EmpName", dataRow["EmpName"]), new SqlParameter("@Dept", dataRow["Dept"]), new SqlParameter("@Status", dataRow["Status"]));
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
