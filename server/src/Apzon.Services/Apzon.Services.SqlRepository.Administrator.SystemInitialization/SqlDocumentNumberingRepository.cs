using Apzon.Services.Repository.Administrator.SystemInitialization;

namespace Apzon.Services.SqlRepository.Administrator.SystemInitialization;

public class SqlDocumentNumberingRepository : DocumentNumberingRepository
{
	public SqlDocumentNumberingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
