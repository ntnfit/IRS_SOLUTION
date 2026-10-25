using Apzon.Services.Repository.Inventory.Batches;

namespace Apzon.Services.SqlRepository.Inventory.Batches;

public class SqlBatchTransactionRepository : BatchTransactionRepository
{
	public SqlBatchTransactionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
