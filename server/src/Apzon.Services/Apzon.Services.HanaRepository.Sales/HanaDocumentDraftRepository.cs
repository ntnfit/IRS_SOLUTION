using Apzon.Services.Repository.Sales;

namespace Apzon.Services.HanaRepository.Sales;

public class HanaDocumentDraftRepository : DocumentDraftRepository
{
	public HanaDocumentDraftRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
