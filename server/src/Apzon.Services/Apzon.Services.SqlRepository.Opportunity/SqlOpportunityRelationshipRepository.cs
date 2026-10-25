using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.SqlRepository.Opportunity;

public class SqlOpportunityRelationshipRepository : OpportunityRelationshipRepository
{
	public SqlOpportunityRelationshipRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
