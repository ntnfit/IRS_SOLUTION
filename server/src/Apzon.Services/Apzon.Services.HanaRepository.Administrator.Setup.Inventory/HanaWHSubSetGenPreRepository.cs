using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.HanaRepository.Administrator.Setup.Inventory;

public class HanaWHSubSetGenPreRepository : WHSubSetGenPreRepository
{
	public HanaWHSubSetGenPreRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
