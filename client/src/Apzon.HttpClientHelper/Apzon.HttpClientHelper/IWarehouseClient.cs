using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;

namespace Apzon.HttpClientHelper;

public interface IWarehouseClient
{
	Task<DataTable> GetAllCode();

	Task<DataTable> GetRecord(string position, string whsCode);

	Task<string> DeleteWarehouse(string whsCode);

	Common.Table_Code WhsCheckFollowBin(string whsCode);

	Common.Table_Code WhsGetDefBinItem(string whsCode, string itemCode);

	Task<string> Save(WarehouseImpl doc);
}
