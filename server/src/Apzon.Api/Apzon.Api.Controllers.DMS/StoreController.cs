using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DMS;

public class StoreController : BaseApiController
{
	[HttpGet]
	public DataTable GetStore(string storeCode, string cardCode, string status)
	{
		return UnitOfWork.Store.GetStore(storeCode, cardCode, status);
	}

	[HttpGet]
	public DataTable GetEvents(int clgCode)
	{
		return UnitOfWork.Store.GetEvents(clgCode);
	}

	[HttpGet]
	public DataTable GetStoreEvents(string storeCode)
	{
		return UnitOfWork.Store.GetStoreEvents(storeCode);
	}

	[HttpGet]
	public DataTable GetGeography()
	{
		return UnitOfWork.Store.GetGeography();
	}

	[HttpGet]
	public DataTable GetRecord(string position, string storeCode)
	{
		return UnitOfWork.Store.GetRecord(position, storeCode);
	}

	[HttpGet]
	public DataTable GetLastEvent(string storeCode)
	{
		return UnitOfWork.Store.GetLastEvent(storeCode);
	}

	[HttpPost]
	public HttpResult Save(Tuple<DataSet, string> tuple)
	{
		return UnitOfWork.Store.Save(tuple);
	}

	[HttpGet]
	public HttpResult CheckRemove(string storeCode)
	{
		return UnitOfWork.Store.CheckRemove(storeCode);
	}

	[HttpPost]
	public HttpResult Remove(Tuple<string> tuple)
	{
		return UnitOfWork.Store.Remove(tuple);
	}

	[HttpGet]
	public DataTable GetParentsGeography(string ggpCode)
	{
		return UnitOfWork.Store.GetParentsGeography(ggpCode);
	}
}
