using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Administrator.Setup.Inventory;

namespace Apzon.Services.Repository.Administrator.Setup.Inventory;

public class WHSubSetupRepository : BaseService<DataTable>, IWHSubSetup
{
	private const string TempTable = "#dtOBSL";

	public WHSubSetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetAllWarehouseSublevel()
	{
		try
		{
			string query = "select \"FldNum\", \"DispName\" from OBFC where \"FldType\"='S' and \"Activated\"='Y';";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetWarehouseSublevelByFldAbs(int abs)
	{
		try
		{
			string query = "select a.\"AbsEntry\", a.\"FldAbs\", a.\"SLCode\", a.\"Descr\", \r\n                            nullif((select count(1) from OBIN where \"SL" + abs + "Code\" = a.\"SLCode\"), 0) \"NoOfBin\"\r\n                            from OBSL a where a.\"FldAbs\"=" + abs + " order by \"NoOfBin\" desc;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetWHSubByFldAbs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string IsUsedInBinLocCodes(int fldAbs, string slCode)
	{
		try
		{
			string query = "select top 1 1 from OBIN where \"SL" + fldAbs + "Code\"='" + slCode + "';";
			return Function.ToString(ExecuteScalar(query, CommandType.Text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Check  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable IsUsedInBinLocCodes(DataTable data)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				CreateTempTable(data, "#dtOBSL");
				BulkCopy(data, "#dtOBSL");
				return ExecuteDataTable("APZ_WhSublevel_IsUsedInBinLocCodes", CommandType.StoredProcedure);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string CheckCodeExist(int fldAbs, string slCode)
	{
		try
		{
			string query = "select top 1 1 from OBSL where \"FldAbs\" = @fldAbs and \"SLCode\" = @slCode;";
			SqlParameter[] parameters = new SqlParameter[2]
			{
				new SqlParameter("@fldAbs", fldAbs),
				new SqlParameter("@slCode", slCode)
			};
			return Function.ToString(ExecuteScalar(query, CommandType.Text, parameters));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Check  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable CheckCodeExist(DataTable data)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				CreateTempTable(data, "#dtOBSL");
				BulkCopy(data, "#dtOBSL");
				return ExecuteDataTable("APZ_WhSublevel_CheckCodeExist", CommandType.StoredProcedure);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string Save(int userSign, DataTable doc)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(doc, "#dtOBSL");
				BulkCopy(doc, "#dtOBSL");
				string query = "APZ_BinLocations_WHSublevel_Save";
				ExecuteNonQuery(query, CommandType.StoredProcedure, new SqlParameter("@userSign", userSign));
				dbTransaction.Commit();
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetRangeSlCode(int fldAbs, string slFrom, string slTo, string descr)
	{
		try
		{
			string query = "APZ_WhSublevel_GetRange";
			SqlParameter[] parameters = new SqlParameter[4]
			{
				new SqlParameter("@fldAbs", fldAbs),
				new SqlParameter("@slFrom", slFrom),
				new SqlParameter("@slTo", slTo),
				new SqlParameter("@descr", descr)
			};
			return ExecuteDataTable(query, CommandType.StoredProcedure, parameters);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRange  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
