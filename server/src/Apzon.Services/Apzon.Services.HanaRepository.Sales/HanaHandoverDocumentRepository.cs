using Apzon.Services.Repository.Sales;

namespace Apzon.Services.HanaRepository.Sales;

public class HanaHandoverDocumentRepository : HandoverDocumentRepository
{
	public HanaHandoverDocumentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
