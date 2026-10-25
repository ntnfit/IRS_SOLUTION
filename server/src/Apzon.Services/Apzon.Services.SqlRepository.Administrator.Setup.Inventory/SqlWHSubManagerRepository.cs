using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.SqlRepository.Administrator.Setup.Inventory;

internal class SqlWHSubManagerRepository : WHSubManagerRepository
{
	public SqlWHSubManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
