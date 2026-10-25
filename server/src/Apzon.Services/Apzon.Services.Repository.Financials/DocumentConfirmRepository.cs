using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Financials;
using Apzon.Services.Interfaces.Financials;

namespace Apzon.Services.Repository.Financials;

public class DocumentConfirmRepository : BaseService<DocumentConfirmImpl>, IDocumentConfirm
{
	public DocumentConfirmRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable SearchDataByBarCode(string barcode, string type)
	{
		DataTable result = new DataTable();
		try
		{
			if (!string.IsNullOrEmpty(barcode))
			{
				string[] array = barcode.Split('-');
				if (array.Length < 2)
				{
					return result;
				}
				string empty = string.Empty;
				SqlParameter[] array2 = new SqlParameter[2];
				if (type == "D")
				{
					empty = "APZ_DocumentConfirm_Default";
					array2[0] = new SqlParameter("@docType", array[0]);
					array2[1] = new SqlParameter("@docEntry", array[1]);
				}
				else
				{
					empty = "APZ_DocumentConfirm_Invoice";
					array2[0] = new SqlParameter("@InvoiceNo", array[0]);
					array2[1] = new SqlParameter("@InvoiceSeries", array[1]);
				}
				result = ExecuteDataTable(empty, CommandType.StoredProcedure, array2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchDataByBarCode ", ex.Message);
		}
		return result;
	}

	public string GetRecord(int docEntry, int type)
	{
		DataTable dataTable = new DataTable();
		int num = 0;
		try
		{
			string query = "APZ_DocumentConfirm_GetRecord_NextLastFirtPrev";
			dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry), new SqlParameter("@Type", type));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				num = dataTable.Rows[0].Field<int>("DocEntry");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRecord ", ex.Message);
		}
		return num.ToString();
	}
}
