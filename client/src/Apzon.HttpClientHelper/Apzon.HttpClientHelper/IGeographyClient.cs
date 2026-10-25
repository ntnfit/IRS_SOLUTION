using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IGeographyClient
{
	DataTable GetGeography(string countryCode, string location, string status);

	DataTable GetGeographyDetails(string countryCode);

	string GetCountryWithGgpCode(string ggpCode);

	bool CheckExistsGeography(string countryCode);

	bool CheckExistsGeographyDetails(string countryCode, string ggpCode);

	DataTable GetRecord(string position, string countryCode);

	HttpResult SaveData(Tuple<DataSet, string> tupData);

	HttpResult CheckRemoveDetails(string code);

	HttpResult Remove(Tuple<string> tupParams);
}
