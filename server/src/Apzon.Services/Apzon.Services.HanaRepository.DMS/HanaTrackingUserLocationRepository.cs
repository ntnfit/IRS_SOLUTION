using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaTrackingUserLocationRepository : TrackingUserLocationRepository
{
	public HanaTrackingUserLocationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
