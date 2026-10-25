using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Mrps;

public class ForecastController : BaseApiController
{
	[HttpGet]
	public DataTable GetData(string code, string name, DateTime? fromdate, DateTime? todate)
	{
		try
		{
			return UnitOfWork.Forecast.GetData(code, name, fromdate, todate);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetMaxAbsId()
	{
		try
		{
			return UnitOfWork.Forecast.GetMaxAbsId();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetMaxLineIdByAbsId([FromBody] int abs)
	{
		try
		{
			return UnitOfWork.Forecast.GetMaxLineIdByAbsId(abs);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetDataRow(int absId)
	{
		try
		{
			return UnitOfWork.Forecast.GetDataRow(absId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetForeCastFromOpportunity(int id)
	{
		try
		{
			return UnitOfWork.Forecast.GetForeCastFromOpportunity(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpPost]
	public HttpResult SaveData([FromBody] DataSet dsDataSet)
	{
		try
		{
			return UnitOfWork.Forecast.SaveData(dsDataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public string DeleteData([FromBody] int abs)
	{
		try
		{
			return UnitOfWork.Forecast.DeleteData(abs);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public string DeleteDataLine([FromBody] Tuple<int, string, DateTime> tup)
	{
		try
		{
			return UnitOfWork.Forecast.DeleteDataLine(tup);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
