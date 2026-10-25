using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.HanaRepository.BusinessPartner;

public class HanaBusinessPartnerRepository : BusinessPartnerRepository
{
	public HanaBusinessPartnerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
