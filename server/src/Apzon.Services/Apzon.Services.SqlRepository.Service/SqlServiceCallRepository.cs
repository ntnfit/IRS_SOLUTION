using Apzon.Services.Repository.Service;

namespace Apzon.Services.SqlRepository.Service;

public class SqlServiceCallRepository : ServiceCallRepository
{
	public SqlServiceCallRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
