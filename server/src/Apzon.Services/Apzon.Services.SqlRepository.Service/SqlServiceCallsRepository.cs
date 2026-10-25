using Apzon.Services.Repository.Service;

namespace Apzon.Services.SqlRepository.Service;

public class SqlServiceCallsRepository : ServiceCallsRepository
{
	public SqlServiceCallsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
