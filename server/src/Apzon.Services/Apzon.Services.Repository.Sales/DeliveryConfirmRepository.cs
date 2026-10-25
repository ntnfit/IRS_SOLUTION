using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Sales;

namespace Apzon.Services.Repository.Sales;

public class DeliveryConfirmRepository : BaseService<DataTable>, IDeliveryConfirm
{
	public DeliveryConfirmRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetDeliveryOrder(int docEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docEntry", docEntry));
			return ExecuteData("Apz_DeliveryConfirm_GetDeliveryOrder", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetBatchDetail(string batch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@batch", batch));
			return ExecuteDataTable("Apz_DeliveryConfirm_GetBatchDetail", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string ConfirmBatch(DataSet ds)
	{
		try
		{
			if (ds.IsHasValidValue() && ds.Tables[1].IsNotNull())
			{
				DataTable dataTable = ds.Tables[2];
				int num = Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
				string text = "";
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					text = text + "," + Function.ToString(dataTable.Rows[i]["U_BatchNo"]);
				}
				if (!string.IsNullOrEmpty(text) && text.StartsWith(","))
				{
					text = text.Substring(1, text.Length - 1);
				}
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@DocEntry", num));
				list.Add(new SqlParameter("@Batch", text));
				if (ExecuteDataTable("Apz_DeliveryConfirm_ConfirmBatch", CommandType.StoredProcedure, list.ToArray()).IsNotNull())
				{
					Logging.Write("WATCH", "Delivery Confirmed ", num.ToString());
					return "";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}
}
