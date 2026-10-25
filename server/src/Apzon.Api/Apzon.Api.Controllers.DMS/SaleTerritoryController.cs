using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DMS;

public class SaleTerritoryController : BaseApiController
{
	[HttpGet]
	public DataTable GetBranchStructure(string territoryCode)
	{
		return UnitOfWork.SaleTerritory.GetBranchStructure(territoryCode);
	}

	[HttpGet]
	public DataTable GetStructureDetails(string territoryCode, string regionCode, string ggpCode)
	{
		return UnitOfWork.SaleTerritory.GetStructureDetails(territoryCode, regionCode, ggpCode);
	}

	[HttpGet]
	public DataTable GetSaleTerritory(string territoryCode, string location, string status)
	{
		return UnitOfWork.SaleTerritory.GetSaleTerritory(territoryCode, location, status);
	}

	[HttpGet]
	public DataTable GetStore(string territoryCode, string code)
	{
		return UnitOfWork.SaleTerritory.GetStore(territoryCode, code);
	}

	[HttpGet]
	public DataTable GetRecord(string position, string territoryCode)
	{
		return UnitOfWork.SaleTerritory.GetRecord(position, territoryCode);
	}

	[HttpGet]
	public DataTable CheckExistsGeography(string code, int isCountry)
	{
		return UnitOfWork.SaleTerritory.CheckExistsGeography(code, isCountry);
	}

	[HttpPost]
	public HttpResult SaveData(Tuple<DataSet, string> tuple)
	{
		return UnitOfWork.SaleTerritory.SaveData(tuple);
	}

	[HttpPost]
	public HttpResult Remove(Tuple<string> tuple)
	{
		return UnitOfWork.SaleTerritory.Remove(tuple);
	}
}
