using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaVisitStoreRepository : VisitStoreRepository
{
	public HanaVisitStoreRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
