using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.HanaRepository.Administrator.Setup.Inventory;

public class HanaBinFieldActivationRepository : BinFieldActivationRepository
{
	public HanaBinFieldActivationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
