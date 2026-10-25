using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Commons;

public class ExRateAndIndexController : BaseApiController
{
	[HttpGet]
	public DataTable GetRatesByMonthYear(string month, string year)
	{
		return UnitOfWork.ExRateAndIndex.GetRatesByMonthYear(month, year);
	}

	[HttpPost]
	public DataTable GetExchangeRate([FromBody] DataTable dt)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add(new DataColumn("Rate", typeof(double)));
		dataTable.AcceptChanges();
		dataTable.Rows.Add(dataTable.NewRow());
		dataTable.Rows[0][0] = 0;
		dataTable.AcceptChanges();
		try
		{
			DateTime? dateTime = Function.ParseDateTime(dt.Rows[0]["PostDate"]);
			if (dateTime.HasValue)
			{
				return UnitOfWork.ExRateAndIndex.GetRateByCurrency(Function.ToString(dt.Rows[0]["Curr"]), dateTime.Value);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return dataTable;
	}

	[HttpGet]
	public DataTable GetIndexByYear(int year)
	{
		return UnitOfWork.ExRateAndIndex.GetIndexByYear(year);
	}

	[HttpPost]
	public string SaveDoc([FromBody] List<ExchangeRateAndIndexImpl> doc)
	{
		return UnitOfWork.ExRateAndIndex.SaveDoc(doc);
	}
}
