using Apzon.Services.Repository.PlanningProduction;

namespace Apzon.Services.HanaRepository.PlanningProduction;

public class HanaMpsRepository : MpsRepository
{
	public HanaMpsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
