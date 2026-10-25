using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.HanaRepository.Inventory.InventoryTransaction;

public class HanaInventoryCoutingBinRespository : InventoryCoutingBinRespository
{
	public HanaInventoryCoutingBinRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
