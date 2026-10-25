using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.HanaRepository.Inventory.InventoryTransaction;

public class HanaStockCountingRespository : StockCountingRespository
{
	public HanaStockCountingRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
