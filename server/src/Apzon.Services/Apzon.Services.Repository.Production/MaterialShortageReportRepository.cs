using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Services.Interfaces.Production;

namespace Apzon.Services.Repository.Production;

public class MaterialShortageReportRepository : BaseService<DataSet>, IMaterialShortageReport
{
	private const string TempDoc = "#TempDoc";

	private const string TempLine = "#TempLine";

	public MaterialShortageReportRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable Run(MaterialShortageReport_Impl input)
	{
		try
		{
			if (input == null)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Input could not null");
				return new DataTable();
			}
			return ExecuteDataTable("USP_APZ_MATERIAL_SHORTAGE", CommandType.StoredProcedure, new SqlParameter("@fdate", input.FromDate), new SqlParameter("@tdate", input.ToDate), new SqlParameter("@excludePrd", input.XcludProductionOrder));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet SearchData(Enums.NavigateType type, int code)
	{
		try
		{
			return ExecuteData("APZ_MaterialShortageReport_SearchData", CommandType.StoredProcedure, new SqlParameter("@type", (int)type), new SqlParameter("@code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public HttpResult GetNewId()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select max(\"Code\") from APZ_OMSR;", CommandType.Text);
			int num = Function.ParseInt(GetObjectId(ApzObjectTypes.oMaterialShortageReport));
			if (dataTable.IsNotNull() && Function.ParseInt(dataTable.Rows[0][0]) >= num)
			{
				return new HttpResult(Enums.MessageCode.Error, "Object material shortage report had been changed");
			}
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable LoadAtpType()
	{
		try
		{
			return ExecuteDataTable("select \"TypeCode\" \"Code\", \"TypeName\" \"Name\" from APZ_OTCF where \"ObjectCode\" = @objCode;", CommandType.Text, new SqlParameter("@objCode", Function.ToString(700016)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDocStructure()
	{
		try
		{
			return ExecuteDataTable("Select Top 0 * from APZ_OMSR;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataSet ds, Enums.TransactionType tranType, string code)
	{
		try
		{
			return Process(ApzObjectTypes.oMaterialShortageReport, -1, tranType, ds, code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetStructureGenPurchase()
	{
		try
		{
			return ExecuteDataTable("APZ_GenPurchaseFromMaterialShortage_GetStructure", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataGenPurchase(string input, int code, string user)
	{
		try
		{
			return ExecuteDataTable("APZ_MaterialShortage_GetDataPurchase", CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@user", user), new SqlParameter("@input", input));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string CheckInputData(string type, int code, string genType)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select \"Status\" from APZ_OMSR where \"Code\" = @code;", CommandType.Text, new SqlParameter("@code", code));
			if (!dataTable.IsNotNull())
			{
				return "You must choose material shortage code";
			}
			if (!Function.ToString(dataTable.Rows[0][0]).Equals("A"))
			{
				return "You must choose approval material shortage";
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		Enums.TransactionType transactionType = transtype;
		if ((uint)transactionType <= 1u)
		{
			return AddSave(dsSave, transtype, ref objId, ref objNum);
		}
		return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
	}

	private HttpResult AddSave(DataSet dsSave, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		if (dsSave == null || dsSave.Tables.Count != 2 || !dsSave.Tables[0].IsNotNull() || !dsSave.Tables[1].IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Data could not null");
		}
		CreateTempTable(dsSave.Tables[0], "#TempDoc");
		BulkCopy(dsSave.Tables[0], "#TempDoc");
		CreateTempTable(dsSave.Tables[1], "#TempLine");
		BulkCopy(dsSave.Tables[1], "#TempLine");
		DataTable dataTable = ExecuteDataTable("APZ_MaterialShortageReport_Save", CommandType.StoredProcedure, new SqlParameter("@type", tranType.ToString()));
		if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
		{
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])} - {Function.ToString(dataTable.Rows[0][1])}");
		}
		objId = Function.ToString(dataTable.Rows[0][0]);
		objNum = Function.ToString(dataTable.Rows[0][0]);
		return new HttpResult(Enums.MessageCode.Success, "", Function.ParseInt(dataTable.Rows[0][0]));
	}

	public DataSet GetListMaterialShortageReport(int bplId, string type, string status)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListMaterialShortageReport", CommandType.StoredProcedure, new SqlParameter("@bplId", bplId), new SqlParameter("@type", type), new SqlParameter("@status", status));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}
}
