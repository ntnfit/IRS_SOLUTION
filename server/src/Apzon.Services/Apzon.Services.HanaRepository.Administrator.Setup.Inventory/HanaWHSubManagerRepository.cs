using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.HanaRepository.Administrator.Setup.Inventory;

public class HanaWHSubManagerRepository : WHSubManagerRepository
{
	public HanaWHSubManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
