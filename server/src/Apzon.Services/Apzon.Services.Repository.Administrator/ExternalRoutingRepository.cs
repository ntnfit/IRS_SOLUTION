using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class ExternalRoutingRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IExternalRouting
{
	private string Temp_OURI = "#APZ_OURI";

	private string Temp_URI1 = "#APZ_URI1";

	private string Temp_URI2 = "#APZ_URI2";

	public DataTable GetAllRoutings()
	{
		try
		{
			string query = "Select * from APZ_OURI;";
			List<SqlParameter> list = new List<SqlParameter>();
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetLines(int absEntry)
	{
		try
		{
			string query = "SELECT * FROM APZ_URI1 where \"AbsEntry\" = @absEntry;\r\n\t\t\t\t\t\t\t\tSELECT * FROM APZ_URI2 where \"AbsEntry\" = @absEntry;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@absEntry", absEntry));
			return ExecuteData(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public string SaveDoc(DataSet ds)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			return Process(ApzObjectTypes.oExternalRouting, -1, transtype, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDoc(int absEntry)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("AbsEntry", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["AbsEntry"] = absEntry;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			return Process(ApzObjectTypes.oExternalRouting, -1, Enums.TransactionType.D, dataSet).Message;
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
			ProcessDelete(ds, ref ret, ref objId, ref objNum);
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
			}
			else
			{
				_ = transtype;
				CreateTempTable(dtS.Tables[0], Temp_OURI);
				BulkCopy(dtS.Tables[0], Temp_OURI);
				CreateTempTable(dtS.Tables[1], Temp_URI1);
				BulkCopy(dtS.Tables[1], Temp_URI1);
				CreateTempTable(dtS.Tables[2], Temp_URI2);
				BulkCopy(dtS.Tables[2], Temp_URI2);
				DataTable dataTable = ExecuteDataTable("apz_process_SaveExternalRouting", CommandType.StoredProcedure, (SqlParameter[])null);
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
			if (!string.IsNullOrEmpty(ret.Message))
			{
				objId = "";
				objNum = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	private void ProcessDelete(DataSet ds, ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@absEntry", Function.ToString(ds.Tables[0].Rows[0]["AbsEntry"])));
			int num = ExecuteNonQuery("DELETE FROM APZ_OURI where \"AbsEntry\" = @absEntry;\r\n\t\t\t\t\t\t\t\t\t\tDELETE FROM APZ_URI1 where \"AbsEntry\" = @absEntry;\r\n\t\t\t\t\t\t\t\t\t\tDELETE FROM APZ_URI2 where \"AbsEntry\" = @absEntry;", CommandType.Text, list.ToArray());
			ret.Message = ((num > 0) ? "" : "Delete error! ");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
			ret.Message = "Delete error: " + ex.Message;
		}
	}
}
