using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

public class PickAndPackController : BaseApiController
{
	[HttpPost]
	public HttpResult Save([FromBody] DataSet ds)
	{
		if (string.IsNullOrEmpty(SessionId))
		{
			return new HttpResult
			{
				Message = "INVALIDSESSION",
				Content = null
			};
		}
		return BrokerService.GetUnitOfWork(SessionId).PickAndPack.Save(ds, SessionUserInfo.SessionDiServer);
	}

	[HttpGet]
	public DataSet GetBatchItem(string barcode)
	{
		try
		{
			return UnitOfWork.PickAndPack.GetBatchItem(barcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetBatchDocument(string barcode)
	{
		try
		{
			return UnitOfWork.PickAndPack.GetBatchDocument(barcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetLineItem(int docEntry)
	{
		try
		{
			return UnitOfWork.PickAndPack.GetLineItem(docEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetRecord(int docEntry, string type)
	{
		try
		{
			return UnitOfWork.PickAndPack.GetRecord(docEntry, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public int GetMaxDocEntry(string tableName)
	{
		return UnitOfWork.PickAndPack.GetMaxDocEntry(tableName);
	}
}
