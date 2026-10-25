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
using Apzon.Services.Interfaces.Service.MarketingCampaign;

namespace Apzon.Services.Repository.Service.MarketingCampaign;

public class MarketingCampaignRepository : BaseService<DataSet>, IMarketingCampaign
{
	private const string docTemp = "#APZ_OSMC";

	private const string bpTemp = "#APZ_SMC1";

	private const string itemTemp = "#APZ_SMC2";

	private const string partnerTemp = "#APZ_SMC3";

	private const string memberTemp = "#APZ_SMC4";

	private const string costTemp = "#APZ_SMC5";

	private const string Linetemp4 = "#ATC1";

	public MarketingCampaignRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable LoadData(int id, string campaignNo, string startFrom, string startTo, string status)
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(startFrom, "yyyyMMdd");
			DateTime? dateTime2 = Function.ParseDateTime(startTo, "yyyyMMdd");
			string query = "APZ_MarketingCampaign_LoadData";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@id", id), new SqlParameter("@campaignNo", campaignNo), new SqlParameter("@startFrom", dateTime), new SqlParameter("@startTo", dateTime2), new SqlParameter("@status", status));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet GetDataById(int Id)
	{
		try
		{
			DataSet dataSet = ExecuteData("APZ_MarketingCampaign_GetById", CommandType.StoredProcedure, new SqlParameter("@Id", Id));
			if (dataSet != null && dataSet.Tables.Count == 7)
			{
				SetDefaultValueToTable("APZ_SMC1", dataSet.Tables[0]);
				SetDefaultValueToTable("APZ_SMC2", dataSet.Tables[1]);
				SetDefaultValueToTable("APZ_SMC3", dataSet.Tables[2]);
				SetDefaultValueToTable("APZ_SMC4", dataSet.Tables[3]);
				SetDefaultValueToTable("APZ_SMC5", dataSet.Tables[4]);
				SetDefaultValueToTable("APZ_OSMC", dataSet.Tables[5]);
			}
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetDataTargetBPById(int id)
	{
		try
		{
			string query = "Select * From APZ_SMC1 Where \"DocEntry\"=@Id;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@Id", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public HttpResult Save(DataSet ds, string action)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
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
			return Process(ApzObjectTypes.oMarketingCampaign, -1, transtype, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
			return DeleteData(ref ret, ref objId, ref objNum);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			if (ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Campaign code must not null!");
			}
			if (ds.Tables[1] == null || ds.Tables[1].Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Tab Target Bp must not null!");
			}
			DataTable tableStruct = GetTableStruct("APZ_OSMC");
			DataTable dataTable = ds.Tables[0];
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, "#APZ_OSMC");
			BulkCopy(dataTable, "#APZ_OSMC");
			DataTable tableStruct2 = GetTableStruct("APZ_SMC1");
			DataTable dataTable2 = ds.Tables[1];
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			CreateTempTable(dataTable2, "#APZ_SMC1");
			BulkCopy(dataTable2, "#APZ_SMC1");
			DataTable tableStruct3 = GetTableStruct("APZ_SMC2");
			DataTable dataTable3 = ds.Tables[2];
			if (!tableStruct3.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source3 = from DataColumn t in tableStruct3.Columns
				select t.ColumnName;
			dataTable3 = dataTable3.DefaultView.ToTable(distinct: false, source3.ToArray());
			CreateTempTable(dataTable3, "#APZ_SMC2");
			BulkCopy(dataTable3, "#APZ_SMC2");
			DataTable tableStruct4 = GetTableStruct("APZ_SMC3");
			DataTable dataTable4 = ds.Tables[3];
			if (!tableStruct3.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source4 = from DataColumn t in tableStruct4.Columns
				select t.ColumnName;
			dataTable4 = dataTable4.DefaultView.ToTable(distinct: false, source4.ToArray());
			CreateTempTable(dataTable4, "#APZ_SMC3");
			BulkCopy(dataTable4, "#APZ_SMC3");
			DataTable tableStruct5 = GetTableStruct("APZ_SMC4");
			DataTable dataTable5 = ds.Tables[4];
			if (!tableStruct5.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source5 = from DataColumn t in tableStruct5.Columns
				select t.ColumnName;
			dataTable5 = dataTable5.DefaultView.ToTable(distinct: false, source5.ToArray());
			CreateTempTable(dataTable5, "#APZ_SMC4");
			BulkCopy(dataTable5, "#APZ_SMC4");
			DataTable tableStruct6 = GetTableStruct("APZ_SMC5");
			DataTable dataTable6 = ds.Tables[5];
			if (!tableStruct6.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source6 = from DataColumn t in tableStruct6.Columns
				select t.ColumnName;
			dataTable6 = dataTable6.DefaultView.ToTable(distinct: false, source6.ToArray());
			CreateTempTable(dataTable6, "#APZ_SMC5");
			BulkCopy(dataTable6, "#APZ_SMC5");
			DataTable dataTable7 = ds.Tables[6];
			DataTable tableStruct7 = GetTableStruct("ATC1");
			if (!tableStruct7.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source7 = from DataColumn t in tableStruct7.Columns
				select t.ColumnName;
			dataTable7 = dataTable7.DefaultView.ToTable(distinct: false, source7.ToArray());
			CreateTempTable(dataTable7, "#ATC1");
			BulkCopy(dataTable7, "#ATC1");
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable8 = ExecuteDataTable("APZ_MarketingCampaign_Save", CommandType.StoredProcedure, sqlParameter);
			if (dataTable8 != null && dataTable8.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable8.Rows[0][1])))
				{
					objId = Function.ToString(dataTable8.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable8.Rows[0][0])}-{Function.ToString(dataTable8.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string DeleteDocs(string Id)
	{
		try
		{
			return Process(ApzObjectTypes.oMarketingCampaign, -1, Enums.TransactionType.D, new DataSet(), Id).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@Id", Function.ParseInt(objId));
			DataTable dataTable = ExecuteDataTable("APZ_MarketingCampaign_Remove", CommandType.StoredProcedure, sqlParameter);
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
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public int GetMaxId()
	{
		try
		{
			string maxIdQuery = GetMaxIdQuery();
			DataTable dataTable = ExecuteDataTable(maxIdQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	protected virtual string GetMaxIdQuery()
	{
		return "Select Max(\"DocEntry\") from APZ_OSMC;";
	}

	public DataTable FindNextLastFrist(int id, int type)
	{
		try
		{
			return ExecuteDataTable("APZ_MarketingCampaign_FindNextLastFrist", CommandType.StoredProcedure, new SqlParameter("@id", id), new SqlParameter("@type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable LoadTargetGroup()
	{
		try
		{
			string query = " select * from OTGG;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadTargetGroupDetails(string code)
	{
		try
		{
			return ExecuteDataTable("Select * From TGG1 Where \"TargetCode\"=@Code;", CommandType.Text, new SqlParameter("@Code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
