using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

public class SaleTerritoryRepository : BaseService<DataSet>, ISaleTerritory
{
	public SaleTerritoryRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetBranchStructure(string territoryCode)
	{
		try
		{
			string query = "select distinct a.Code, a.RegionCode, a.Name as RegionName, a.GgpCode, a.FCode as FatherCode, a.Status, a.Remarks, a.Id, a.FatherId\r\n                                 from APZ_D_LEGACY_STR1 a\r\n                                 where a.Code = @territoryCode";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@territoryCode", Function.ToString(territoryCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetStructureDetails(string territoryCode, string regionCode, string ggpCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@territoryCode", Function.ToString(territoryCode)));
			list.Add(new SqlParameter("@regionCode", Function.ToString(regionCode)));
			list.Add(new SqlParameter("@ggpCode", Function.ToString(ggpCode)));
			string query = "select a.Code, c.Name as GgpName, a.RegionCode, a.GgpCode, a.GgpCode1, a.Remarks from APZ_D_LEGACY_STR2 a\r\n                                left join APZ_D_GGP1 c on a.GgpCode = c.Code\r\n                                where a.Code = @territoryCode and a.RegionCode = @regionCode and a.GgpCode1 = @ggpCode";
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetSaleTerritory(string territoryCode, string location, string status)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@territoryCode", Function.ToString(territoryCode)));
			list.Add(new SqlParameter("@location", Function.ToString(location)));
			list.Add(new SqlParameter("@status", Function.ToString(status)));
			return ExecuteDataTable("APZ_SaleTerritory_GetData", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetStore(string territoryCode, string code)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@territoryCode", territoryCode));
			list.Add(new SqlParameter("@code", code));
			string query = "select b.Code, b.StrCode, a.Name as StrName, a.Region, a.City, a.District, a.Ward, a.Town, a.CardCode, a.Status, a.Address, b.Remarks \r\n                                from APZ_D_STORE a \r\n                                inner join APZ_D_LEGACY_STR3 b on a.Code = b.StrCode\r\n                                where b.Code = @territoryCode and (@code = '' or b.GgpCode is null or b.GgpCode in (select value from string_split(@code, ',')))";
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckExistsGeography(string code, int isCountry)
	{
		try
		{
			string query = "APZ_Geography_CheckExists";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@code", code));
			list.Add(new SqlParameter("@isCountry", isCountry));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveData(Tuple<DataSet, string> tuple)
	{
		if (tuple.Item2 == "A")
		{
			return Process(ApzObjectTypes.oSaleTerritory, -1, Enums.TransactionType.A, tuple.Item1);
		}
		return Process(ApzObjectTypes.oSaleTerritory, -1, Enums.TransactionType.U, tuple.Item1);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		try
		{
			CreateTempTable(document.Tables[0], "#APZ_D_OSTR");
			BulkCopy(document.Tables[0], "#APZ_D_OSTR");
			CreateTempTable(document.Tables[1], "#APZ_D_STR1");
			BulkCopy(document.Tables[1], "#APZ_D_STR1");
			CreateTempTable(document.Tables[2], "#APZ_D_STR2");
			BulkCopy(document.Tables[2], "#APZ_D_STR2");
			CreateTempTable(document.Tables[3], "#APZ_D_STR3");
			BulkCopy(document.Tables[3], "#APZ_D_STR3");
			if (transtype == Enums.TransactionType.A)
			{
				DataTable dataTable = ExecuteDataTable("APZ_Process_SaleTerritory", CommandType.StoredProcedure, new SqlParameter("@type", Function.ToString(transtype)));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable.Rows[0][1]));
				}
			}
			else
			{
				DataTable dataTable2 = ExecuteDataTable("APZ_Process_SaleTerritory", CommandType.StoredProcedure, new SqlParameter("@type", Function.ToString(transtype)));
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable2.Rows[0][1]));
				}
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetRecord(string position, string territoryCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@position", position));
			list.Add(new SqlParameter("@territoryCode", territoryCode));
			return ExecuteDataTable("APZ_SaleTerritory_GetRecord", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Remove(Tuple<string> tuple)
	{
		try
		{
			if (ExecuteNonQuery("APZ_SaleTerritory_Remove", CommandType.StoredProcedure, new SqlParameter("@territoryCode", Function.ToString(tuple.Item1))) == -1)
			{
				return new HttpResult(Enums.MessageCode.Error, "Delete Failed");
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
