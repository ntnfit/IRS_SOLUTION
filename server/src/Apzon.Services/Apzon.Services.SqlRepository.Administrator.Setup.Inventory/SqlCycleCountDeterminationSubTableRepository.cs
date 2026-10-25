using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.SqlRepository.Administrator.Setup.Inventory;

internal class SqlCycleCountDeterminationSubTableRepository : CycleCountDeterminationSubTableRepository
{
	private const string TempLine = "#templine";

	public SqlCycleCountDeterminationSubTableRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
