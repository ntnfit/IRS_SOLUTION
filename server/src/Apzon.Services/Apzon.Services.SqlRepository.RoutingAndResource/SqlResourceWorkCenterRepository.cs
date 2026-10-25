using Apzon.Services.Repository.RoutingAndResource;

namespace Apzon.Services.SqlRepository.RoutingAndResource;

public class SqlResourceWorkCenterRepository : ResourceWorkCenterRepository
{
	public SqlResourceWorkCenterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
