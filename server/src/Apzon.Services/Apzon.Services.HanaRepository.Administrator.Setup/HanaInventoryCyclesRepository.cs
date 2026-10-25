using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaInventoryCyclesRepository : InventoryCyclesRepository
{
	public HanaInventoryCyclesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
