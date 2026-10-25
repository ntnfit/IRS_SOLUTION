using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources.TimeSheeting;

namespace Apzon.Services.Repository.HumanResources.TimeSheeting;

public class WorkShiftRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IWorkShift
{
	private string Temp_Header = "#APZ_OEWS";

	private string Temp_Line = "#APZ_EWS1";

	public DataTable GetAll()
	{
		try
		{
			return ExecuteDataTable("Select * from APZ_OEWS;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetAll  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetLine(int shiftID)
	{
		try
		{
			return ExecuteDataTable("Select * from APZ_EWS1 Where \"ShiftID\" = @_id;", CommandType.Text, new SqlParameter("@_id", shiftID));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetLine  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public string Save(DataSet ds)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.U;
			return Process(ApzObjectTypes.oWorkShift, -1, transtype, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Remove(int shiftId)
	{
		try
		{
			return Process(ApzObjectTypes.oWorkShift, -1, Enums.TransactionType.D, new DataSet(), Function.ToString(shiftId)).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
			ProcessSave(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.U:
			ProcessSave(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.D:
			ProcessDelete(ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessSave(ref Enums.TransactionType transtype, DataSet dtS, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (!dtS.IsNotNull())
			{
				ret.Message = "No data to save";
				return;
			}
			CreateTempTable(dtS.Tables[0], Temp_Header);
			BulkCopy(dtS.Tables[0], Temp_Header);
			CreateTempTable(dtS.Tables[1], Temp_Line);
			BulkCopy(dtS.Tables[1], Temp_Line);
			DataTable dataTable = ExecuteDataTable("APZ_WorkShift_Save", CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
				{
					objId = Function.ToString(dataTable.Rows[0][0]);
					objNum = objId;
					ret.Message = "";
				}
				else
				{
					ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
					string text = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
					ret.Message = text + " - " + ret.Message;
				}
			}
			else
			{
				ret.Message = "Save Error!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	private void ProcessDelete(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			new List<SqlParameter>();
			int num = ExecuteNonQuery("Delete from APZ_OEWS where \"ShiftID\"=@id;\r\n                                    Delete from APZ_EWS1 where \"ShiftID\"=@id;", CommandType.Text, new SqlParameter("@id", objId));
			ret.Message = ((num > 0) ? "" : "Delete error! ");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}
}
