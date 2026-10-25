using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.SqlRepository.Administrator.Setup.Inventory;

internal class SqlCycleCountDeterminationRepository : CycleCountDeterminationRepository
{
	private const string TempLine = "#templine";

	public SqlCycleCountDeterminationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
