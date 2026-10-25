using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

public class GeographyRepository : BaseService<DataSet>, IGeography
{
	public GeographyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetGeography(string countryCode, string location, string status)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@countryCode", Function.ToString(countryCode)));
			list.Add(new SqlParameter("@location", Function.ToString(location)));
			list.Add(new SqlParameter("@status", Function.ToString(status)));
			return ExecuteDataTable("APZ_D_Geography_GetData", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetGeographyDetails(string countryCode)
	{
		try
		{
			string query = "select b.* \r\n                                 from APZ_D_OGGP a \r\n\t                             inner join APZ_D_GGP1 b on a.\"CountryCode\" = b.\"CountryCode\"\r\n\t                             where a.\"CountryCode\" = @countryCode;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@countryCode", Function.ToString(countryCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetCountryWithGgpCodeQuery()
	{
		return "select ISNULL(CountryCode, '') from APZ_D_GGP1 where Code = @ggpCode";
	}

	public string GetCountryWithGgpCode(string ggpCode)
	{
		try
		{
			string countryWithGgpCodeQuery = GetCountryWithGgpCodeQuery();
			DataTable dataTable = ExecuteDataTable(countryWithGgpCodeQuery, CommandType.Text, new SqlParameter("@ggpCode", ggpCode));
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public DataTable GetRecord(string position, string countryCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@position", Function.ToString(position)));
			list.Add(new SqlParameter("@countryCode", Function.ToString(countryCode)));
			ExecuteDataTable("APZ_D_Geography_GetRecord", CommandType.StoredProcedure, list.ToArray());
			return ExecuteDataTable("APZ_D_Geography_GetRecord", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveData(DataSet ds, string type)
	{
		if (type == "U")
		{
			return Process(ApzObjectTypes.oGeography, -1, Enums.TransactionType.U, ds);
		}
		return Process(ApzObjectTypes.oGeography, -1, Enums.TransactionType.A, ds);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		try
		{
			CreateTempTable(document.Tables[0], "#APZ_D_OGGP");
			BulkCopy(document.Tables[0], "#APZ_D_OGGP");
			CreateTempTable(document.Tables[1], "#APZ_D_GGP1");
			BulkCopy(document.Tables[1], "#APZ_D_GGP1");
			DataTable dataTable = ExecuteDataTable("APZ_D_Process_Geography", CommandType.StoredProcedure, new SqlParameter("@type", (transtype == Enums.TransactionType.A) ? "A" : "U"));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return new HttpResult(Enums.MessageCode.Error, 1470000158 + " - " + Function.ToString(dataTable.Rows[0]["ErrorMessage"]));
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult CheckRemoveDetails(string code)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_D_Geography_CheckRemoveDetail", CommandType.StoredProcedure, new SqlParameter("@code", Function.ToString(code)));
			if (dataTable.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][0])))
			{
				return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable.Rows[0][0]) + " - " + Function.ToString(dataTable.Rows[0][1]));
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Remove(string countryCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_D_Geography_Remove", CommandType.StoredProcedure, new SqlParameter("@countryCode", Function.ToString(countryCode)));
			if (dataTable.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][0])))
			{
				return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable.Rows[0][0]) + " - " + Function.ToString(dataTable.Rows[0][1]));
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
