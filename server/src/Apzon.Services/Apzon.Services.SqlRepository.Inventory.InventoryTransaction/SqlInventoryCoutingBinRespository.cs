using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.SqlRepository.Inventory.InventoryTransaction;

public class SqlInventoryCoutingBinRespository : InventoryCoutingBinRespository
{
	public SqlInventoryCoutingBinRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
