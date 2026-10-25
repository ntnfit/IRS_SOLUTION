using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.SqlRepository.Inventory.PickAndPack.PriceLists;

public class SqlPeriodAndVolumeDiscountsRepository : PeriodAndVolumeDiscountsRepository
{
	public SqlPeriodAndVolumeDiscountsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
