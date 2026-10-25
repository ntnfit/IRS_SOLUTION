using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.HanaRepository.ProdutionAddon;

public class HanaRoutingRepository : RoutingRepository
{
	public HanaRoutingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
