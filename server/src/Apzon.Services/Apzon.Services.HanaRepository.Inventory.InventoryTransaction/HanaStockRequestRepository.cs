using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.HanaRepository.Inventory.InventoryTransaction;

public class HanaStockRequestRepository : StockRequestRepository
{
	public HanaStockRequestRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
