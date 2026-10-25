using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaQueriesRepository : QueriesRepository
{
	public HanaQueriesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
