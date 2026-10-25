using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.SqlRepository.Opportunity;

public class SqlActivityMeetingLocationRepository : ActivityMeetingLocationRepository
{
	public SqlActivityMeetingLocationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
