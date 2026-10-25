using Apzon.Services.Repository.Retail;

namespace Apzon.Services.SqlRepository.Retail;

public class SqlRetailItemSearchRepository : RetailItemSearchRepository
{
	public SqlRetailItemSearchRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
