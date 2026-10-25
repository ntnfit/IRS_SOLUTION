using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.SqlRepository.Mrps;

public class SqlLinesRepository : LinesRepository
{
	public SqlLinesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
