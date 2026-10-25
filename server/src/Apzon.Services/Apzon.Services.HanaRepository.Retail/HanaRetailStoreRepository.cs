using Apzon.Services.Repository.Retail;

namespace Apzon.Services.HanaRepository.Retail;

public class HanaRetailStoreRepository : RetailStoreRepository
{
	public HanaRetailStoreRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
