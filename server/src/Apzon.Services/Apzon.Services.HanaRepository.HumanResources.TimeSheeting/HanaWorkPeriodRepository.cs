using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.HanaRepository.HumanResources.TimeSheeting;

public class HanaWorkPeriodRepository : WorkPeriodRepository
{
	public HanaWorkPeriodRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
