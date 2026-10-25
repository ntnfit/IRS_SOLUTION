using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Mrps;
using Apzon.Services.Interfaces.Mrps;

namespace Apzon.Services.Repository.Mrps;

public class AdvanceForecastRepository : BaseService<DataSet>, IAdvanceForecast
{
	private const string TableDocName = "#APZDoc";

	private const string TableLineName = "#APZLine";

	public AdvanceForecastRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDataDoc(string forecastCode, string view, Enums.NavigateType type)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@forecastCode", forecastCode),
				new SqlParameter("@view", view),
				new SqlParameter("@type", (int)type)
			};
			return ExecuteDataTable("APZ_AdvanceForecast_GetDataDoc", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDataLine(int absId)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@AbsID", absId);
			return ExecuteData("APZ_AdvanceForecast_GetDataLine", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetColumnDoc()
	{
		try
		{
			return ExecuteDataTable("APZ_AdvanceForecast_GetColumnsDoc", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetColumnLine()
	{
		try
		{
			return ExecuteDataTable("APZ_AdvanceForecast_GetColumnLine", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetSettingSap()
	{
		try
		{
			return ExecuteDataTable("APZ_AdvanceForecast_GetSettingSap", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult DeleteForecast(int absId, ref string objId, ref string objNum)
	{
		SqlParameter sqlParameter = new SqlParameter("@absId", absId);
		DataTable dataTable = ExecuteDataTable("APZ_AdvanceForecast_Delete", CommandType.StoredProcedure, sqlParameter);
		if (dataTable.IsNotNull() && Function.ParseInt(dataTable.Rows[0][0]) == 0)
		{
			objId = (objNum = Function.ToString(absId));
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, $"{Function.ToString(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
	}

	public HttpResult Save(DataSet dtset, Enums.TransactionType tranType, string objId)
	{
		try
		{
			return Process(ApzObjectTypes.oAdvanceForecast, -1, tranType, dtset, objId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
			return DeleteForecast(Function.ParseInt(objId), ref objId, ref objNum);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataSet dsSave, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		if (dsSave == null || dsSave.Tables.Count < 2 || dsSave.Tables[0] == null || dsSave.Tables[0].Rows.Count == 0 || dsSave.Tables[1] == null || dsSave.Tables[1].Rows.Count == 0)
		{
			return new HttpResult(Enums.MessageCode.Error, "Data could not null");
		}
		DataTable tableStruct = GetTableStruct("APZ_OFCT");
		if (!tableStruct.IsNotNull())
		{
			throw new Exception("Could not get data doc structure");
		}
		IEnumerable<string> source = from DataColumn t in tableStruct.Columns
			select t.ColumnName;
		DataTable dt = dsSave.Tables[0].DefaultView.ToTable(distinct: false, source.ToArray());
		CreateTempTable(dt, "#APZDoc");
		BulkCopy(dt, "#APZDoc");
		DataTable tableStruct2 = GetTableStruct("APZ_FCT1");
		if (!tableStruct2.IsNotNull())
		{
			throw new Exception("Could not get data Line structure");
		}
		IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
			select t.ColumnName;
		DataTable dt2 = dsSave.Tables[1].DefaultView.ToTable(distinct: false, source2.ToArray());
		CreateTempTable(dt2, "#APZLine");
		BulkCopy(dt2, "#APZLine");
		SqlParameter sqlParameter = new SqlParameter("@Type", tranType.ToString());
		DataTable dataTable = ExecuteDataTable("APZ_AdvanceForecast_Save", CommandType.StoredProcedure, sqlParameter);
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
			{
				objId = (objNum = Function.ToString(dataTable.Rows[0][0]));
				return new HttpResult();
			}
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
		}
		return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Save error" : "Update error");
	}

	public DataTable GetDataItemCode(string itemCode)
	{
		try
		{
			return ExecuteDataTable("APZ_AdvanceForecast_GetDataItemByCode", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataPrice(string priceList)
	{
		try
		{
			return ExecuteDataTable("APZ_AdvanceForecast_GetDataPrice", CommandType.StoredProcedure, new SqlParameter("@priceList", priceList));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult GetNewIdForecast()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select max(\"Code\") from APZ_OFCT;", CommandType.Text);
			int num = Function.ParseInt(GetObjectId(ApzObjectTypes.oAdvanceForecast));
			if (dataTable.IsNotNull() && Function.ParseInt(dataTable.Rows[0][0]) >= num)
			{
				return new HttpResult(Enums.MessageCode.Error, "Object forecast code had been changed");
			}
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataSet GenerateForecast(InputGenerateForecastImpl inputGen)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@fdate", inputGen.FromDate),
				new SqlParameter("@tdate", inputGen.ToDate),
				new SqlParameter("@fitem", inputGen.FromItem),
				new SqlParameter("@titem", inputGen.ToItem),
				new SqlParameter("@itemGroup", inputGen.ItemGroup),
				new SqlParameter("@whscode", inputGen.WhsCode),
				new SqlParameter("@viewby", inputGen.ViewBy),
				new SqlParameter("@bwlength", inputGen.BwLength),
				new SqlParameter("@fwlength", inputGen.FwLength),
				new SqlParameter("@bwbase", inputGen.BwBase),
				new SqlParameter("@fwbase", inputGen.FwBase),
				new SqlParameter("@ignore", inputGen.IgnorePropeties),
				new SqlParameter("@compareType", inputGen.CompareType),
				new SqlParameter("@propeties", inputGen.Propeties)
			};
			return ExecuteData("USP_APZ_GEN_FORECAST", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetDataBaseOnInputForecast()
	{
		try
		{
			return ExecuteDataTable("APZ_GenForecast_GetDataBaseOn", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
