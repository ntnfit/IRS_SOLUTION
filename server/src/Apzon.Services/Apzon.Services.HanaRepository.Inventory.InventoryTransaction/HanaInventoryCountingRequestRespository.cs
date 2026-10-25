using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.HanaRepository.Inventory.InventoryTransaction;

public class HanaInventoryCountingRequestRespository : InventoryCountingRequestRespository
{
	public HanaInventoryCountingRequestRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string DeleteDataQuery()
	{
		return "delete from APZ_OCRQ where \"DocEntry\" =ids";
	}
}
