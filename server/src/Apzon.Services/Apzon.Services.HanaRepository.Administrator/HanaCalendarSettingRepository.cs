using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaCalendarSettingRepository : CalendarSettingRepository
{
	public HanaCalendarSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
