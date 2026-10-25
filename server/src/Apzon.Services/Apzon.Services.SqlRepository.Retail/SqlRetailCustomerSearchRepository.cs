using Apzon.Services.Repository.Retail;

namespace Apzon.Services.SqlRepository.Retail;

public class SqlRetailCustomerSearchRepository : RetailCustomerSearchRepository
{
	public SqlRetailCustomerSearchRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
