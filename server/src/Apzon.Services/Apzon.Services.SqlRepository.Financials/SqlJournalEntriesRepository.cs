using Apzon.Services.Repository.Financials;

namespace Apzon.Services.SqlRepository.Financials;

public class SqlJournalEntriesRepository : JournalEntriesRepository
{
	public SqlJournalEntriesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
