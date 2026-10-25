using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

public class SqlPrintLayoutRepository : PrintLayoutRepository
{
	public SqlPrintLayoutRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
