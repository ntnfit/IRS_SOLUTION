using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.HanaRepository.Opportunity;

public class HanaOpportunityReasonRepository : OpportunityReasonRepository
{
	public HanaOpportunityReasonRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
