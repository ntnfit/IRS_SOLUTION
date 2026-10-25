using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.SqlRepository.Inventory.InventoryTransaction;

public class SqlStockRequestRepository : StockRequestRepository
{
	public SqlStockRequestRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
