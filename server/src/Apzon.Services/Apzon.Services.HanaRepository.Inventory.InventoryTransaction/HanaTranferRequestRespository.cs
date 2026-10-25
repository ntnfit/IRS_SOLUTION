using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.HanaRepository.Inventory.InventoryTransaction;

public class HanaTranferRequestRespository : TranferRequestRespository
{
	public HanaTranferRequestRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
