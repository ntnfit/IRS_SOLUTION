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

public class InOutMethodRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IInOutMethod
{
	private string Temp_Header = "#APZ_OIOM";

	private string Temp_Line = "#APZ_IOM1";

	public DataTable GetAll()
	{
		try
		{
			return ExecuteDataTable("Select * from APZ_OIOM;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetAll  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public string Save(DataSet ds)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.U;
			return Process(ApzObjectTypes.oInOutMethod, -1, transtype, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Remove(int methodId)
	{
		try
		{
			return Process(ApzObjectTypes.oInOutMethod, -1, Enums.TransactionType.D, new DataSet(), Function.ToString(methodId)).Message;
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
			DataTable dataTable = ExecuteDataTable("APZ_InOutMethod_Save", CommandType.StoredProcedure, (SqlParameter[])null);
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
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ID", Function.ToString(objId)));
			int num = ExecuteNonQuery("Delete from APZ_OIOM where \"MethodId\" = @ID;", CommandType.Text, list.ToArray());
			ret.Message = ((num > 0) ? "" : "Delete error! ");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}
}
