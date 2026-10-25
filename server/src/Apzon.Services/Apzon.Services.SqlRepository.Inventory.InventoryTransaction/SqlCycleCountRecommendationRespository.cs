using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.SqlRepository.Inventory.InventoryTransaction;

public class SqlCycleCountRecommendationRespository : CycleCountRecommendationRespository
{
	public SqlCycleCountRecommendationRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
