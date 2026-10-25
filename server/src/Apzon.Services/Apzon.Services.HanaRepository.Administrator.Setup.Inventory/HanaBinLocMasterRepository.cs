using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.HanaRepository.Administrator.Setup.Inventory;

public class HanaBinLocMasterRepository : BinLocMasterRepository
{
	public HanaBinLocMasterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
