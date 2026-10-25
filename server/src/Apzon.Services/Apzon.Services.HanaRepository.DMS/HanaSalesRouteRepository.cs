using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaSalesRouteRepository : SalesRouteRepository
{
	public HanaSalesRouteRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
