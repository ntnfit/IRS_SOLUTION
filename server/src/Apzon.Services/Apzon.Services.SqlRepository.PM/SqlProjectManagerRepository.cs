using Apzon.Services.Repository.PM;

namespace Apzon.Services.SqlRepository.PM;

public class SqlProjectManagerRepository : ProjectManagerRepository
{
	public SqlProjectManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
