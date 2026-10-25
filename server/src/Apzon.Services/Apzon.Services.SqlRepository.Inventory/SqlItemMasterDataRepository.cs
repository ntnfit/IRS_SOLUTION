using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.SqlRepository.Inventory;

public class SqlItemMasterDataRepository : ItemMasterDataRepository
{
	public SqlItemMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
