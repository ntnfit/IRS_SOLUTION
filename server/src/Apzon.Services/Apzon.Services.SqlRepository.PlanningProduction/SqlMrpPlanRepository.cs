using Apzon.Services.Repository.PlanningProduction;

namespace Apzon.Services.SqlRepository.PlanningProduction;

public class SqlMrpPlanRepository : MrpPlanRepository
{
	public SqlMrpPlanRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
