using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DMS;

public interface IGeography
{
	DataTable GetGeography(string countryCode, string location, string status);

	DataTable GetGeographyDetails(string countryCode);

	string GetCountryWithGgpCode(string ggpCode);

	DataTable GetRecord(string position, string countryCode);

	HttpResult CheckRemoveDetails(string code);

	HttpResult SaveData(DataSet ds, string type);

	HttpResult Remove(string countryCode);
}
