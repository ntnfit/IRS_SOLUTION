using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DMS;

public class SalesRouteController : BaseApiController
{
	[HttpGet]
	public DataTable GetDataDoc(string routeCode, string routeName, string status)
	{
		return UnitOfWork.SalesRoute.GetDataDoc(routeCode, routeName, status);
	}

	[HttpGet]
	public DataTable GetDataDetails(string routeCode)
	{
		return UnitOfWork.SalesRoute.GetDataDetails(routeCode);
	}

	[HttpGet]
	public DataTable GetDataOutlets(int id, string routeCode, int empId)
	{
		return UnitOfWork.SalesRoute.GetDataOutlets(id, routeCode, empId);
	}

	[HttpGet]
	public DataTable GetListDistributors(int id, string routeCode, int empId)
	{
		return UnitOfWork.SalesRoute.GetListDistributors(id, routeCode, empId);
	}

	[HttpGet]
	public DataTable GetBpMasterData(string type, string cardCode)
	{
		return UnitOfWork.SalesRoute.GetBpMasterData(type, cardCode);
	}

	[HttpGet]
	public DataTable GetRecord(string position, string routeCode)
	{
		return UnitOfWork.SalesRoute.GetRecord(position, routeCode);
	}

	[HttpPost]
	public HttpResult SaveData(Tuple<DataSet, string> tupData)
	{
		return UnitOfWork.SalesRoute.SaveData(tupData.Item1, tupData.Item2);
	}

	[HttpPost]
	public HttpResult Remove(Tuple<string> tupData)
	{
		return UnitOfWork.SalesRoute.Remove(tupData.Item1);
	}

	[HttpGet]
	public DataTable GetEmployees(string routeCode)
	{
		return UnitOfWork.SalesRoute.GetEmployees(routeCode);
	}

	[HttpGet]
	public DataTable GetOutlets(string routeCode)
	{
		return UnitOfWork.SalesRoute.GetOutlets(routeCode);
	}

	[HttpGet]
	public DataTable GetDistributors(string routeCode)
	{
		return UnitOfWork.SalesRoute.GetDistributors(routeCode);
	}
}
