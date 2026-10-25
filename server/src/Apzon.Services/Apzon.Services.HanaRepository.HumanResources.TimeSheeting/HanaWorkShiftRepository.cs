using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.HanaRepository.HumanResources.TimeSheeting;

public class HanaWorkShiftRepository : WorkShiftRepository
{
	public HanaWorkShiftRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
