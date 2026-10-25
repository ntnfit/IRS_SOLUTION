using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaAdditionalGeneralSettingsRepository : AdditionalGeneralSettingsRepository
{
	public HanaAdditionalGeneralSettingsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
