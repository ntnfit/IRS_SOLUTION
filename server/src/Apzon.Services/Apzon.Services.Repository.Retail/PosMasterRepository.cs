using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Retail;

namespace Apzon.Services.Repository.Retail;

public class PosMasterRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IPosMaster
{
	public string TempTable = "#APZ_RPOS";

	private string _processType;

	public DataTable LoadDoc(string code)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@StoreCode", Function.ToString(code)));
			return ExecuteDataTable("APZ_PosMaster_GetDoc", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable LoadAllDoc()
	{
		try
		{
			return ExecuteDataTable("APZ_PosMaster_GetAllDoc", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string SaveAssignment(DataTable dt)
	{
		_processType = "Assignment";
		return Process(ApzObjectTypes.oRetail, -1, Enums.TransactionType.A, dt).Message;
	}

	public string Save(DataTable dt)
	{
		_processType = "Master";
		return Process(ApzObjectTypes.oRetail, -1, Enums.TransactionType.A, dt).Message;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			if (_processType.ToUpper().Equals("MASTER"))
			{
				return ProcessSavePosMaster(document, ref objId, ref objNum, ref transtype);
			}
			if (_processType.ToUpper().Equals("ASSIGNMENT"))
			{
				return ProcessSaveAssignment(document, ref objId, ref objNum, ref transtype);
			}
			break;
		}
		return new HttpResult();
	}

	private HttpResult ProcessSavePosMaster(DataTable document, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		HttpResult httpResult = new HttpResult();
		try
		{
			if (!document.IsNotNull())
			{
				httpResult.Message = "No data to save";
			}
			else
			{
				CreateTempTable(document, TempTable);
				BulkCopy(document, "#APZ_RPOS");
				DataTable dataTable = ExecuteDataTable("apz_retail_SavePosMaster", CommandType.StoredProcedure, (SqlParameter[])null);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					httpResult.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
					string text = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
					httpResult.Message = text + " - " + httpResult.Message;
				}
				else
				{
					httpResult.Message = "";
				}
			}
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				objId = "";
				objNum = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			httpResult.Message = ex.Message;
		}
		return httpResult;
	}

	private HttpResult ProcessSaveAssignment(DataTable document, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		HttpResult httpResult = new HttpResult();
		try
		{
			if (!document.IsNotNull())
			{
				httpResult.Message = "No data to save";
			}
			else
			{
				object value = document.Rows[0]["UserCode"];
				DataTable dataTable = GetTableStruct("APZ_POS1").Clone();
				for (int i = 0; i < document.Rows.Count; i++)
				{
					DataRow dataRow = document.Rows[i];
					DataRow dataRow2 = dataTable.NewRow();
					if (Function.ToString(dataRow["Checked"]) == "Y")
					{
						dataRow2["UserCode"] = value;
						dataRow2["PosID"] = dataRow["PosID"];
						dataRow2["IsDflt"] = dataRow["IsDflt"];
						dataTable.Rows.Add(dataRow2);
					}
				}
				CreateTempTable(dataTable, "#APZ_POS1");
				BulkCopy(dataTable, "#APZ_POS1");
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@UserCode", value));
				DataTable dataTable2 = ExecuteDataTable("apz_retail_SavePosAssignment", CommandType.StoredProcedure, list.ToArray());
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					httpResult.Message = Function.ToString(dataTable2.Rows[0]["ErrorMessage"]);
					string text = Function.ToString(dataTable2.Rows[0]["ErrorNumber"]);
					httpResult.Message = text + " - " + httpResult.Message;
				}
				else
				{
					httpResult.Message = "";
				}
			}
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				objId = "";
				objNum = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			httpResult.Message = ex.Message;
		}
		return httpResult;
	}

	public string Remove(string code)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@PosID", code));
				DataTable dataTable = ExecuteDataTable("SP_Retail_PosMaster_REMOVE", CommandType.StoredProcedure, list.ToArray());
				string result;
				if (dataTable == null || dataTable.Rows.Count == 0)
				{
					dbTransaction.Commit();
					result = "";
				}
				else
				{
					result = Function.ToString(dataTable.Rows[0][0]);
				}
				return result;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public DataTable GetUserPosAssignment(string userCode)
	{
		try
		{
			new DataTable();
			string query = "APZ_Retail_GetUserPosAssignment";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@UserCode", Function.ToString(userCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}
}
