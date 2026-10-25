using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlProjectRepository : ProjectRepository
{
	public SqlProjectRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
