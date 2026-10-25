using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Inventory.Batches;

namespace Apzon.Services.Repository.Inventory.Batches;

public class BatchDetailsRepository : BaseService<DataTable>, IBatchDetail
{
	private string UserName { get; set; }

	public BatchDetailsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetQuantities(string itemCode, int sysNumber)
	{
		try
		{
			string query = "APZ_Inventory_Bathch_BatchDetails_GetQuantities";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@sysNumber", sysNumber));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable Search(string itemCode, string itemName, string batch)
	{
		try
		{
			return ExecuteDataTable("APZ_Inventory_Bathch_BatchDetails_Searches", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@itemName", itemName), new SqlParameter("@distNumber", batch));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string Save(DataSet ds)
	{
		string text = "";
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				text = SaveBatch(ds.Tables[0].Rows[0]);
				if (string.IsNullOrEmpty(text))
				{
					text = SaveLine(ds.Tables[1]);
				}
				if (string.IsNullOrEmpty(text))
				{
					dbTransaction.Commit();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			text = ex.Message;
		}
		return text;
	}

	private string SaveBatch(DataRow r)
	{
		string result = "";
		try
		{
			string text = "UPDATE OBTN SET ";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrWhiteSpace(Function.ToString(r["Status"])))
			{
				text += " Status = @Status,";
				list.Add(new SqlParameter("@Status", Function.ToString(r["Status"])));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(r["MnfSerial"])))
			{
				text += " MnfSerial = @MnfSerial,";
				list.Add(new SqlParameter("@MnfSerial", Function.ToString(r["MnfSerial"])));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(r["LotNumber"])))
			{
				text += " LotNumber = @LotNumber,";
				list.Add(new SqlParameter("@LotNumber", Function.ToString(r["LotNumber"])));
			}
			DateTime? dateTime = Function.ParseDateTime(r["InDate"]);
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				if (dateTime.Value.Year > 1900)
				{
					text += " InDate = @InDate,";
					list.Add(new SqlParameter("@InDate", dateTime));
				}
			}
			dateTime = Function.ParseDateTime(r["MnfDate"]);
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				if (dateTime.Value.Year > 1900)
				{
					text += " MnfDate = @MnfDate,";
					list.Add(new SqlParameter("@MnfDate", dateTime));
				}
			}
			dateTime = Function.ParseDateTime(r["ExpDate"]);
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				if (dateTime.Value.Year > 1900)
				{
					text += " ExpDate = @ExpDate,";
					list.Add(new SqlParameter("@ExpDate", dateTime));
				}
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(r["Notes"])))
			{
				text += " Notes = @Notes,";
				list.Add(new SqlParameter("@Notes", Function.ToString(r["Notes"])));
			}
			if (text[text.Length - 1] == ',')
			{
				text = text.Remove(text.Length - 1);
			}
			list.Add(new SqlParameter("@AbsEntry", Function.ParseInt(r["AbsEntry"])));
			text += " WHERE AbsEntry = @AbsEntry";
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) != 1)
			{
				result = "[Apzon.BatchDetails.SaveBatch] - No row updated";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			result = ex.Message;
		}
		return result;
	}

	private string SaveLine(DataTable dt)
	{
		string text = "";
		try
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				text = SaveALine(dt.Rows[i]);
				if (!string.IsNullOrEmpty(text))
				{
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			text = ex.Message;
		}
		return text;
	}

	private string SaveALine(DataRow r)
	{
		string result = "";
		try
		{
			string text = "UPDATE OBTW SET ";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrWhiteSpace(Function.ToString(r["Location"])))
			{
				text += " Location = @Location";
				list.Add(new SqlParameter("@Location", Function.ToString(r["Location"])));
				list.Add(new SqlParameter("@AbsEntry", Function.ParseInt(r["AbsEntry"])));
				text += " WHERE AbsEntry = @AbsEntry";
				if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) != 1)
				{
					result = "[Apzon.BatchDetails.SaveALine] - No row updated";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			result = ex.Message;
		}
		return result;
	}

	public DataTable GetByAbsEntry(int absEntry)
	{
		try
		{
			return ExecuteDataTable("apz_document_getbatchserial_getbyabs", CommandType.StoredProcedure, new SqlParameter("@AbsEntry", absEntry), new SqlParameter("@Type", "B"));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadAnEntry(int absEntry)
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM OBTN WHERE \"AbsEntry\" = @AbsEntry;", CommandType.Text, new SqlParameter("@AbsEntry", absEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataBatchWhs(string itemCode, string code, string whs)
	{
		try
		{
			return ExecuteDataTable("APZ_BatchAndSerial_GetData", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@code", code), new SqlParameter("@whs", whs));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataAutoSelect(string type, string isAllocateBin, string itemCode, string whsCode, double genQty, DataTable dtExistsBatch)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				CreateTempTable(dtExistsBatch, "#tempExist");
				BulkCopy(dtExistsBatch, "#tempExist");
				return ExecuteDataTable("APZ_BatchDetail_GetDataAutoSelect", CommandType.StoredProcedure, new SqlParameter("@type", type), new SqlParameter("@isAllocateBin", isAllocateBin), new SqlParameter("@itemCode", itemCode), new SqlParameter("@whsCode", whsCode), new SqlParameter("@qty", genQty));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataAllowcateBin(string itemCode, string whsCode, decimal qty)
	{
		try
		{
			return ExecuteDataTable("APZ_BatchDetail_GetDataAllowcateBin", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@whsCode", whsCode), new SqlParameter("@qty", qty));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet LoadDataAutoSelectConfig()
	{
		try
		{
			return ExecuteData("APZ_BathDetails_LoadDataAutoSelectConfig", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public string SaveConfig(DataTable dt, string userName)
	{
		try
		{
			UserName = userName;
			return Process(ApzObjectTypes.oAutoBatchConfig, -1, Enums.TransactionType.U, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		if (objtype == ApzObjectTypes.oAutoBatchConfig)
		{
			CreateTempTable(document, "#tempConfig");
			BulkCopy(document, "#tempConfig");
			DataTable dataTable = ExecuteDataTable("APZ_BathDetails_SaveConfig", CommandType.StoredProcedure, new SqlParameter("@userName", UserName));
			if (!dataTable.IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.Error, "Can not save config");
			}
			return new HttpResult(Enums.MessageCode.Success, Function.ToString(dataTable.Rows[0][1]));
		}
		throw new Exception("Object not support");
	}

	public int GetPage(int current, int type, string username, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Current", current));
			list.Add(new SqlParameter("@Type", type));
			if (!string.IsNullOrEmpty(username))
			{
				list.Add(new SqlParameter("@Username", username));
			}
			if (branch != -1)
			{
				list.Add(new SqlParameter("@Branch", branch));
			}
			DataTable dataTable = ExecuteDataTable("Apz_BatchDetail_GetPage", CommandType.StoredProcedure, list.ToArray());
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	public DataSet GetBatchDetail(int absEntry, string itemCode, string status, string distNumber, string mnfSerial, string lotNumber)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (absEntry < 0)
			{
				absEntry = -1;
			}
			if (string.IsNullOrEmpty(itemCode))
			{
				itemCode = "";
			}
			if (string.IsNullOrEmpty(status))
			{
				status = "";
			}
			if (string.IsNullOrEmpty(distNumber))
			{
				distNumber = "";
			}
			if (string.IsNullOrEmpty(mnfSerial))
			{
				mnfSerial = "";
			}
			if (string.IsNullOrEmpty(lotNumber))
			{
				lotNumber = "";
			}
			list.Add(new SqlParameter("@AbsEntry", absEntry));
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@Status", status));
			list.Add(new SqlParameter("@DistNumber", distNumber));
			list.Add(new SqlParameter("@MnfSerial", mnfSerial));
			list.Add(new SqlParameter("@LotNumber", lotNumber));
			return ExecuteData("APZ_BatchDetail_GetBatchDetail", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetDataBatchSerial(string serialNo, string mnfSerial, string lotNumber, string itemCode, string tranType)
	{
		try
		{
			string query = (tranType.Equals("B") ? "select \"AbsEntry\", \"Status\" from OBTN where \"DistNumber\" = @serialNo and \"MnfSerial\" = @mnfSerial and \"LotNumber\" = @lotNumber and \"ItemCode\" = @itemCode;" : "select \"AbsEntry\", \"Status\" from OSRN where \"DistNumber\" = @serialNo and \"MnfSerial\" = @mnfSerial and \"LotNumber\" = @lotNumber and \"ItemCode\" = @itemCode;");
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@serialNo", serialNo), new SqlParameter("@mnfSerial", mnfSerial), new SqlParameter("@lotNumber", lotNumber), new SqlParameter("@itemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
