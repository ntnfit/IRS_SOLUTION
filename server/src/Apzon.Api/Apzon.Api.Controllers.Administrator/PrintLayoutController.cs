using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Administrator;

namespace Apzon.Api.Controllers.Administrator;

public class PrintLayoutController : BaseApiController
{
	[HttpGet]
	public DataTable GetAll()
	{
		try
		{
			return UnitOfWork.PrintLayout.GetAll();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpPost]
	public string SaveData([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.PrintLayout.SaveData(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public PrintLayout GetDefaultPrintLayout(string userName, string formLayoutCode, string cardCode)
	{
		try
		{
			return UnitOfWork.PrintLayout.GetDefaultPrintLayout(userName, formLayoutCode, cardCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new PrintLayout();
	}

	[HttpGet]
	public List<PrintLayout> GetLayouts(string userName, string formLayoutCode, string cardCode)
	{
		try
		{
			return UnitOfWork.PrintLayout.GetLayouts(userName, formLayoutCode, cardCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new List<PrintLayout>();
	}

	[HttpGet]
	public string UpdatePrinted(string docentry, string table, string type, string userName = "")
	{
		try
		{
			return UnitOfWork.PrintLayout.UpdatePrinted(docentry, table, type, userName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
