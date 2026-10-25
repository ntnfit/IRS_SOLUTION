using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Inventory;
using Apzon.Services.Interfaces.Inventory.Batches;

namespace Apzon.Services.Repository.Inventory.Batches;

public class BatchesControlRepository : BaseService<BatchTransactionImpl>, IBatchesControl
{
	public BatchesControlRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet LoadBatch(string option, string sb, string so, string bp, string userName, int branch)
	{
		try
		{
			string query = "Apz_BatchesControl_LoadBatch";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(option))
			{
				list.Add(new SqlParameter("@Option", option));
			}
			if (!string.IsNullOrEmpty(sb))
			{
				list.Add(new SqlParameter("@SbNo", sb));
			}
			if (!string.IsNullOrEmpty(so))
			{
				list.Add(new SqlParameter("@SoNo", so));
			}
			if (!string.IsNullOrEmpty(bp))
			{
				list.Add(new SqlParameter("@BpCode", bp));
			}
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@UserName", userName));
			}
			if (branch != -1)
			{
				list.Add(new SqlParameter("@Branch", branch));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string UpdateBatchSalesBlanket(string batches, int argNo)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Batch", batches));
			list.Add(new SqlParameter("@Argno", argNo));
			if (ExecuteDataTable("Apz_BatchesControl_UpdateBatchSalesBlanket", CommandType.StoredProcedure, list.ToArray()).IsNotNull())
			{
				Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "UpdateBatchSalesBlanket ", batches, argNo.ToString());
				return "";
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "UpdateBatchSalesBlanket ", batches, argNo.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "Update False!";
	}

	public string UpdateBatchForSales(DataTable dtBatch)
	{
		try
		{
			string text = "";
			if (dtBatch.IsNotNull() && dtBatch.IsExistsColumn("DistNumber") && dtBatch.IsExistsColumn("U_DocEntry"))
			{
				for (int i = 0; i < dtBatch.Rows.Count; i++)
				{
					text += string.Format("EXEC Apz_BatchesControl_UpdateBatchForSales '{0}', {1} \n", Function.ToString(dtBatch.Rows[i]["DistNumber"]), Function.ParseInt(dtBatch.Rows[i]["U_DocEntry"]));
				}
			}
			if (ExecuteDataTable(text, CommandType.Text).IsNotNull())
			{
				return "";
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Apz_BatchesControl_UpdateBatchForSales ", text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "Update False!";
	}

	public string UpdateBatchDetails(DataTable dtBatch)
	{
		try
		{
			string text = "";
			if (dtBatch.IsNotNull() && dtBatch.IsExistsColumn("DistNumber") && dtBatch.IsExistsColumn("U_DocEntry"))
			{
				for (int i = 0; i < dtBatch.Rows.Count; i++)
				{
					text += string.Format("EXEC Apz_BatchesControl_UpdateBatchDetails '{0}', '{1}', '{2}', '{3}', '{4}', '{5}' \n", Function.ToString(dtBatch.Rows[i]["DistNumber"]), Function.ToString(dtBatch.Rows[i]["U_StatusQC"]), Function.ToString(dtBatch.Rows[i]["U_ObjType"]), Function.ParseInt(dtBatch.Rows[i]["U_DocEntry"]), Function.ToString(dtBatch.Rows[i]["U_BPCode"]), Function.ToString(dtBatch.Rows[i]["U_NoteQC"]));
				}
			}
			if (ExecuteDataTable(text, CommandType.Text).IsNotNull())
			{
				return "";
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "UpdateBatchDetails ", text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "Update False!";
	}
}
