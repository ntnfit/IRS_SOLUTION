using Apzon.Services.Repository.Production.PlanningProduction;

namespace Apzon.Services.HanaRepository.PlanningProduction;

public class HanaShiftRepository : ShiftRepository
{
	public HanaShiftRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
