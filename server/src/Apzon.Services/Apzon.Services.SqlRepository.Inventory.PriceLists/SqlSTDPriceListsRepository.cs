using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.SqlRepository.Inventory.PriceLists;

public class SqlSTDPriceListsRepository : STDPriceListsRepository
{
	public SqlSTDPriceListsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
