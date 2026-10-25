using Apzon.Services.Repository.Inventory.Batches;

namespace Apzon.Services.HanaRepository.Inventory.Batches;

public class HanaBatchDetailsRepository : BatchDetailsRepository
{
	public HanaBatchDetailsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
