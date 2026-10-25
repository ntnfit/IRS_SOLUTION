using Apzon.Services.Repository.Financials;

namespace Apzon.Services.HanaRepository.Financials;

public class HanaDocumentConfirmRepository : DocumentConfirmRepository
{
	public HanaDocumentConfirmRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
