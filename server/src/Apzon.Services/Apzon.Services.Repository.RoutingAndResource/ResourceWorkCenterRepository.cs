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

public class ResourceWorkCenterRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IResourceWorkCenter
{
	public string tempTable = "#APZ_ORSC";

	public string tempTableDelete = "#APZ_ORSC_Delete";

	public DataTable GetDataResourceWorkCenter()
	{
		try
		{
			return ExecuteDataTable("select * from orsc;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetDataDoc(string resCode, Enums.NavigateType type)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@resCode", resCode));
			list.Add(new SqlParameter("@type", (int)type));
			return ExecuteDataTable("APZ_ResourceWorkCenter_GetDataDoc", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataSet GetDataOriginal()
	{
		try
		{
			return ExecuteData("select \"ResGrpNam\", \"ResGrpCod\" from ORSB;\r\n                                    select \"Code\" as \"DeptCode\",\"Name\" from OUDP;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataSet();
		}
	}

	public DataTable SearchResourceWorkCenter(DataTable dtSearch)
	{
		try
		{
			DataRow dataRow = dtSearch.Rows[0];
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@resCode", dataRow["ResCode"]));
			list.Add(new SqlParameter("@resGroup", dataRow["ResGroup"]));
			list.Add(new SqlParameter("@valid", dataRow["Valid"]));
			list.Add(new SqlParameter("@calCode", dataRow["CalCode"]));
			list.Add(new SqlParameter("@cardCode", dataRow["CardCode"]));
			list.Add(new SqlParameter("@deptCode", dataRow["DeptCode"]));
			return ExecuteDataTable("APZ_ResourceWorkCenter_Search", CommandType.StoredProcedure, list.ToArray());
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
			return new HttpResult(Enums.MessageCode.Error, "Function not support");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult();
		}
	}

	public HttpResult DeleteResource(DataTable dt)
	{
		try
		{
			return new HttpResult(Enums.MessageCode.Error, "Function not support");
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
			CreateTempTable(dt, tempTable);
			BulkCopy(dt, tempTable);
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@RowCount", Function.ParseInt(0)));
			DataTable dataTable = ExecuteDataTable("APZ_ResourceWorkCenter_SaveData", CommandType.StoredProcedure, list.ToArray());
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
			CreateTempTable(dt, tempTableDelete);
			BulkCopy(dt, tempTableDelete);
			if (ExecuteNonQuery("delete a from APZ_ORSC a inner join #APZ_ORSC_Delete b on a.ResCode = b.ResCode", CommandType.Text) > 0)
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

	public DataTable GetDataWorkCenterByCode(string code)
	{
		try
		{
			return ExecuteDataTable("Select * from ORSC where \"ResCode\" = @code;", CommandType.Text, new SqlParameter("@code", Function.ToString(code)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}
}
