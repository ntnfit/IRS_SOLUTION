using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources;

namespace Apzon.Services.Repository.HumanResources;

public class PenalTypeRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IPenalType
{
	private string Temp_PenalType = "#APZ_OHPT";

	public DataTable GetData()
	{
		try
		{
			return ExecuteDataTable("select *,  (case when \"Type\" = 'P' then 'Penalty' else 'Bonus'  end) as \"TypeGroup\" from APZ_OHPT order by \"Type\";", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadPenalType  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public string Save(DataTable dt)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			return Process(ApzObjectTypes.oPenalType, -1, transtype, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
			ProcessSave(ref transtype, dt, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.U:
			ProcessSave(ref transtype, dt, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.D:
			ret = ProcessDelete(ref objId, ref objNum, bplId);
			break;
		}
		return ret;
	}

	private void ProcessSave(ref Enums.TransactionType transtype, DataTable dtS, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (!dtS.IsNotNull())
			{
				ret.Message = "No data to save";
				return;
			}
			_ = transtype;
			if (dtS.Columns.Contains("TypeGroup"))
			{
				dtS.Columns.Remove("TypeGroup");
			}
			CreateTempTable(dtS, Temp_PenalType);
			BulkCopy(dtS, Temp_PenalType);
			DataTable dataTable = ExecuteDataTable("APZ_SavePenalType", CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
				string text = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
				ret.Message = text + " - " + ret.Message;
			}
			else
			{
				ret.Message = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	public HttpResult ProcessDelete(ref string objId, ref string objNum, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			SqlParameter item = new SqlParameter("@code", objId);
			list.Add(item);
			if (ExecuteNonQuery(GetDeleteQuery(), CommandType.Text, list.ToArray()) > 0)
			{
				return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", objId));
			}
			return new HttpResult(Enums.MessageCode.Error, "Cannot remove name because it has been used in Employee Penalization!");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "RemoveEmpPenalization  FAILED ", ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual string GetDeleteQuery()
	{
		return "Delete from APZ_OHPT where  \"Code\" NOT IN(select \"PenalizationType\" from APZ_HPT1) and \"Code\" = @code;";
	}

	public string Remove(int code)
	{
		try
		{
			return Process(ApzObjectTypes.oPenalType, -1, Enums.TransactionType.D, new DataTable(), Function.ToString(code)).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "RemoveEmpPenalization  FAILED ", ex.Message);
			return ex.Message;
		}
	}
}
