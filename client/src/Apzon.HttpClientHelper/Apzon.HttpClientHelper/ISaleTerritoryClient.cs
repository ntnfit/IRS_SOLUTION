using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ISaleTerritoryClient
{
	Task<DataTable> GetBranchStructure(string territoryCode);

	Task<DataTable> GetStructureDetails(string territoryCode, string regionCode, string ggpCode);

	Task<DataTable> GetSaleTerritory(string territoryCode, string location, string status);

	Task<DataTable> GetStore(string territoryCode, string code);

	DataTable CheckExistsGeography(string code, int isCountry);

	Task<DataTable> GetRecord(string position, string territory);

	Task<HttpResult> SaveData(Tuple<DataSet, string> tuple);

	Task<HttpResult> Remove(Tuple<string> territoryCode);
}
