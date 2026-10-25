using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.HanaRepository.Opportunity;

public class HanaActivityMeetingLocationRepository : ActivityMeetingLocationRepository
{
	public HanaActivityMeetingLocationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
