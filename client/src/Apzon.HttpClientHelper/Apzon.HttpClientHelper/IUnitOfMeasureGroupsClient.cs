using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IUnitOfMeasureGroupsClient
{
	DataTable GetDataUomInGroup(int ugpEntry, int uomEntry);

	Task<DataTable> GetUoMGroupDefinition(int ugpEntry);

	bool CheckUoMGroupExists(int ugpEntry);

	Task<HttpResult> SaveSap(Tuple<DataTable, DataTable, Enums.HttpAction> tupData);

	Task<HttpResult> Remove(string ugpCode);

	DataTable GetUomItem(string itemCode);
}
