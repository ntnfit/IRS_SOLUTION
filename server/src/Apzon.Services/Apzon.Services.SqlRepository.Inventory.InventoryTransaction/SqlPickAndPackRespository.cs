using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.SqlRepository.Inventory.InventoryTransaction;

public class SqlPickAndPackRespository : PickAndPackRespository
{
	public SqlPickAndPackRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
