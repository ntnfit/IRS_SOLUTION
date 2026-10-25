using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Retail;

namespace Apzon.Services.Repository.Retail;

public class RetailChooseFromListRepository : BaseService<DataTable>, IRetailChooseFromList
{
	public RetailChooseFromListRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet ListItems(string username, string storecode, string itemcode, string cardcode, string barcode)
	{
		try
		{
			return ExecuteData("apz_retail_cfl_item", CommandType.StoredProcedure, new SqlParameter("@UserName", Function.ToString(username)), new SqlParameter("@StoreCode", Function.ToString(storecode)), new SqlParameter("@ItemCode", Function.ToString(itemcode)), new SqlParameter("@CardCode", Function.ToString(cardcode)), new SqlParameter("@Codebars", Function.ToString(barcode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet Customer(string username, string storecode, string cardcode)
	{
		try
		{
			return ExecuteData("apz_retail_cfl_customer", CommandType.StoredProcedure, new SqlParameter("@UserName", Function.ToString(username)), new SqlParameter("@StoreCode", Function.ToString(storecode)), new SqlParameter("@CardCode", Function.ToString(cardcode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}
}
