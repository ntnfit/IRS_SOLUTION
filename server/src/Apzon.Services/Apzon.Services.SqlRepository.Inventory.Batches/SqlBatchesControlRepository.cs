using Apzon.Services.Repository.Inventory.Batches;

namespace Apzon.Services.SqlRepository.Inventory.Batches;

public class SqlBatchesControlRepository : BatchesControlRepository
{
	public SqlBatchesControlRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
