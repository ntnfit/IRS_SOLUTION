using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaCustomsGroupRepository : CustomsGroupRepository
{
	public HanaCustomsGroupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
