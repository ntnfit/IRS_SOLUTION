using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.SqlRepository.Inventory.InventoryTransaction;

public class SqlInventoryConfirmRespository : InventoryConfirmRespository
{
	public SqlInventoryConfirmRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
