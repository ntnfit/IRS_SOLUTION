using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.HanaRepository.HumanResources.TimeSheeting;

public class HanaHolidaySetupRepository : HolidaySetupRepository
{
	public HanaHolidaySetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
