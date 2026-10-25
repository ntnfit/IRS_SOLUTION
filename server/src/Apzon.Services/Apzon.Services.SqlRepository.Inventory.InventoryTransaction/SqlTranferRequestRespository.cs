using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.SqlRepository.Inventory.InventoryTransaction;

public class SqlTranferRequestRespository : TranferRequestRespository
{
	public SqlTranferRequestRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
