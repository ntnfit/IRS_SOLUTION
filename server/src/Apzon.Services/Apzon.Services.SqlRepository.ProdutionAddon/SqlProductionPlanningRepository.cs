using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.SqlRepository.ProdutionAddon;

public class SqlProductionPlanningRepository : ProductionPlanningRepository
{
	public SqlProductionPlanningRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
