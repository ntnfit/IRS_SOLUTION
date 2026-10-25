using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Mrps;

public class ItemQuotaArrangementDateController : BaseApiController
{
	[HttpPost]
	public string DateSave([FromBody] DataSet ds)
	{
		try
		{
			return UnitOfWork.ItemQuotaArrangementDate.DateSave(ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public string DeleteRows([FromBody] int periodID)
	{
		try
		{
			return UnitOfWork.ItemQuotaArrangementDate.DeleteRows(periodID);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public DataTable DateGetDocs(int periodID)
	{
		try
		{
			return UnitOfWork.ItemQuotaArrangementDate.DateGetDocs(periodID);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetCardName(string CardCode)
	{
		try
		{
			return UnitOfWork.ItemQuotaArrangementDate.GetCardName(CardCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
