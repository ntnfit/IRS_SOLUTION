using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.AdditionalGeneralSettings;

internal class SqlAdditionalGeneralSettingsRepository : AdditionalGeneralSettingsRepository
{
	public SqlAdditionalGeneralSettingsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
