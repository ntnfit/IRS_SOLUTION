using Apzon.Services.Repository.RoutingAndResource;

namespace Apzon.Services.HanaRepository.RoutingAndResource;

public class HanaResourceWorkCenterRepository : ResourceWorkCenterRepository
{
	public HanaResourceWorkCenterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
