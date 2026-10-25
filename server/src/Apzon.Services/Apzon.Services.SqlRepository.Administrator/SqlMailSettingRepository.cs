using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

internal class SqlMailSettingRepository(IDatabaseService databaseService) : MailSettingRepository(databaseService)
{
	private string Temp_OACS = "#APZ_OEMC";
}
