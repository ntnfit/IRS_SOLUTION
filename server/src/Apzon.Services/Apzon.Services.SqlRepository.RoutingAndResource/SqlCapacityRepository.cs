using Apzon.Services.Repository.RoutingAndResource;

namespace Apzon.Services.SqlRepository.RoutingAndResource;

public class SqlCapacityRepository : CapacityRepository
{
	public SqlCapacityRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
