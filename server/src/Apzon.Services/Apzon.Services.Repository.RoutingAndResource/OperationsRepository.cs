using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.RoutingAndResource;

namespace Apzon.Services.Repository.RoutingAndResource;

public class OperationsRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IOperations
{
	private string tempTable = "#APZ_OOMD";

	private string tempTableDelete = "#APZ_OOMD_Delete";

	public DataTable SearchData(DataTable dt)
	{
		try
		{
			DataRow dataRow = dt.Rows[0];
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@OpeCode", Function.ToString(dataRow[0])));
			list.Add(new SqlParameter("@Status", Function.ToString(dataRow[1])));
			return ExecuteDataTable("APZ_Operations_Search", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetDataLine(string opeCode)
	{
		try
		{
			return ExecuteDataTable("select * from APZ_OOMD where \"OpeCode\"=@opeCode;", CommandType.Text, new SqlParameter("@opeCode", Function.ToString(opeCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable OperationsGetDataDoc(string OpeCode, Enums.NavigateType type)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@OpeCode", OpeCode));
			list.Add(new SqlParameter("@type", (int)type));
			return ExecuteDataTable("APZ_Operations_GetDataDoc", CommandType.StoredProcedure, list.ToArray());
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
			return Process(ApzObjectTypes.oOperation, -1, Enums.TransactionType.U, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult DeleteData(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oOperation, -1, Enums.TransactionType.D, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		HttpResult httpResult = new HttpResult();
		string message = "";
		switch (transtype)
		{
		case Enums.TransactionType.U:
			message = Upadate(objtype, bplId, ref transtype, dt, ref objId, ref objNum).Message;
			break;
		case Enums.TransactionType.D:
			message = Delete(objtype, bplId, ref transtype, dt, ref objId, ref objNum).Message;
			break;
		}
		httpResult.Message = message;
		return httpResult;
	}

	public HttpResult Upadate(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		try
		{
			objId = "";
			objNum = "";
			CreateTempTable(dt, tempTable);
			BulkCopy(dt, tempTable);
			DataTable dataTable = ExecuteDataTable("APZ_Operations_SaveData", CommandType.StoredProcedure, new SqlParameter("@RowCount", Function.ParseInt(0)));
			return new HttpResult(Enums.MessageCode.Success, Function.ToString(dataTable.Rows[0][1]));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Delete(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		try
		{
			objId = "";
			objNum = "";
			CreateTempTable(dt, tempTableDelete);
			BulkCopy(dt, tempTableDelete);
			if (ExecuteNonQuery("delete from APZ_OOMD where \"OpeCode\" in (select \"OpeCode\" from \"#APZ_OOMD_Delete\");", CommandType.Text) > 0)
			{
				return new HttpResult();
			}
			return new HttpResult(Enums.MessageCode.Error, "Fail");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
