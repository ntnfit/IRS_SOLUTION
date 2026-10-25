using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Mrps;

namespace Apzon.Services.Repository.Mrps;

public class ItemQuotaArrangementRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IItemQuotaArrangement
{
	private string TableDocName = "#APZ_MRP_QUOTA_ARR";

	private string TableLineName = "#APZ_MRP_QUOTA_LINE_ARR";

	public DataTable GetLines(string ItemCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "Select * From APZ_IQA1 Where \"ItemCode\"=@ItemCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", ItemCode));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetDocs(string itemCode)
	{
		DataTable result = new DataTable();
		try
		{
			if (itemCode == null)
			{
				itemCode = "";
			}
			itemCode = itemCode.Replace("'", "");
			itemCode = itemCode.Replace(" ", "");
			result = ExecuteDataTable(QueryGetDocs(), CommandType.Text, new SqlParameter("@ItemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string QueryGetDocs()
	{
		return "Select * From APZ_OIQA where \"ItemCode\" like '%' + @ItemCode + '%'";
	}

	public DataTable GetADoc(string ItemCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "Select * From APZ_OIQA Where \"ItemCode\" = @ItemCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", ItemCode));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	private bool ExistItem(string itemCode)
	{
		try
		{
			string query = "Select * From APZ_OIQA Where \"ItemCode\"=@itemCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@itemCode", itemCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public DataTable GetItemName(string ItemCode)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "";
			List<SqlParameter> list = new List<SqlParameter>();
			text = "Select \"ItemName\" From OITM Where \"ItemCode\" = @ItemCode;";
			list.Add(new SqlParameter("@ItemCode", ItemCode));
			result = ExecuteDataTable(text, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable CheckExistPeriod(int periodID)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "";
			List<SqlParameter> list = new List<SqlParameter>();
			text = "Select top 1 \"PeriodID\" From APZ_IQA1 Where \"PeriodID\" = @periodID;";
			list.Add(new SqlParameter("@periodID", periodID));
			result = ExecuteDataTable(text, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable CheckExistItem(string ItemCode)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "";
			List<SqlParameter> list = new List<SqlParameter>();
			text = "Select top 1 \"ItemCode\" From APZ_OIQA Where \"ItemCode\" = @ItemCode;";
			list.Add(new SqlParameter("@ItemCode", ItemCode));
			result = ExecuteDataTable(text, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public string SaveDoc(DataSet ds)
	{
		return Process(ApzObjectTypes.oMRPItemQuotaArrangement, -1, Enums.TransactionType.A, ds).Message;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
			ProcessItem(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.D:
			ProcessDeleteItem(ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessDeleteItem(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", objId));
			string query = "Delete FROM APZ_IQA2 Where \"PeriodID\" in (Select \"PeriodID\" From APZ_IQA1 Where \"ItemCode\"=@ItemCode);\r\n                        Delete FROM APZ_IQA1 Where \"ItemCode\"=@ItemCode;\r\n                        Delete FROM APZ_OIQA Where \"ItemCode\"=@ItemCode;";
			if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) > 0)
			{
				objId = "";
			}
			else
			{
				objId = "No rows deleted";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			objId = ex.Message;
		}
	}

	private void ProcessItem(ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (ds == null || ds.Tables.Count != 2 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0 || ds.Tables[1] == null || ds.Tables[1].Rows.Count == 0)
			{
				ret.Message = "No data to save";
			}
			else
			{
				string itemCode = Function.ToString(ds.Tables[0].Rows[0]["ItemCode"]);
				string text = (ExistItem(itemCode) ? "U" : "A");
				CreateTempTable(ds.Tables[0], TableDocName);
				BulkCopy(ds.Tables[0], TableDocName);
				DataTable dataTable = ds.Tables[1].Clone();
				dataTable.Columns["PeriodID"].DataType = typeof(int);
				foreach (DataRow row in ds.Tables[1].Rows)
				{
					dataTable.ImportRow(row);
				}
				dataTable.AcceptChanges();
				CreateTempTable(dataTable, TableLineName);
				BulkCopy(dataTable, TableLineName);
				SqlParameter sqlParameter = new SqlParameter("@Type", text);
				DataTable dataTable2 = ExecuteDataTable("apz_process_MRP_QUOTA_ARR", CommandType.StoredProcedure, sqlParameter);
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					ret.Message = Function.ToString(dataTable2.Rows[0][0]);
					if (text == "A")
					{
						ret.Message = "Save error!";
					}
					else
					{
						ret.Message = "Update error";
					}
				}
				else if (!GetVendorData(itemCode).IsNotNull())
				{
					DateTime? fromDate = Function.ParseDateTime(dataTable.Rows[0]["FromDate"]);
					DataSet vendorAndBPCatalog = GetVendorAndBPCatalog(itemCode, fromDate);
					if (vendorAndBPCatalog.IsNotNull() && vendorAndBPCatalog.Tables[0].IsNotNull())
					{
						double num = 100 / vendorAndBPCatalog.Tables[0].Rows.Count;
						for (int i = 0; i < vendorAndBPCatalog.Tables[0].Rows.Count - 1; i++)
						{
							vendorAndBPCatalog.Tables[0].Rows[i]["Per"] = num;
						}
						vendorAndBPCatalog.Tables[0].Rows[vendorAndBPCatalog.Tables[0].Rows.Count - 1]["Per"] = 100.0 - num * (double)(vendorAndBPCatalog.Tables[0].Rows.Count - 1);
						vendorAndBPCatalog.Tables[0].AcceptChanges();
						IUnitOfWork unitOfWork = GetUnitOfWork();
						ret.Message = unitOfWork.ItemQuotaArrangementDate.DateSave(vendorAndBPCatalog);
					}
				}
				else
				{
					ret.Message = "";
				}
			}
			if (string.IsNullOrEmpty(ret.Message))
			{
				objId = Function.ToString(ds.Tables[0].Rows[0]["ItemCode"]);
				objNum = Function.ToString(ds.Tables[0].Rows[0]["ItemCode"]);
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

	private DataSet GetVendorAndBPCatalog(string itemCode, DateTime? fromDate)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select \"PeriodID\" from APZ_IQA1\r\nWhere \"ItemCode\"=@itemCode;", CommandType.Text, new SqlParameter("@itemCode", itemCode), new SqlParameter("@fromDate", fromDate));
			if (dataTable.IsNotNull())
			{
				int num = Function.ParseInt(dataTable.Rows[0][0]);
				DataTable dataTable2 = ExecuteDataTable("apz_process_MRP_QUOTA_GetVendorAndBPCatalog", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@periodID", num));
				if (dataTable2.IsNotNull())
				{
					dataTable2.TableName = "dtRet";
					DataSet dataSet = new DataSet();
					DataSet dataSet2 = dataTable2.DataSet;
					if (dataSet2 != null)
					{
						dataSet2.Tables.Remove("dtRet");
						dataSet2.AcceptChanges();
					}
					dataSet.Tables.Clear();
					dataSet.Tables.Add(dataTable2);
					return dataSet;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	private DataTable GetVendorData(string itemCode)
	{
		try
		{
			return ExecuteDataTable("select \r\n\ttop 1 1\r\nfrom APZ_IQA2 T0 inner join APZ_IQA1 T1 on T0.\"PeriodID\"=T1.\"PeriodID\"\r\ninner join APZ_OIQA T2 on T2.\"ItemCode\"=T1.\"ItemCode\"\r\nWhere T2.\"ItemCode\"=@itemCode;", CommandType.Text, new SqlParameter("@itemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string DeleteDoc(string ItemCode)
	{
		HttpResult httpResult = Process(ApzObjectTypes.oMRPItemQuotaArrangement, -1, Enums.TransactionType.D, null, ItemCode);
		if (!string.IsNullOrEmpty(httpResult.Message))
		{
			return httpResult.Message.ToString();
		}
		return httpResult.Content.ToString();
	}
}
