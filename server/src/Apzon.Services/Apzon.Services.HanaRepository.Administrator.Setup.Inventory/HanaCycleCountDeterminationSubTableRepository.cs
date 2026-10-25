using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.HanaRepository.Administrator.Setup.Inventory;

public class HanaCycleCountDeterminationSubTableRepository : CycleCountDeterminationSubTableRepository
{
	public HanaCycleCountDeterminationSubTableRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
