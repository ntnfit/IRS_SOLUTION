using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.HanaRepository.BusinessPartner;

public class HanaCardGroupRepository : CardGroupRepository
{
	public HanaCardGroupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
