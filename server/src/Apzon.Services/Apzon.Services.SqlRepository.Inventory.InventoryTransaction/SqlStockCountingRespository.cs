using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.SqlRepository.Inventory.InventoryTransaction;

public class SqlStockCountingRespository : StockCountingRespository
{
	public SqlStockCountingRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
