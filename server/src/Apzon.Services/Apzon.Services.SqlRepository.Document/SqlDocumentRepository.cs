using Apzon.Services.Repository.Document;

namespace Apzon.Services.SqlRepository.Document;

public class SqlDocumentRepository : DocumentRepository
{
	public SqlDocumentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
