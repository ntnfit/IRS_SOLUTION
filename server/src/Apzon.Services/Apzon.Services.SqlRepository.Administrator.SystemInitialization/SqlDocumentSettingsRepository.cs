using Apzon.Services.Repository.Administrator.SystemInitialization;

namespace Apzon.Services.SqlRepository.Administrator.SystemInitialization;

public class SqlDocumentSettingsRepository : DocumentSettingsRepository
{
	public SqlDocumentSettingsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
