using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DMS;

public interface ISaleTerritory
{
	DataTable GetBranchStructure(string territoryCode);

	DataTable GetStructureDetails(string territoryCode, string regionCode, string ggpCode);

	DataTable GetSaleTerritory(string territoryCode, string location, string status);

	DataTable GetStore(string territoryCode, string code);

	DataTable GetRecord(string position, string territoryCode);

	DataTable CheckExistsGeography(string code, int isCountry);

	HttpResult SaveData(Tuple<DataSet, string> tuple);

	HttpResult Remove(Tuple<string> tuple);
}
