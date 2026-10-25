using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.HanaRepository.Opportunity;

public class HanaOpportunityPartnerRepository : OpportunityPartnerRepository
{
	public HanaOpportunityPartnerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
