using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

public class SqlQueriesRepository : QueriesRepository
{
	public SqlQueriesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
