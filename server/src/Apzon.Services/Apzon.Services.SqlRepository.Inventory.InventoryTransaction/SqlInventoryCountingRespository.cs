using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.SqlRepository.Inventory.InventoryTransaction;

public class SqlInventoryCountingRespository : InventoryCountingRespository
{
	public SqlInventoryCountingRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
