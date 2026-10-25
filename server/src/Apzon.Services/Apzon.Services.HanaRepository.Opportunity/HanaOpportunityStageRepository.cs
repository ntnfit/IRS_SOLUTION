using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.HanaRepository.Opportunity;

public class HanaOpportunityStageRepository : OpportunityStageRepository
{
	public HanaOpportunityStageRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
