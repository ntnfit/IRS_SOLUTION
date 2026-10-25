using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.SqlRepository.Opportunity;

public class SqlOpportunityStageRepository : OpportunityStageRepository
{
	public SqlOpportunityStageRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
