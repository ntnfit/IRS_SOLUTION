using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlWorkingTableRepository : WorkingTableRepository
{
	public SqlWorkingTableRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
