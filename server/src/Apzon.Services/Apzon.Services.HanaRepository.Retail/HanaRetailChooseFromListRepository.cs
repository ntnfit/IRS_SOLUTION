using Apzon.Services.Repository.Retail;

namespace Apzon.Services.HanaRepository.Retail;

public class HanaRetailChooseFromListRepository : RetailChooseFromListRepository
{
	public HanaRetailChooseFromListRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
