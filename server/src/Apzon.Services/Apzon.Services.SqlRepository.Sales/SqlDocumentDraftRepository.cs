using Apzon.Services.Repository.Sales;

namespace Apzon.Services.SqlRepository.Sales;

public class SqlDocumentDraftRepository : DocumentDraftRepository
{
	public SqlDocumentDraftRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
