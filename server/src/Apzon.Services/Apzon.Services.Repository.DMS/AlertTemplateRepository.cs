using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

public class AlertTemplateRepository : BaseService<DataTable>, IAlertTemplate
{
	public AlertTemplateRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		try
		{
			CreateTempTable(document, "#APZ_D_OALT");
			BulkCopy(document, "#APZ_D_OALT");
			DataTable dataTable = ExecuteDataTable("APZ_D_Process_AlertTemplate", CommandType.StoredProcedure, new SqlParameter("@type", (transtype == Enums.TransactionType.A) ? "A" : "U"));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return new HttpResult(Enums.MessageCode.Error, 20600003 + " - " + Function.ToString(dataTable.Rows[0]["ErrorMessage"]));
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetAllTemplate()
	{
		try
		{
			return ExecuteDataTable("APZ_D_AlertTemplate_GetData", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveData(DataTable dt, string type)
	{
		try
		{
			if (type == "A")
			{
				return Process(ApzObjectTypes.oAlertTemplate, -1, Enums.TransactionType.A, dt);
			}
			return Process(ApzObjectTypes.oAlertTemplate, -1, Enums.TransactionType.U, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Remove(string code)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_D_AlertTemplate_Remove", CommandType.StoredProcedure, new SqlParameter("@code", Function.ToString(code)));
			if (dataTable.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][0])))
			{
				return new HttpResult(Enums.MessageCode.Error, 20600013 + " - " + Function.ToString(dataTable.Rows[0][0]));
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
