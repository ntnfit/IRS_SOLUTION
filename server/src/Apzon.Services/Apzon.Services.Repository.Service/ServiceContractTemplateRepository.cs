using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Service;

namespace Apzon.Services.Repository.Service;

public class ServiceContractTemplateRepository : BaseService<DataTable>, IServiceContractTemplate
{
	private const string tempTable = "#APZ_OCTT";

	private const string tempTableDelete = "#APZ_OCTT_DELETE";

	public ServiceContractTemplateRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable SearchData(DataTable dt)
	{
		try
		{
			DataRow dataRow = dt.Rows[0];
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Type", Function.ToString(dataRow["Type"])));
			list.Add(new SqlParameter("@ContractTemplateNum", Function.ToString(dataRow["ContractTemplateNum"])));
			list.Add(new SqlParameter("@SLACode", Function.ToString(dataRow["SLACode"])));
			list.Add(new SqlParameter("@Status", Function.ToString(dataRow["Status"])));
			return ExecuteDataTable("APZ_ServiceContractTemplate_Search", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetDataDoc(int id, Enums.NavigateType type)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@type", (int)type));
			list.Add(new SqlParameter("@Id", id));
			return ExecuteDataTable("APZ_ServiceContractTemplate_GetDataDoc", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public HttpResult SaveData(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oServiceContractTemplate, -1, Enums.TransactionType.U, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult();
		}
	}

	public HttpResult DeleteData(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oServiceContractTemplate, -1, Enums.TransactionType.D, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult();
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		HttpResult httpResult = new HttpResult();
		string message = "";
		switch (transtype)
		{
		case Enums.TransactionType.U:
			message = Update(objtype, bplId, dt, ref objId, ref objNum, ref transtype).Message;
			break;
		case Enums.TransactionType.D:
			message = Delete(objtype, bplId, dt, ref objId, ref objNum, ref transtype).Message;
			break;
		}
		httpResult.Message = message;
		return httpResult;
	}

	public HttpResult Update(ApzObjectTypes objType, int bplId, DataTable dt, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		try
		{
			objId = "";
			objNum = "";
			CreateTempTable(dt, "#APZ_OCTT");
			BulkCopy(dt, "#APZ_OCTT");
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@RowCount", Function.ParseInt(0)));
			DataTable dataTable = ExecuteDataTable("APZ_ServiceContractTemplate_SaveData", CommandType.StoredProcedure, list.ToArray());
			return new HttpResult(Enums.MessageCode.Success, Function.ToString(dataTable.Rows[0][1]));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Delete(ApzObjectTypes objType, int bplId, DataTable dt, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		try
		{
			objId = "";
			objNum = "";
			CreateTempTable(dt, "#APZ_OCTT_DELETE");
			BulkCopy(dt, "#APZ_OCTT_DELETE");
			string query = DeleteContractTemplateQuery();
			new List<SqlParameter>().Add(new SqlParameter("@contractID", Function.ParseInt(dt.Rows[0][0])));
			if (ExecuteNonQuery(query, CommandType.Text) > 0)
			{
				return new HttpResult();
			}
			return new HttpResult(Enums.MessageCode.Error, "Failed");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual string DeleteContractTemplateQuery()
	{
		return "delete a from APZ_OCTT a inner join #APZ_OCTT_DELETE b on a.ContractID =b.ContractID";
	}
}
