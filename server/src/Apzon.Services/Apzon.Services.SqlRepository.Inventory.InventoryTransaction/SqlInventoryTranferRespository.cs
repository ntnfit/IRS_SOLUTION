using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.SqlRepository.Inventory.InventoryTransaction;

public class SqlInventoryTranferRespository : InventoryTranferRespository
{
	public SqlInventoryTranferRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
