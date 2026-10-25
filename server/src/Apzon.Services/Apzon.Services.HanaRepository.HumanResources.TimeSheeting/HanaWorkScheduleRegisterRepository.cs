using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.HanaRepository.HumanResources.TimeSheeting;

public class HanaWorkScheduleRegisterRepository : WorkScheduleRegisterRepository
{
	public HanaWorkScheduleRegisterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
