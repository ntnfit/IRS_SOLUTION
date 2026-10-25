using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

internal class SqlCalendarSettingRepository(IDatabaseService databaseService) : CalendarSettingRepository(databaseService)
{
	private string Temp_OACS = "#APZ_OACS";

	private string Temp_ACS1 = "#APZ_ACS1";
}
