using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.SqlRepository.Inventory;

public class SqlPackingRepository : PackingRepository
{
	public SqlPackingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
