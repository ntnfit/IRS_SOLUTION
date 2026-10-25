using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.SqlRepository.Opportunity;

public class SqlOpportunityReasonRepository : OpportunityReasonRepository
{
	public SqlOpportunityReasonRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
