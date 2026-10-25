using Apzon.Services.Repository.RoutingAndResource;

namespace Apzon.Services.HanaRepository.RoutingAndResource;

public class HanaAdvanceRoutingRepository : AdvanceRoutingRepository
{
	public HanaAdvanceRoutingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
