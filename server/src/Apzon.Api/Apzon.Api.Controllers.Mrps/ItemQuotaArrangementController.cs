using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Mrps;

public class ItemQuotaArrangementController : BaseApiController
{
	[HttpGet]
	public DataTable GetLines(string ItemCode)
	{
		try
		{
			return UnitOfWork.ItemQuotaArrangement.GetLines(ItemCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetDocs(string str)
	{
		try
		{
			return UnitOfWork.ItemQuotaArrangement.GetDocs(str);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetADoc(string itemCode)
	{
		try
		{
			return UnitOfWork.ItemQuotaArrangement.GetADoc(itemCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetItemName(string ItemCode)
	{
		try
		{
			return UnitOfWork.ItemQuotaArrangement.GetItemName(ItemCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable CheckExistItem(string ItemCode)
	{
		try
		{
			return UnitOfWork.ItemQuotaArrangement.CheckExistItem(ItemCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable CheckExistPeriod(int periodID)
	{
		try
		{
			return UnitOfWork.ItemQuotaArrangement.CheckExistPeriod(periodID);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		try
		{
			return UnitOfWork.ItemQuotaArrangement.SaveDoc(ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public string DeleteDoc([FromBody] string ItemCode)
	{
		try
		{
			return UnitOfWork.ItemQuotaArrangement.DeleteDoc(ItemCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
