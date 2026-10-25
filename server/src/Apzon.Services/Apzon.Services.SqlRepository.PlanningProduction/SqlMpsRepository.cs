using Apzon.Services.Repository.PlanningProduction;

namespace Apzon.Services.SqlRepository.PlanningProduction;

public class SqlMpsRepository : MpsRepository
{
	public SqlMpsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
