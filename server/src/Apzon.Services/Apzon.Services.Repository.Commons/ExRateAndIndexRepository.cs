using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class ExRateAndIndexRepository : BaseService<List<ExchangeRateAndIndexImpl>>, IExRateAndIndex
{
	public ExRateAndIndexRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string SaveDoc(List<ExchangeRateAndIndexImpl> docs)
	{
		try
		{
			return Process(ApzObjectTypes.oPriceLists, -1, Enums.TransactionType.U, docs).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, List<ExchangeRateAndIndexImpl> docs, ref string objId, ref string objNum)
	{
		HttpResult httpResult = new HttpResult();
		try
		{
			foreach (ExchangeRateAndIndexImpl doc in docs)
			{
				string text = SaveOneDoc(doc);
				if (!string.IsNullOrEmpty(text))
				{
					httpResult.Message = text;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			httpResult.Message = ex.Message;
		}
		return httpResult;
	}

	public string SaveOneDoc(ExchangeRateAndIndexImpl doc)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string empty = string.Empty;
			empty = " DELETE FROM ORTT WHERE \"RateDate\"=@RateDate and \"Currency\"=@Currency;\r\n                            INSERT INTO ORTT\r\n                                           (\"RateDate\"\r\n                                           ,\"Currency\"\r\n                                           ,\"Rate\"\r\n                                           ,\"DataSource\"\r\n                                           ,\"UserSign\")\r\n                                     VALUES\r\n                                           (";
			empty += " @RateDate ";
			list.Add(new SqlParameter("@RateDate", doc.RateDate));
			empty += " ,@Currency ";
			list.Add(new SqlParameter("@Currency", doc.Currency));
			empty += " ,@Rate ";
			list.Add(new SqlParameter("@Rate", doc.Rate));
			empty += " ,@DataSource ";
			list.Add(new SqlParameter("@DataSource", doc.DataSource));
			empty += " ,@UserSign ";
			list.Add(new SqlParameter("@UserSign", doc.UserSign));
			empty += " );";
			ExecuteNonQuery(empty, CommandType.Text, list.ToArray());
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetIndexByYear(int year)
	{
		try
		{
			string query = "SP_APZ_GETINDEXRATES";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@Year", year));
			if (dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetIndexByYear  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetRatesByMonthYear(string month, string year)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SP_APZ_GETEXCHANGERATES", CommandType.StoredProcedure, new SqlParameter("@Month", month), new SqlParameter("@Year", year));
			if (dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRatesByMonthYear  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetRateByCurrency(string cur, DateTime date)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetRateByCurrencyQuery(date), CommandType.Text, new SqlParameter("@cur", Function.ToString(cur).ToUpper()));
			if (dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRateByMonthYearCurrency  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetRateByCurrencyQuery(DateTime date)
	{
		return string.Concat("DECLARE @main NVARCHAR(30)\r\n                SET @main=(SELECT TOP 1 MainCurncy FROM dbo.OADM)\r\n                IF(@main=@cur)\r\n                SELECT 1\r\n                ELSE\r\n                select rate from ortt where ratedate='" + $"{date.Year}-{date.Month}-{date.Day}" + "'", " and currency=@cur");
	}
}
