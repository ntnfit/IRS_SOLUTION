using Apzon.Services.Repository.Production;

namespace Apzon.Services.SqlRepository.Production;

public class SqlProductionForBatchRepository : ProductionForBatchRepository
{
	public SqlProductionForBatchRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
