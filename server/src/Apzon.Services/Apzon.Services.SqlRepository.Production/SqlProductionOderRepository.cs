using Apzon.Services.Repository.Production;

namespace Apzon.Services.SqlRepository.Production;

public class SqlProductionOderRepository : ProductionOderRepository
{
	public SqlProductionOderRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
