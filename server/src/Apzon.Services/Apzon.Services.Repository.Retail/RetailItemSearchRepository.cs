using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Retail;

namespace Apzon.Services.Repository.Retail;

public class RetailItemSearchRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IRetailItemSearch
{
	public string TempTable = "#";

	public DataTable LoadDoc(string itemCode, string itemType, string userName, string branch, string pr)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(itemCode))
			{
				list.Add(new SqlParameter("@itemCode", itemCode));
			}
			if (!string.IsNullOrEmpty(itemType))
			{
				list.Add(new SqlParameter("@itemType", itemType));
			}
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			if (!string.IsNullOrEmpty(pr))
			{
				list.Add(new SqlParameter("@pr", pr));
			}
			DataTable dataTable = ExecuteDataTable("APZ_ItemSearch_GetDoc", CommandType.StoredProcedure, list.ToArray());
			if (dataTable.IsNotNull())
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
