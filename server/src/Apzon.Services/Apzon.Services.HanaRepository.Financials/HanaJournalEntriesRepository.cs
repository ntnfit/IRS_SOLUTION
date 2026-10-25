using Apzon.Services.Repository.Financials;

namespace Apzon.Services.HanaRepository.Financials;

public class HanaJournalEntriesRepository : JournalEntriesRepository
{
	public HanaJournalEntriesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
