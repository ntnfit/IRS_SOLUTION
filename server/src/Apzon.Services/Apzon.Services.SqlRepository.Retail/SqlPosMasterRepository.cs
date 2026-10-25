using Apzon.Services.Repository.Retail;

namespace Apzon.Services.SqlRepository.Retail;

public class SqlPosMasterRepository : PosMasterRepository
{
	public SqlPosMasterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
