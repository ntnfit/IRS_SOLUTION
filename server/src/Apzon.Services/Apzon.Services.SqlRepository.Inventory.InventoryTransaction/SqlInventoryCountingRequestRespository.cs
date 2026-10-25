using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.SqlRepository.Inventory.InventoryTransaction;

public class SqlInventoryCountingRequestRespository : InventoryCountingRequestRespository
{
	public SqlInventoryCountingRequestRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
