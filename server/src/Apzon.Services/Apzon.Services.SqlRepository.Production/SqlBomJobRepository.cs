using Apzon.Services.Repository.Production;

namespace Apzon.Services.SqlRepository.Production;

public class SqlBomJobRepository : BoMJobRepository
{
	public SqlBomJobRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
