using Apzon.Services.Repository.DMS;

namespace Apzon.Services.SqlRepository.DMS;

public class SqlTrackingUserLocationRepository : TrackingUserLocationRepository
{
	public SqlTrackingUserLocationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
