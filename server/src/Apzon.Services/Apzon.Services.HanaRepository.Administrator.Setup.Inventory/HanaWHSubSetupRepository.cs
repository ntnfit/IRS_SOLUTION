using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.HanaRepository.Administrator.Setup.Inventory;

public class HanaWHSubSetupRepository : WHSubSetupRepository
{
	public HanaWHSubSetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
