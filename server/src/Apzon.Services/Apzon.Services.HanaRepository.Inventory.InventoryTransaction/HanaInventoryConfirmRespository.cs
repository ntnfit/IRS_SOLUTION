using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.HanaRepository.Inventory.InventoryTransaction;

public class HanaInventoryConfirmRespository : InventoryConfirmRespository
{
	public HanaInventoryConfirmRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
