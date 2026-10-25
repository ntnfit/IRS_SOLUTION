using Apzon.Services.Repository.Inventory.PickAndPack;

namespace Apzon.Services.SqlRepository.Inventory.PickAndPack;

public class SqlPickingRepository : PickingRepository
{
	public SqlPickingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
