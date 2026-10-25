using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.HanaRepository.Inventory.InventoryTransaction;

public class HanaCycleCountRecommendationRespository : CycleCountRecommendationRespository
{
	public HanaCycleCountRecommendationRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetListCycleCodeQuery()
	{
		return "select \"Code\",\"Name\" from OCYC order by \"Code\" ASC";
	}
}
