using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.KPIs;

namespace Apzon.Services.Repository.KPIs;

public class KPIsSettingRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IKPIsSetting
{
	private string Temp_APZ_D_OKPM = "#APZ_D_OKPM";

	private string Temp_APZ_D_KPM1 = "#APZ_D_KPM1";

	private string Temp_APZ_D_KPM2 = "#APZ_D_KPM2";

	protected string saveType = "";

	public DataSetResult GetKPIsSetting()
	{
		try
		{
			return new DataSetResult("", ExecuteData("Apz_D_KPIs_GetKPIsSetting", CommandType.StoredProcedure));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public DataTable LoadCopySetting(int absEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("absEntry", absEntry));
			return ExecuteDataTable("Apz_D_KPIs_LoadCopySetting", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSetResult GetDetail(int absEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("absEntry", absEntry));
			return new DataSetResult("", ExecuteData("Apz_D_KPIs_GetKPIsSettingDetail", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public HttpResult SaveCopySetting(DataTable dt)
	{
		try
		{
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dt);
			saveType = "C";
			return Process(ApzObjectTypes.oKPIsSetting, -1, Enums.TransactionType.A, dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Save(DataSet ds, string action)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			saveType = "N";
			switch (action)
			{
			case "A":
				transtype = Enums.TransactionType.A;
				break;
			case "U":
				transtype = Enums.TransactionType.U;
				break;
			case "D":
				transtype = Enums.TransactionType.D;
				break;
			}
			return Process(ApzObjectTypes.oKPIsSetting, -1, transtype, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
		{
			string text = saveType;
			if (!(text == "N"))
			{
				if (!(text == "C"))
				{
					break;
				}
				return ProcessCopy(dsSave, transtype, ref objId, ref objNum);
			}
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		}
		case Enums.TransactionType.D:
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		}
		return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
	}

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			if (ds == null || (ds.Tables.Count < 2 && tranType != Enums.TransactionType.D) || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				httpResult.Message = "No data to save";
				return httpResult;
			}
			DataTable dataTable = ds.Tables[0];
			DataTable dataTable2 = GetTableStruct("APZ_D_OKPM").Clone();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DataRow dataRow = dataTable.Rows[i];
				DataRow dataRow2 = dataTable2.NewRow();
				for (int j = 0; j < dataTable2.Columns.Count; j++)
				{
					string columnName = dataTable2.Columns[j].ColumnName;
					if (dataTable.Columns.Contains(columnName))
					{
						dataRow2[columnName] = dataRow[columnName];
					}
				}
				dataTable2.Rows.Add(dataRow2);
			}
			CreateTempTable(dataTable2, Temp_APZ_D_OKPM);
			BulkCopy(dataTable2, Temp_APZ_D_OKPM);
			if (tranType != Enums.TransactionType.D)
			{
				CreateTempTable(ds.Tables[1], Temp_APZ_D_KPM1);
				BulkCopy(ds.Tables[1], Temp_APZ_D_KPM1);
			}
			SqlParameter sqlParameter = new SqlParameter("@type", Function.ToString(tranType));
			DataTable dataTable3 = ExecuteDataTable("apz_process_SaveKPIsSetting", CommandType.StoredProcedure, sqlParameter);
			if (dataTable3.IsNotNull())
			{
				if (Function.ParseInt(dataTable3.Rows[0][0]) > 0 && dataTable3.Columns.Count <= 1)
				{
					objId = Function.ToString(dataTable3.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable3.Rows[0][0])}-{Function.ToString(dataTable3.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.D) ? "Delete error!" : "Save error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult ProcessCopy(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			if (ds == null || (ds.Tables.Count < 1 && tranType != Enums.TransactionType.D) || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				httpResult.Message = "No data to save";
			}
			else
			{
				DataTable dataTable = ds.Tables[0];
				DataTable dataTable2 = ExecuteDataTable("APZ_D_KPIs_CopyKpiSetting", CommandType.StoredProcedure, new SqlParameter("@AbsEntry", Function.ToString(dataTable.Rows[0]["AbsEntry"])), new SqlParameter("@CopyEmployees", Function.ToString(dataTable.Rows[0]["CopyEmployees"])), new SqlParameter("@CopyAll", Function.ToString(dataTable.Rows[0]["CopyAll"])), new SqlParameter("@UserUpdate", Function.ToString(dataTable.Rows[0]["UserUpdate"])));
				List<string> list = new List<string>();
				foreach (DataRow row in dataTable2.Rows)
				{
					list.Add(Function.ToString(row[0]));
				}
				objId = string.Join(";", list.ToArray());
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
