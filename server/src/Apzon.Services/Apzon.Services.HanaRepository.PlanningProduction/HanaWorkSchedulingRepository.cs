using Apzon.Services.Repository.Production.PlanningProduction;

namespace Apzon.Services.HanaRepository.PlanningProduction;

public class HanaWorkSchedulingRepository : WorkSchedulingRepository
{
	public HanaWorkSchedulingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
