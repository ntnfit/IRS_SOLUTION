using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.SqlRepository.Opportunity;

public class SqlOpportunityPartnerRepository : OpportunityPartnerRepository
{
	public SqlOpportunityPartnerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
