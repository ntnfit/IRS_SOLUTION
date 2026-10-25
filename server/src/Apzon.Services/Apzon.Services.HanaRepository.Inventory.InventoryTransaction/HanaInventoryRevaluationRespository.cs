using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.HanaRepository.Inventory.InventoryTransaction;

public class HanaInventoryRevaluationRespository : InventoryRevaluationRespository
{
	public HanaInventoryRevaluationRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
