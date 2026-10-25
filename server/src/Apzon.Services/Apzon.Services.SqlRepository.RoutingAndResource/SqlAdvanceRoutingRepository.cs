using Apzon.Services.Repository.RoutingAndResource;

namespace Apzon.Services.SqlRepository.RoutingAndResource;

public class SqlAdvanceRoutingRepository : AdvanceRoutingRepository
{
	public SqlAdvanceRoutingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
