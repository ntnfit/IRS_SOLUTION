using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.HanaRepository.Administrator.Setup.Inventory;

public class HanaBinLocListRepository : BinLocListRepository
{
	public HanaBinLocListRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
