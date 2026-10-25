using Apzon.Services.Repository.Sales;

namespace Apzon.Services.SqlRepository.Sales;

public class SqlHandoverDocumentRepository : HandoverDocumentRepository
{
	public SqlHandoverDocumentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
