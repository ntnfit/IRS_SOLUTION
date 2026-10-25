using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

public class SqlExRateAndIndexRepository : ExRateAndIndexRepository
{
	public SqlExRateAndIndexRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
