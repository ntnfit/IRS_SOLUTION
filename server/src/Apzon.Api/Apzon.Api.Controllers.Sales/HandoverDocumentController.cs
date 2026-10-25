using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Commons;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Sales;

public class HandoverDocumentController : BaseApiController
{
	[HttpGet]
	public DataSet LoadDocuments(int id, string barcode)
	{
		try
		{
			return UnitOfWork.HandoverDocument.LoadDocuments(id, barcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataTable GetDoc(int id)
	{
		try
		{
			return UnitOfWork.HandoverDocument.GetById(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public int GetPage(int currentId, int type, string username, int branch)
	{
		try
		{
			return UnitOfWork.HandoverDocument.GetPage(currentId, type, username, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	[HttpGet]
	public DataSet GetDocumentDetail(string empCode, string barcode, string userName, int branch)
	{
		try
		{
			return UnitOfWork.HandoverDocument.GetDocumentDetail(empCode, barcode, userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public DataSet SearchDocuments([FromBody] DocumentsImpl doc)
	{
		try
		{
			return UnitOfWork.HandoverDocument.SearchDocuments(doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}
}
