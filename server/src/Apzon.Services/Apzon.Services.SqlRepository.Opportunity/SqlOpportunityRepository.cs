using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.SqlRepository.Opportunity;

public class SqlOpportunityRepository : OpportunityRepository
{
	public SqlOpportunityRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
