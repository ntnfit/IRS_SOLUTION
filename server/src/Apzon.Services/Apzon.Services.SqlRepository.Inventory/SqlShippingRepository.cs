using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.SqlRepository.Inventory;

public class SqlShippingRepository : ShippingRepository
{
	public SqlShippingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
