using Apzon.Services.Repository.Integrated;

namespace Apzon.Services.HanaRepository.Integrated;

public class HanaIntegratedInventoryTransferRepository : IntegratedInventoryTransferRepository
{
	public HanaIntegratedInventoryTransferRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
