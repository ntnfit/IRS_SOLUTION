using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.SqlRepository.Inventory.InventoryTransaction;

public class SqlInventoryRevaluationRespository : InventoryRevaluationRespository
{
	public SqlInventoryRevaluationRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
