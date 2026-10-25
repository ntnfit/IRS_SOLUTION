using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaMarkerRepository : MarkerRepository
{
	public HanaMarkerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
