using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

public class CycleCountRecommendationController : BaseApiController
{
	[HttpGet]
	public DataTable GetListCycleCode()
	{
		try
		{
			return UnitOfWork.CycleCountRecommendation.GetListCycleCode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetListItemCode()
	{
		try
		{
			return UnitOfWork.CycleCountRecommendation.GetListItemCode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetWarehouse()
	{
		try
		{
			return UnitOfWork.CycleCountRecommendation.GetWarehouse();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetListDataLine(string whs, int fromCycleCode, int toCycleCode, string fromItemCode, string toItemCode)
	{
		try
		{
			return UnitOfWork.CycleCountRecommendation.GetListDataLine(whs, fromCycleCode, toCycleCode, fromItemCode, toItemCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
