using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Mrps;

public class MrpsController : BaseApiController
{
	[DeflateCompression]
	[HttpGet]
	public DataTable GetById(int id)
	{
		try
		{
			return UnitOfWork.MRP.GetById(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public HttpResult Run([FromBody] DataTable dt)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data must not emptry!");
			}
			return UnitOfWork.MRP.Save(dt.Rows[0], Enums.TransactionType.A, "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpGet]
	public DataTable GetWarehouse(int branch)
	{
		try
		{
			return UnitOfWork.MRP.GetWarehouse(branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public string GetEndateForecast(string forecastCode)
	{
		return UnitOfWork.MRP.GetEndateForecast(forecastCode);
	}

	[HttpGet]
	public DataTable GetDataDoc(int id, Enums.NavigateType type)
	{
		return UnitOfWork.MRP.GetDataDoc(id, type);
	}

	[HttpPost]
	public HttpResult Delete([FromBody] int id)
	{
		return UnitOfWork.MRP.Save(null, Enums.TransactionType.D, Function.ToString(id));
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataTable dt)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data must not emptry!");
			}
			return UnitOfWork.MRP.Save(dt.Rows[0], Enums.TransactionType.U, "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpGet]
	public HttpResult ValidateScheduleForMrp(string lstPsCode)
	{
		return UnitOfWork.MRP.ValidateScheduleForMrp(lstPsCode);
	}

	[HttpGet]
	public HttpResult GetNewIdMrp()
	{
		return UnitOfWork.MRP.GetNewIdMrp();
	}

	[HttpGet]
	public DataTable GetSourceCellDataDocumentType()
	{
		return UnitOfWork.MRP.GetSourceCellDataDocumentType();
	}

	[HttpGet]
	public DataTable LoadDataSourceCell(string demandType, int demandEntry, int demandLine)
	{
		return UnitOfWork.MRP.LoadDataSourceCell(demandType, demandEntry, demandLine);
	}

	[HttpGet]
	public int GetRequestTimeout()
	{
		return Global.MrpRequestTimeout;
	}
}
