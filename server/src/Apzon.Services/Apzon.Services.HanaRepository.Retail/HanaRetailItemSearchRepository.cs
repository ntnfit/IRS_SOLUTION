using Apzon.Services.Repository.Retail;

namespace Apzon.Services.HanaRepository.Retail;

public class HanaRetailItemSearchRepository : RetailItemSearchRepository
{
	public HanaRetailItemSearchRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
