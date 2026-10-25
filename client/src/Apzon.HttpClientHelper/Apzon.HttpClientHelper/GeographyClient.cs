using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class GeographyClient : IGeographyClient
{
	private string _controller = "Geography";

	public DataTable GetGeography(string countryCode, string location, string status)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return httpClientHelper.Get("GetGeography", new { countryCode, location, status });
	}

	public DataTable GetGeographyDetails(string countryCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return httpClientHelper.Get("GetGeographyDetails", new { countryCode });
	}

	public bool CheckExistsGeography(string countryCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return httpClientHelper.GetBool("CheckExistsGeography", new { countryCode });
	}

	public bool CheckExistsGeographyDetails(string countryCode, string ggpCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return httpClientHelper.GetBool("CheckExistsGeographyDetails", new { countryCode, ggpCode });
	}

	public string GetCountryWithGgpCode(string ggpCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return httpClientHelper.GetString("GetCountryWithGgpCode", new { ggpCode });
	}

	public DataTable GetRecord(string position, string countryCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return httpClientHelper.Get("GetRecord", new { position, countryCode });
	}

	public HttpResult SaveData(Tuple<DataSet, string> tupData)
	{
		using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, _controller);
		return httpClientHelper.Post("SaveData", tupData);
	}

	public HttpResult CheckRemoveDetails(string code)
	{
		using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, _controller);
		return httpClientHelper.Get("CheckRemoveDetails", new { code });
	}

	public HttpResult Remove(Tuple<string> tupParams)
	{
		using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, _controller);
		return httpClientHelper.Post("Remove", tupParams);
	}
}
