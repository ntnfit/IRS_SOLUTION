using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.HanaRepository.BusinessPartner;

public class HanaIndustryRepository : IndustryRepository
{
	public HanaIndustryRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
