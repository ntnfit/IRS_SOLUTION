using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class AdditionalGeneralSettingsRepository : BaseService<DataSet>, IAdditionalGeneralSettings
{
	public AdditionalGeneralSettingsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public HttpResult Process_APZ_AdditionalGeneralSettings(DataSet ds, Enums.TransactionType tranType)
	{
		string query = "APZ_PROCESS_ADDITIONALGENERALSETTINGS";
		HttpResult httpResult = new HttpResult();
		try
		{
			CreateTempTable(ds.Tables[0], "#APZ_OADM");
			BulkCopy(ds.Tables[0], "#APZ_OADM");
			CreateTempTable(ds.Tables[1], "#APZ_ATC2");
			BulkCopy(ds.Tables[1], "#APZ_ATC2");
			CreateTempTable(ds.Tables[2], "#APZ_D_OAPC");
			BulkCopy(ds.Tables[2], "#APZ_D_OAPC");
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@type", Function.ToString(tranType)));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				httpResult.Message = Function.ToString(dataTable.Rows[0]["ErrorCode"]) + " - " + Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), dataTable.Rows[0]["ErrorMessage"]?.ToString() ?? "");
			}
			else
			{
				httpResult.Message = "";
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			httpResult.Message = "Save Failed";
			return httpResult;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		HttpResult result = new HttpResult();
		try
		{
			if (transtype == Enums.TransactionType.A)
			{
				result = Process_APZ_AdditionalGeneralSettings(document, transtype);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return result;
		}
		return result;
	}

	public DataSet GetData()
	{
		try
		{
			return ExecuteData("APZ_AdditionalGeneralSettings_GetData", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public HttpResult Save(DataSet ds)
	{
		return Process(ApzObjectTypes.oAdditionalGeneralSettings, -1, Enums.TransactionType.A, ds);
	}

	public string GetDefaultLng()
	{
		try
		{
			string text = Function.ToString(ExecuteScalar("select top 1 \"DflLng\" from APZ_OADM;", CommandType.Text));
			return string.IsNullOrEmpty(text) ? "en-US" : text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "en-US";
		}
	}
}
