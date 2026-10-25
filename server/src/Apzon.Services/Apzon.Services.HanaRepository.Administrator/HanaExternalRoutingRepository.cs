using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaExternalRoutingRepository : ExternalRoutingRepository
{
	public HanaExternalRoutingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
