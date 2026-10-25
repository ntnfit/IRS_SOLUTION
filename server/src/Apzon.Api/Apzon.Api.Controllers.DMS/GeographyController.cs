using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DMS;

public class GeographyController : BaseApiController
{
	[HttpGet]
	public DataTable GetGeography(string countryCode, string location, string status)
	{
		return UnitOfWork.Geography.GetGeography(countryCode, location, status);
	}

	[HttpGet]
	public DataTable GetGeographyDetails(string countryCode)
	{
		return UnitOfWork.Geography.GetGeographyDetails(countryCode);
	}

	[HttpGet]
	public string GetCountryWithGgpCode(string ggpCode)
	{
		return UnitOfWork.Geography.GetCountryWithGgpCode(ggpCode);
	}

	[HttpGet]
	public DataTable GetRecord(string position, string countryCode)
	{
		return UnitOfWork.Geography.GetRecord(position, countryCode);
	}

	[HttpPost]
	public HttpResult SaveData(Tuple<DataSet, string> tupData)
	{
		return UnitOfWork.Geography.SaveData(tupData.Item1, tupData.Item2);
	}

	[HttpGet]
	public HttpResult CheckRemoveDetails(string code)
	{
		return UnitOfWork.Geography.CheckRemoveDetails(code);
	}

	[HttpPost]
	public HttpResult Remove(Tuple<string> tupParams)
	{
		return UnitOfWork.Geography.Remove(tupParams.Item1);
	}
}
