using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlBarConfigRepository : BarConfigRepository
{
	public SqlBarConfigRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
