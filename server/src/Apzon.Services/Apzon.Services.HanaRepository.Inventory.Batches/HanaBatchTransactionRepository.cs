using Apzon.Services.Repository.Inventory.Batches;

namespace Apzon.Services.HanaRepository.Inventory.Batches;

public class HanaBatchTransactionRepository : BatchTransactionRepository
{
	public HanaBatchTransactionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
