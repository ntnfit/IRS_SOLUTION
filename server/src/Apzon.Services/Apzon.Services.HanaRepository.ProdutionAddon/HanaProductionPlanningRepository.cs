using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.HanaRepository.ProdutionAddon;

public class HanaProductionPlanningRepository : ProductionPlanningRepository
{
	public HanaProductionPlanningRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
