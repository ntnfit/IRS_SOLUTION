using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Inventory.ItemManagement;

namespace Apzon.Services.Repository.Inventory.ItemManagement;

public class SerialNumberTransactionsRespository : BaseService<DataTable>, ISerialNumberTransactions
{
	public SerialNumberTransactionsRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDocs(string distNumber, string itemfrom, string itemto, string status)
	{
		try
		{
			string query = "APZ_Inventory_Serial_SerialTransaction_GetDocs";
			if (string.IsNullOrEmpty(distNumber))
			{
				distNumber = "";
			}
			if (string.IsNullOrEmpty(itemfrom))
			{
				itemfrom = "";
			}
			if (string.IsNullOrEmpty(itemto))
			{
				itemto = "";
			}
			if (string.IsNullOrEmpty(status))
			{
				status = "-1";
			}
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@distNumber", distNumber), new SqlParameter("@ItemFrom", itemfrom), new SqlParameter("@ItemTo", itemto), new SqlParameter("@Status", Function.ParseInt(status)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetSerialNumbers(string itemCode, int sysNumber)
	{
		try
		{
			string query = "SELECT a.\"ItemCode\", a.\"itemName\", a.\"MnfSerial\", a.\"SysNumber\", a.\"DistNumber\", a.\"LotNumber\", a.\"ExpDate\", a.\"MnfDate\", a.\"InDate\", a.\"GrntStart\", a.\"GrntExp\", b.\"WhsCode\", a.\"Location\", \r\n\t\t                            CASE WHEN b.\"Quantity\" = 0 THEN '1' ELSE (CASE WHEN b.\"CommitQty\" > 0 THEN '2' ELSE '0' END) END AS \"Status\", a.\"Notes\" \r\n\t                            FROM OSRN a \r\n\t                            LEFT OUTER JOIN OSRQ b ON a.\"AbsEntry\" = b.\"AbsEntry\" \r\n\t                            WHERE a.\"ItemCode\" = itemCode \r\n\t\t                            AND a.\"SysNumber\" = sysNumber; ";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@itemCode", itemCode), new SqlParameter("@sysNumber", sysNumber));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetTransactions(string itemCode, int sysNumber, string whsCode)
	{
		try
		{
			string query = "SELECT b.\"DocType\", \"DocEntry\", \"DocNum\" AS \"Document\", b.\"DocLine\", b.\"DocDate\", b.\"LocCode\", b.\"CardName\", c.\"DftBinAbs\", a.\"AllocQty\", \r\n\t\t                            CASE WHEN a.\"AllocQty\" > 0 THEN 'Allocation' ELSE (CASE WHEN a.\"Quantity\" > 0 THEN 'IN' ELSE 'OUT' END) END AS \"Direction\" \r\n\t                            FROM ITL1 a \r\n\t                            LEFT OUTER JOIN OITL b ON a.\"LogEntry\" = b.\"LogEntry\" \r\n\t                            LEFT OUTER JOIN OITW c ON a.\"ItemCode\" = c.\"ItemCode\" AND b.\"LocCode\" = c.\"WhsCode\" \r\n\t                            WHERE a.\"ItemCode\" = @itemCode \r\n\t\t                            AND a.\"SysNumber\" = @sysNumber \r\n\t\t                            AND b.\"LocCode\" = @locCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@itemCode", itemCode));
			list.Add(new SqlParameter("@sysNumber", sysNumber));
			list.Add(new SqlParameter("@locCode", whsCode));
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
