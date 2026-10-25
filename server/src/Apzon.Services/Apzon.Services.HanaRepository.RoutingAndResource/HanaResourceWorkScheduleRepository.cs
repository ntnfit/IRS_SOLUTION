using Apzon.Services.Repository.RoutingAndResource;

namespace Apzon.Services.HanaRepository.RoutingAndResource;

public class HanaResourceWorkScheduleRepository : ResourceWorkScheduleRepository
{
	public HanaResourceWorkScheduleRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
