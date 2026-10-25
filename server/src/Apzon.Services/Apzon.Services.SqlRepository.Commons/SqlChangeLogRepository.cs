using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

public class SqlChangeLogRepository : ChangeLogRepository
{
	public SqlChangeLogRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
