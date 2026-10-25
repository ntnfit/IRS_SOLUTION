using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Inventory.ItemManagement;

namespace Apzon.Services.Repository.Inventory.ItemManagement;

public class SerialNumberDetailsRespository : BaseService<DataTable>, ISerialNumberDetails
{
	public SerialNumberDetailsRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetQuantity(int absEntry)
	{
		try
		{
			string query = "SELECT \"Quantity\" FROM OSRQ WHERE \"AbsEntry\"=@absEntry;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@absEntry", absEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable Search(string itemCode, string itemName, string mnfSerial, string serialNum, string lotNum)
	{
		try
		{
			string query = "APZ_Inventory_SerialNumberDetails_Search";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@itemName", itemName), new SqlParameter("@mnfSerial", mnfSerial), new SqlParameter("@serialNum", serialNum), new SqlParameter("@lotNum", lotNum));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetSearialNumbers(string itemCode)
	{
		try
		{
			string query = "SELECT b.*, CASE WHEN a.\"Quantity\" = 0 THEN '1' ELSE (CASE WHEN a.\"CommitQty\" > 0 THEN '2' ELSE '0' END) END AS \"StatusOSRQ\" \r\n                                FROM OSRQ a \r\n\t                            LEFT OUTER JOIN OSRN b ON b.\"AbsEntry\" = a.\"AbsEntry\" \r\n\t                            WHERE a.\"ItemCode\" = @itemCode;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@itemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string SaveSerialNumberDetail(DataTable dt)
	{
		string text = "";
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					text = SaveASerial(dt.Rows[i]);
					if (!string.IsNullOrEmpty(text))
					{
						break;
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					dbTransaction.Commit();
				}
				else
				{
					dbTransaction.Rollback();
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

	private string SaveASerial(DataRow r)
	{
		string result = "";
		try
		{
			string text = "UPDATE OSRN SET ";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrWhiteSpace(Function.ToString(r["MnfSerial"])))
			{
				text += " MnfSerial = @MnfSerial,";
				list.Add(new SqlParameter("@MnfSerial", Function.ToString(r["MnfSerial"])));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(r["DistNumber"])))
			{
				text += " DistNumber = @DistNumber,";
				list.Add(new SqlParameter("@DistNumber", Function.ToString(r["DistNumber"])));
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
			dateTime = Function.ParseDateTime(r["GrntStart"]);
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				if (dateTime.Value.Year > 1900)
				{
					text += " GrntStart = @GrntStart,";
					list.Add(new SqlParameter("@GrntStart", dateTime));
				}
			}
			dateTime = Function.ParseDateTime(r["GrntExp"]);
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				if (dateTime.Value.Year > 1900)
				{
					text += " GrntExp = @GrntExp";
					list.Add(new SqlParameter("@GrntExp", dateTime));
				}
			}
			if (text[text.Length - 1] == ',')
			{
				text = text.Remove(text.Length - 1);
			}
			list.Add(new SqlParameter("@ItemCode", Function.ToString(r["ItemCode"])));
			list.Add(new SqlParameter("@SysNumber", Function.ParseInt(r["SysNumber"])));
			text += " WHERE ItemCode = @ItemCode AND SysNumber = @SysNumber";
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) != 1)
			{
				result = "[Apzon.SaveASerial] - No row updated";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			result = ex.Message;
		}
		return result;
	}

	public string GetUniqueSerialNo()
	{
		try
		{
			string query = "SELECT \"SriUniqFld\" FROM OADM;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public DataTable GetByAbsEntry(int absEntry)
	{
		try
		{
			return ExecuteDataTable("apz_document_getbatchserial_getbyabs", CommandType.StoredProcedure, new SqlParameter("@AbsEntry", absEntry), new SqlParameter("@Type", "S"));
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
			return ExecuteDataTable("APZ_Inventory_SerialNumberDetails_LoadAnEntry", CommandType.StoredProcedure, new SqlParameter("@AbsEntry", absEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
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
			DataTable dataTable = ExecuteDataTable("Apz_SerialDetail_GetPage", CommandType.StoredProcedure, list.ToArray());
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

	public DataSet GetSerialDetail(int absEntry, string itemCode, string status, string distNumber, string mnfSerial, string lotNumber)
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
			return ExecuteData("APZ_SerialDetail_GetSerialDetail", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}
}
