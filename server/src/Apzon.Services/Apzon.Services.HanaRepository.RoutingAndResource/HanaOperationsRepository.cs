using Apzon.Services.Repository.RoutingAndResource;

namespace Apzon.Services.HanaRepository.RoutingAndResource;

public class HanaOperationsRepository : OperationsRepository
{
	public HanaOperationsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
