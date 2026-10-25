using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Retail;

namespace Apzon.Services.Repository.Retail;

public class RetailCustomerSearchRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IRetailCustomerSearch
{
	public string TempTable = "#";

	public DataTable LoadDoc(string cardType, string userName, string branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(cardType))
			{
				list.Add(new SqlParameter("@cardType", cardType));
			}
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			DataTable dataTable = ExecuteDataTable("APZ_CustomerSearch_GetDoc", CommandType.StoredProcedure, list.ToArray());
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
