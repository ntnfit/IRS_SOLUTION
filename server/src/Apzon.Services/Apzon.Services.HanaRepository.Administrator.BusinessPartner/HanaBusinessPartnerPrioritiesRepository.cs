using Apzon.Services.Repository.Administrator.Business_Partner;

namespace Apzon.Services.HanaRepository.Administrator.BusinessPartner;

public class HanaBusinessPartnerPrioritiesRepository : BusinessPartnerPrioritiesRepository
{
	public HanaBusinessPartnerPrioritiesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
