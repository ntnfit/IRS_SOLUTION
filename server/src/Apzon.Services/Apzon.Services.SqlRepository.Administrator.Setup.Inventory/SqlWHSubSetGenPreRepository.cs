using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.SqlRepository.Administrator.Setup.Inventory;

internal class SqlWHSubSetGenPreRepository : WHSubSetGenPreRepository
{
	public SqlWHSubSetGenPreRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
