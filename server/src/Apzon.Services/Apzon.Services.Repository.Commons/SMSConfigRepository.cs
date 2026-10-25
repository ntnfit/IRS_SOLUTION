using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class SMSConfigRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), ISMSConfig
{
	private string TempTable = "#APZ_SMSC";

	public DataTable GetConfig()
	{
		try
		{
			string query = "Select Top 1 * from APZ_SMSC;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string SaveConfig(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oSMSConfig, -1, Enums.TransactionType.A, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		string text = "";
		try
		{
			CreateTempTable(document, TempTable);
			BulkCopy(document, TempTable);
			text = "APZ_SMSConfig_Save";
			object obj = ExecuteScalar(text, CommandType.StoredProcedure);
			objId = Function.ToString(obj);
			objNum = objId;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}
}
