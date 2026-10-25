using Apzon.Services.Repository.Production;

namespace Apzon.Services.SqlRepository.Production;

public class SqlBomVersionRepository : BomVersionRepository
{
	public SqlBomVersionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
