using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.SqlRepository.Administrator.Setup.Inventory;

internal class SqlWHSubSetupRepository : WHSubSetupRepository
{
	public SqlWHSubSetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
