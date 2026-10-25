using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.SqlRepository.Opportunity;

public class SqlActivityStatusRepository : ActivityStatusRepository
{
	public SqlActivityStatusRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
