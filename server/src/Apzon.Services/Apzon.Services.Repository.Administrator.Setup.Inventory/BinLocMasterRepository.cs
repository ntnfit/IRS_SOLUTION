using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Administrator.Setup.Inventory;

namespace Apzon.Services.Repository.Administrator.Setup.Inventory;

public class BinLocMasterRepository : BaseService<IBinLocMaster>, IBinLocMaster
{
	public BinLocMasterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetBinLocByBinCode(string binCode)
	{
		try
		{
			return ExecuteDataTable("APZ_BinLocations_GetBinLocByBinCode", CommandType.StoredProcedure, new SqlParameter("@binCode", binCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetWHSublevel()
	{
		try
		{
			string query = "SELECT * from OBFC WHERE \"FldType\" = 'S' AND \"Activated\" = 'Y';";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataBinLocation(int absEntry)
	{
		try
		{
			return ExecuteDataTable("APZ_BinLocations_GetBinLocation", CommandType.StoredProcedure, new SqlParameter("@absEntry", absEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataBinLocation(string binCode, string whsCode)
	{
		try
		{
			return ExecuteDataTable("select * from OBIN where \"BinCode\" = @binCode and \"WhsCode\" = @whsCode;", CommandType.Text, new SqlParameter("@binCode", binCode), new SqlParameter("@whsCode", whsCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBatchBinAllowcateLineByDocEntry(string docEntry, ApzObjectTypes objectType)
	{
		try
		{
			return ExecuteDataTable("apz_document_getBinAllowcationLine_byDocEntry", CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry), new SqlParameter("@ObjType", Function.ToString((int)objectType)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataCopyBinAllowcationLine(string lstDocentry, ApzObjectTypes objectType)
	{
		try
		{
			return ExecuteDataTable("apz_document_GetDataCopyBinAllowcationLine", CommandType.StoredProcedure, new SqlParameter("@docEntry", lstDocentry), new SqlParameter("@ObjType", Function.ToString((int)objectType)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAllBinCode()
	{
		try
		{
			return ExecuteDataTable("APZ_BinLocations_GetBinLocation", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public DataTable GetTableStructure()
	{
		try
		{
			return ExecuteDataTable("select top 0 * from OBIN;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string GetBinSeperatorOfWarehouse(string whsCode)
	{
		try
		{
			string query = "select \"BinSeptor\" from OWHS where \"WhsCode\"=@whsCode;";
			return Function.ToString(ExecuteScalar(query, CommandType.Text, new SqlParameter("@whsCode", whsCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public DataTable GetBinLocProperties(int absEntry)
	{
		try
		{
			return ExecuteDataTable("APZ_BinLocations_GetBinLocProp", CommandType.StoredProcedure, new SqlParameter("@absEntry", absEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public DataTable GetBinDefault(string tranType, string whsCode, string itemCode, double qty)
	{
		try
		{
			return ExecuteDataTable("APZ_Warehouse_WhsGetDefBinItem", CommandType.StoredProcedure, new SqlParameter("@transType", tranType), new SqlParameter("@itemCode", itemCode), new SqlParameter("@whsCode", whsCode), new SqlParameter("@quantity", qty));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public DataTable GetBinDefaultBatch(string tranType, string whsCode, string itemCode, int absEntry, double qty)
	{
		try
		{
			return ExecuteDataTable("APZ_Warehouse_WhsGetDefBinItem_Batch", CommandType.StoredProcedure, new SqlParameter("@transType", tranType), new SqlParameter("@itemCode", itemCode), new SqlParameter("@whsCode", whsCode), new SqlParameter("@batchEntry", absEntry), new SqlParameter("@quantity", qty));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}
}
