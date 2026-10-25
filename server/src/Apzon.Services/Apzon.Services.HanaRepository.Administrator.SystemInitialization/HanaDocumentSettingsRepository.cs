using Apzon.Services.Repository.Administrator.SystemInitialization;

namespace Apzon.Services.HanaRepository.Administrator.SystemInitialization;

public class HanaDocumentSettingsRepository : DocumentSettingsRepository
{
	public HanaDocumentSettingsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
