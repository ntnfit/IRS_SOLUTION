using Apzon.Services.Repository.PM;

namespace Apzon.Services.SqlRepository.PM;

public class SqlTrackingIssueRepository : TrackingIssueRepository
{
	public SqlTrackingIssueRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
