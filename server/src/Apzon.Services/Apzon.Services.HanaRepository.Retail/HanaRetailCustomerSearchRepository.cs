using Apzon.Services.Repository.Retail;

namespace Apzon.Services.HanaRepository.Retail;

public class HanaRetailCustomerSearchRepository : RetailCustomerSearchRepository
{
	public HanaRetailCustomerSearchRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
