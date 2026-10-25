using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.HanaRepository.Opportunity;

public class HanaOpportunityRelationshipRepository : OpportunityRelationshipRepository
{
	public HanaOpportunityRelationshipRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
