using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.HanaRepository.HumanResources.TimeSheeting;

public class HanaAboutRangeTimeRepository : AboutRangeTimeRepository
{
	public HanaAboutRangeTimeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
