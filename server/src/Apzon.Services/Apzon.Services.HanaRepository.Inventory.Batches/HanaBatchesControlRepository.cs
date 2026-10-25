using Apzon.Services.Repository.Inventory.Batches;

namespace Apzon.Services.HanaRepository.Inventory.Batches;

public class HanaBatchesControlRepository : BatchesControlRepository
{
	public HanaBatchesControlRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
