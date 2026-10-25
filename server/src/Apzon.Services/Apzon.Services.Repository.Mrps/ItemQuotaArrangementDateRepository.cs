using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Mrps;

namespace Apzon.Services.Repository.Mrps;

public class ItemQuotaArrangementDateRepository : BaseService<DataSet>, IItemQuotaArrangementDate
{
	public ItemQuotaArrangementDateRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable DateGetDocs(int periodID)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "Select * From APZ_IQA2 Where \"PeriodID\" = @periodID;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@periodID", periodID));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetCardName(string CardCode)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "";
			List<SqlParameter> list = new List<SqlParameter>();
			text = "Select \"CardName\" From OCRD Where \"CardCode\" = @CardCode;";
			list.Add(new SqlParameter("@CardCode", CardCode));
			result = ExecuteDataTable(text, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
			ProcessDate(ds, ref ret, ref objId, ref objNum);
			break;
		case Enums.TransactionType.D:
			ProcessDeleteDate(ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessDate(DataSet ds, ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			if (ds == null || ds.Tables.Count == 0 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				ret.Message = "No data to save";
			}
			else
			{
				DataTable dataTable = ds.Tables[0];
				ret.Message = DateDeleteByPeriodID(Function.ParseInt(dataTable.Rows[0]["PeriodID"]));
				if (string.IsNullOrEmpty(ret.Message))
				{
					ret.Message = DateAddItem(dataTable);
				}
			}
			if (string.IsNullOrEmpty(ret.Message))
			{
				objId = Function.ToString(ds.Tables[0].Rows[0]["PeriodID"]);
				objNum = Function.ToString(ds.Tables[0].Rows[0]["PeriodID"]);
			}
			else
			{
				objId = "";
				objNum = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	private void ProcessDeleteDate(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			string query = "Select \"PeriodID\" From APZ_IQA2 Where \"PeriodID\"=" + objId;
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				query = "Delete from APZ_IQA2 Where \"PeriodID\"=" + objId;
				if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
				{
					objId = "Y";
				}
			}
			else
			{
				objId = "No data to save";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
			objId = ex.Message;
		}
	}

	public string DateSave(DataSet ds)
	{
		return Process(ApzObjectTypes.oMRPItemQuotaArrangementDate, -1, Enums.TransactionType.A, ds).Message;
	}

	private string DateAddItem(DataTable dt)
	{
		string text = "";
		try
		{
			CreateTempTable(dt, "#APZ_MRP_QUOTA_DATEARR");
			BulkCopy(dt, "#APZ_MRP_QUOTA_DATEARR");
			string query = "apz_process_MRP_QUOTA_DATEARR";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (dataTable.Columns.Contains("message"))
				{
					return Function.ToString(dataTable.Rows[0]["message"]);
				}
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string DateDeleteByPeriodID(int periodID)
	{
		string result = "";
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@periodID", periodID));
			string query = "Delete FROM APZ_IQA2 Where \"PeriodID\"=@periodID;";
			ExecuteNonQuery(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public string DeleteRows(int periodID)
	{
		HttpResult httpResult = Process(ApzObjectTypes.oMRPItemQuotaArrangementDate, -1, Enums.TransactionType.D, null, periodID.ToString());
		if (!string.IsNullOrEmpty(httpResult.Message))
		{
			return httpResult.Message.ToString();
		}
		return httpResult.Content.ToString();
	}
}
