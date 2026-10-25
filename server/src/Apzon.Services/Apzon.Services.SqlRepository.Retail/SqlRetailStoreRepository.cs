using Apzon.Services.Repository.Retail;

namespace Apzon.Services.SqlRepository.Retail;

public class SqlRetailStoreRepository : RetailStoreRepository
{
	public SqlRetailStoreRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
