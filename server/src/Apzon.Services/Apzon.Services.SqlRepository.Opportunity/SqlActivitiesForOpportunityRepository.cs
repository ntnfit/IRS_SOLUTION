using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.SqlRepository.Opportunity;

public class SqlActivitiesForOpportunityRepository : ActivitiesForOpportunityRepository
{
	public SqlActivitiesForOpportunityRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
