using Apzon.Services.Repository.Service;

namespace Apzon.Services.SqlRepository.Service;

public class SqlSubActivitiesRepository : SubActivitiesRepository
{
	public SqlSubActivitiesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
