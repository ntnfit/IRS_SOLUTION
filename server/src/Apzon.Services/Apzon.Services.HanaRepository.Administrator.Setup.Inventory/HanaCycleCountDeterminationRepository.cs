using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.HanaRepository.Administrator.Setup.Inventory;

public class HanaCycleCountDeterminationRepository : CycleCountDeterminationRepository
{
	public HanaCycleCountDeterminationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
