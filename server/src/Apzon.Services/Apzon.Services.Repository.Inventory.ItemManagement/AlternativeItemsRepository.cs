using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Services.Interfaces.Inventory.ItemManagement;

namespace Apzon.Services.Repository.Inventory.ItemManagement;

public class AlternativeItemsRepository : BaseService<DataTable>, IAlternativeItems
{
	private const string _temp = "#OALI";

	public AlternativeItemsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetItemNameByItemCode(string itemCode)
	{
		try
		{
			return ExecuteDataTable("select \"ItemName\" from OITM where \"ItemCode\"=@ItemCode;", CommandType.Text, new SqlParameter("@ItemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetDataDoc(Tuple<string, Enums.NavigateType> tuple)
	{
		try
		{
			string item = tuple.Item1;
			int item2 = (int)tuple.Item2;
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@type", item2));
			list.Add(new SqlParameter("@origItem", item));
			return ExecuteDataTable("APZ_AlternativeItems_GetDataDoc", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetData(string originItem)
	{
		try
		{
			string empty = string.Empty;
			if (string.IsNullOrEmpty(originItem))
			{
				empty = "select * from OALI;";
				return ExecuteDataTable(empty, CommandType.Text);
			}
			empty = "select * from OALI where \"OrigItem\" = @originItem;";
			return ExecuteDataTable(empty, CommandType.Text, new SqlParameter("@originItem", Function.ToString(originItem)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataSet GetDataCfl(string itemCode, string userName, int branch, string cardCode, DateTime docDate)
	{
		try
		{
			return ExecuteData("Apz_Cfl_GetListAlternativeItem", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@userName", userName), new SqlParameter("@branch", branch), new SqlParameter("@cardCode", cardCode), new SqlParameter("@docDate", docDate));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataSet();
		}
	}
}
