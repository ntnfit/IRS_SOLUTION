using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaStoreRepository : StoreRepository
{
	public HanaStoreRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
