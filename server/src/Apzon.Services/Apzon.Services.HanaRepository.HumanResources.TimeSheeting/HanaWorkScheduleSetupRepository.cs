using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.HanaRepository.HumanResources.TimeSheeting;

public class HanaWorkScheduleSetupRepository : WorkScheduleSetupRepository
{
	public HanaWorkScheduleSetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
