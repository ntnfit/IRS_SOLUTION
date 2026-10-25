using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.HanaRepository.Inventory.InventoryTransaction;

public class HanaInventoryTranferRespository : InventoryTranferRespository
{
	public HanaInventoryTranferRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
