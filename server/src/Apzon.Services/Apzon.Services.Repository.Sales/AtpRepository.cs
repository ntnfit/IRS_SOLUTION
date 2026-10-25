using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Sales;

namespace Apzon.Services.Repository.Sales;

public class AtpRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IAtp
{
	private readonly string[] _inputColum = new string[4] { "ItemCode", "WhsCode", "ShipDate", "Qty" };

	private const string TempAtpDoc = "#ATPDOC";

	private const string TempAtpInput = "#ATPINPUT";

	private const string TempAtpLine = "#ATPLINE";

	private const string TempScheduleDoc = "#ATPSCHEDULEDOC";

	private const string TempScheduleLine = "#ATPSCHEDULELINE";

	public DataSet Search(DataTable dtSearch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			DataRow dataRow = dtSearch.Rows[0];
			DateTime? dateTime = Function.ParseDateTime(dataRow["fDate"]);
			DateTime? dateTime2 = Function.ParseDateTime(dataRow["tDate"]);
			string value = Function.ToString(dataRow["cardCode"]);
			string value2 = Function.ToString(dataRow["whsCode"]);
			string value3 = Function.ToString(dataRow["itemCode"]);
			int num = Function.ParseInt(dataRow["OrderNo"]);
			DateTime? dateTime3 = Function.ParseDateTime(dataRow["delDate"]);
			if (dateTime.HasValue)
			{
				list.Add(new SqlParameter("@FromDate", dateTime));
			}
			if (dateTime2.HasValue)
			{
				list.Add(new SqlParameter("@ToDate", dateTime2));
			}
			if (!string.IsNullOrEmpty(value))
			{
				list.Add(new SqlParameter("@CardCode", value));
			}
			if (!string.IsNullOrEmpty(value3))
			{
				list.Add(new SqlParameter("@ItemCode", value3));
			}
			if (!string.IsNullOrEmpty(value2))
			{
				list.Add(new SqlParameter("@WhsCode", value2));
			}
			if (num != 0)
			{
				list.Add(new SqlParameter("@OrderNo", num));
			}
			if (dateTime3.HasValue)
			{
				list.Add(new SqlParameter("@DelDate", dateTime3));
			}
			return ExecuteData("apz_atp", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string Insert(Atp entity)
	{
		throw new NotImplementedException();
	}

	public string Update(Atp entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(Atp entity)
	{
		throw new NotImplementedException();
	}

	public Atp GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable GetDataAtpType()
	{
		try
		{
			return ExecuteDataTable("select \"TypeCode\" as \"Code\", \"TypeName\" as \"Name\" from APZ_OTCF where \"ObjectCode\" = @objCode;", CommandType.Text, new SqlParameter("@objCode", Function.ToString(700013)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public HttpResult GetNewIdAtp()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select max(\"Code\") from APZ_OATP;", CommandType.Text);
			int num = Function.ParseInt(GetObjectId(ApzObjectTypes.oAtp));
			if (dataTable.IsNotNull() && Function.ParseInt(dataTable.Rows[0][0]) >= num)
			{
				return new HttpResult(Enums.MessageCode.Error, "Object atp code had been changed");
			}
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetStructureInput()
	{
		try
		{
			return ExecuteDataTable("SELECT TOP 0 * FROM APZ_ATP2;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDocStructure()
	{
		try
		{
			return ExecuteDataTable("SELECT TOP 0 * FROM APZ_OATP;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable RunAtp(DataSet ds)
	{
		try
		{
			if (ds == null || ds.Tables.Count != 2 || !ds.Tables[0].IsNotNull())
			{
				return new DataTable();
			}
			DataTable value = ds.Tables[1].DefaultView.ToTable(distinct: false, _inputColum);
			return ExecuteDataTable("USP_APZ_ATP", CommandType.StoredProcedure, new SqlParameter("@fdate", Function.ParseDateTime(ds.Tables[0].Rows[0]["FromDate"])), new SqlParameter("@tdate", Function.ParseDateTime(ds.Tables[0].Rows[0]["ToDate"])), new SqlParameter("@INPUT", value));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataSet ds, Enums.TransactionType tranType, string code)
	{
		try
		{
			return Process(ApzObjectTypes.oAtp, -1, tranType, ds, code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		if (objtype == ApzObjectTypes.oAtp)
		{
			switch (transtype)
			{
			case Enums.TransactionType.A:
			case Enums.TransactionType.U:
				return AddSave(dsSave, transtype, ref objId, ref objNum);
			}
		}
		else
		{
			switch (transtype)
			{
			case Enums.TransactionType.A:
			case Enums.TransactionType.U:
				return SaveScheduleAtp(dsSave, transtype, ref objId, ref objNum);
			}
		}
		return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
	}

	private HttpResult AddSave(DataSet dsSave, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		if (dsSave == null || dsSave.Tables.Count != 3 || !dsSave.Tables[0].IsNotNull() || !dsSave.Tables[2].IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Data could not null");
		}
		CreateTempTable(dsSave.Tables[0], "#ATPDOC");
		BulkCopy(dsSave.Tables[0], "#ATPDOC");
		CreateTempTable(dsSave.Tables[1], "#ATPINPUT");
		BulkCopy(dsSave.Tables[1], "#ATPINPUT");
		CreateTempTable(dsSave.Tables[2], "#ATPLINE");
		BulkCopy(dsSave.Tables[2], "#ATPLINE");
		DataTable dataTable = ExecuteDataTable("APZ_ATP_Save", CommandType.StoredProcedure, new SqlParameter("@type", (int)tranType));
		if (Function.ParseInt(dataTable.Rows[0][0]) > 0)
		{
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])} - {Function.ToString(dataTable.Rows[0][1])}");
		}
		objId = Function.ToString(dataTable.Rows[0][1]);
		objNum = Function.ToString(dataTable.Rows[0][1]);
		return new HttpResult(Enums.MessageCode.Success, "", Function.ParseInt(dataTable.Rows[0][1]));
	}

	public DataSet SearchData(Enums.NavigateType type, int code)
	{
		try
		{
			return ExecuteData("APZ_ATP_SearchData", CommandType.StoredProcedure, new SqlParameter("@type", (int)type), new SqlParameter("@code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetDataGenPurchase(string input, int code, string user)
	{
		try
		{
			return ExecuteDataTable("APZ_Atp_GetDataPurchaseFormAtp", CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@user", user), new SqlParameter("@input", input));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetStructureGenPurchase()
	{
		try
		{
			return ExecuteDataTable("APZ_GenPurchaseFromAtp_GetStructure", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult GetNewIdScheduleForAtp()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select max(\"Code\") from APZ_OSAT;", CommandType.Text);
			int num = Function.ParseInt(GetObjectId(ApzObjectTypes.oScheduleForAtp));
			if (dataTable.IsNotNull() && Function.ParseInt(dataTable.Rows[0][0]) >= num)
			{
				return new HttpResult(Enums.MessageCode.Error, "Object schedule for atp code had been changed");
			}
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataSet RunSchedule(string code, string type)
	{
		try
		{
			return ExecuteData("APZ_Atp_Schedule_Run", CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetDataLineSchedule(int id)
	{
		try
		{
			return ExecuteData("APZ_Atp_Schedule_GetDataLine", CommandType.StoredProcedure, new SqlParameter("@code", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetDocScheduleStructure()
	{
		try
		{
			return ExecuteDataTable("Select top 0 * from APZ_OSAT;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDataAtpForSchedle(int code)
	{
		try
		{
			return ExecuteData("APZ_Atp_Schedule_GetDataDocToAddNew", CommandType.StoredProcedure, new SqlParameter("@code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public HttpResult SaveSchedule(DataSet dtSet, Enums.TransactionType tranType, string code)
	{
		try
		{
			return Process(ApzObjectTypes.oScheduleForAtp, -1, tranType, dtSet, code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetDataDocSchedule(int code, Enums.NavigateType type)
	{
		try
		{
			return ExecuteDataTable("APZ_Atp_Schedule_GetDataDoc", CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@type", (int)type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDataStructureGenProduction()
	{
		try
		{
			return ExecuteData("APZ_GenProductionFromAtp_GetStructure", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	private HttpResult SaveScheduleAtp(DataSet dsSave, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		if (dsSave == null || dsSave.Tables.Count != 2 || !dsSave.Tables[0].IsNotNull() || !dsSave.Tables[1].IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Data could not null");
		}
		CreateTempTable(dsSave.Tables[0], "#ATPSCHEDULEDOC");
		BulkCopy(dsSave.Tables[0], "#ATPSCHEDULEDOC");
		CreateTempTable(dsSave.Tables[1], "#ATPSCHEDULELINE");
		BulkCopy(dsSave.Tables[1], "#ATPSCHEDULELINE");
		DataTable dataTable = ExecuteDataTable("APZ_ATP_Schedule_Save", CommandType.StoredProcedure, new SqlParameter("@type", (int)tranType));
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
			{
				objId = Function.ToString(dataTable.Rows[0][0]);
				objNum = objId;
				return new HttpResult(Enums.MessageCode.Success, "", objId);
			}
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
		}
		return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Save error!" : "Update error");
	}

	public DataTable GetDataGenProduction(string id, string type)
	{
		try
		{
			return ExecuteDataTable("APZ_GenProductionFromAtp", CommandType.StoredProcedure, new SqlParameter("@atpCode", id), new SqlParameter("@type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "APZ_GenProductionFromAtp  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public string CheckInputData(string type, int code, string genType)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_GenPurchseFromAtp_CheckInputData", CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@type", type), new SqlParameter("@genType", genType));
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "can not check data input";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
