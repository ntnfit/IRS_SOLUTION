using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlTerritoryRepository : TerritoryRepository
{
	public SqlTerritoryRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
