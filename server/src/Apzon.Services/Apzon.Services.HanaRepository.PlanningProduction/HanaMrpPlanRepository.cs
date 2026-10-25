using Apzon.Services.Repository.PlanningProduction;

namespace Apzon.Services.HanaRepository.PlanningProduction;

public class HanaMrpPlanRepository : MrpPlanRepository
{
	public HanaMrpPlanRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
