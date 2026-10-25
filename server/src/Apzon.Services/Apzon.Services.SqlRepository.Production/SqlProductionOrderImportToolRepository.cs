using Apzon.Services.Repository.Production;

namespace Apzon.Services.SqlRepository.Production;

public class SqlProductionOrderImportToolRepository : ProductionOrderImportToolRepository
{
	public SqlProductionOrderImportToolRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
