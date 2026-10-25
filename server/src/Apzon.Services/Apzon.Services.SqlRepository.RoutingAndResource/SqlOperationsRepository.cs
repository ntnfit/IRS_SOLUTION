using Apzon.Services.Repository.RoutingAndResource;

namespace Apzon.Services.SqlRepository.RoutingAndResource;

public class SqlOperationsRepository : OperationsRepository
{
	public SqlOperationsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
