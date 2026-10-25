using Apzon.Services.Repository.Inventory.PriceLists;

namespace Apzon.Services.SqlRepository.Inventory.PriceLists;

public class SqlDiscountGroupRepository : DiscountGroupRepository
{
	public SqlDiscountGroupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
