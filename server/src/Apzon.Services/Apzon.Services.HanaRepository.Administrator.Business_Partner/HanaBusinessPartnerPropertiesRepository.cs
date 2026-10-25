using Apzon.Services.Repository.Administrator.Business_Partner;

namespace Apzon.Services.HanaRepository.Administrator.Business_Partner;

public class HanaBusinessPartnerPropertiesRepository : BusinessPartnerPropertiesRepository
{
	public HanaBusinessPartnerPropertiesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
