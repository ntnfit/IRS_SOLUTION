using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaActivityCalendarRepository : ActivityCalendarRepository
{
	public HanaActivityCalendarRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
