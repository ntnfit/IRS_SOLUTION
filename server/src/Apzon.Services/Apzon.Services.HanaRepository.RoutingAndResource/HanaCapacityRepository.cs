using Apzon.Services.Repository.RoutingAndResource;

namespace Apzon.Services.HanaRepository.RoutingAndResource;

public class HanaCapacityRepository : CapacityRepository
{
	public HanaCapacityRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
