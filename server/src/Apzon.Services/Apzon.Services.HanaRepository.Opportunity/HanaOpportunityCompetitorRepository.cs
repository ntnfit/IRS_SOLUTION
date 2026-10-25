using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.HanaRepository.Opportunity;

public class HanaOpportunityCompetitorRepository : OpportunityCompetitorRepository
{
	public HanaOpportunityCompetitorRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
