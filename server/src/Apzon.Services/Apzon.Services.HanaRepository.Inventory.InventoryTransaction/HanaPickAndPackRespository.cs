using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.HanaRepository.Inventory.InventoryTransaction;

public class HanaPickAndPackRespository : PickAndPackRespository
{
	public HanaPickAndPackRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
