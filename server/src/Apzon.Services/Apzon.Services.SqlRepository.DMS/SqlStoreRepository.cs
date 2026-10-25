using Apzon.Services.Repository.DMS;

namespace Apzon.Services.SqlRepository.DMS;

public class SqlStoreRepository : StoreRepository
{
	public SqlStoreRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
