using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Production;

namespace Apzon.Services.Repository.Production;

public class ProductionOrderImportToolRepository : BaseService<DataSet>, IProductionOrderImportTool
{
	public ProductionOrderImportToolRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable Search(DataTable dt)
	{
		try
		{
			if (dt == null && dt.Rows.Count == 0)
			{
				return new DataTable();
			}
			DataRow dataRow = dt.Rows[0];
			string query = "APZ_ProductionOrderImportTool_SearchFG";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@Code", Function.ToString(dataRow["Code"])), new SqlParameter("@TabName", Function.ToString(dataRow["TabName"])), new SqlParameter("@Branch", Function.ToString(dataRow["Branch"])));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetTableStructure(string tabName)
	{
		try
		{
			string query = "";
			if (tabName == "FG")
			{
				query = "Select top 0\r\n\t                                *\r\n\t                                ,'' \"ItemName\"\r\n                                From OWOR;";
			}
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBOMHeader(string code)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(code))
			{
				return new DataTable();
			}
			string query = "select\r\n\t                            a.*\r\n\t                            ,(select itemname from oitm t1 where t1.itemcode=a.code)  ItemName\r\n                             from oitt a\r\n                             where a.code=@code";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@code", code ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetItemCodeAndName()
	{
		try
		{
			return ExecuteDataTable("select ItemCode,ItemName from oitm", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable RCGetProductionDetails(string barCode)
	{
		try
		{
			return ExecuteDataTable("APZ_ProductionOrderImportTool_RCGetProductionDetails", CommandType.StoredProcedure, new SqlParameter("@barCode", barCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetTransferedBinData(int docEntry, int lineNum)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_ProductionOrderImportTool_GetBinInvTransferDocEntry", CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry), new SqlParameter("@lineNum", lineNum));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				DataTable dataTable2 = new DataTable();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					int entry = Function.ParseInt(dataTable.Rows[i][0]);
					DataTable binData = GetBinData(entry);
					if (dataTable2.Rows.Count == 0)
					{
						dataTable2 = binData.Copy();
						continue;
					}
					for (int j = 0; j < binData.Rows.Count; j++)
					{
						dataTable2.ImportRow(binData.Rows[j]);
					}
					dataTable2.AcceptChanges();
				}
				return dataTable2;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private DataTable GetBinData(int entry)
	{
		try
		{
			return ExecuteDataTable("APZ_ProductionOrderImportTool_GetBinData", CommandType.StoredProcedure, new SqlParameter("@docEntry", entry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAllBinData(string itemCode, string whsCode)
	{
		try
		{
			return ExecuteDataTable("APZ_ProductionOrderImportTool_GetAllBinData", CommandType.StoredProcedure, new SqlParameter("@ItemCode", itemCode), new SqlParameter("@WhsCode", whsCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public bool IsBinWarehouse(string whsCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select isnull(BinActivat,'N') from owhs where whsCode=@whsCode", CommandType.Text, new SqlParameter("@whsCode", whsCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]) == "Y";
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public DataTable GetBinWarehouse()
	{
		try
		{
			return ExecuteDataTable(GetBinWarehouseQuery(), CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetBinWarehouseQuery()
	{
		return "select WhsCode from owhs where isnull(BinActivat,'N') = 'Y'";
	}

	public DataTable GetAllBatchSerial(string itemCode, string fromWhsCode, string toWhsCode)
	{
		try
		{
			return ExecuteDataTable("APZ_ProductionOrderImportTool_GetAllBatchSerial", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@fromWhsCode", fromWhsCode), new SqlParameter("@toWhsCode", toWhsCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetEditTableEnableColumns()
	{
		try
		{
			return ExecuteDataTable("APZ_ProductionOrderImportTool_GetEditTableEnableColumns", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string UpdateDataForBarcodeApp(string weekNo)
	{
		try
		{
			ExecuteNonQuery("APZ_ProductionOrderImportTool_UpdatePrd", CommandType.StoredProcedure, new SqlParameter("@WeekNo ", weekNo));
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataSet IssNdRecQtySearch(string date)
	{
		try
		{
			string text = "";
			string value = "";
			string value2 = "";
			try
			{
				text = date.Substring(0, 4);
				value = date.Substring(4, 2);
				value2 = date.Substring(6, 2);
			}
			catch (Exception)
			{
				text = "";
			}
			if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(value2))
			{
				return ExecuteData("APZ_ProductionOrderImportTool_GI4PrdOrder", CommandType.StoredProcedure, new SqlParameter("@date", new DateTime(Convert.ToInt32(text), Convert.ToInt32(value), Convert.ToInt32(value2))));
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
		}
		return new DataSet();
	}
}
