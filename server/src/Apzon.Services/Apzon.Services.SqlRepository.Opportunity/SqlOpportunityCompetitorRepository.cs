using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.SqlRepository.Opportunity;

public class SqlOpportunityCompetitorRepository : OpportunityCompetitorRepository
{
	public SqlOpportunityCompetitorRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
