using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlLocationRepository : LocationRepository
{
	public SqlLocationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
