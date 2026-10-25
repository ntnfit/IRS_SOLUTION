using Apzon.Services.Repository.Production.PlanningProduction;

namespace Apzon.Services.SqlRepository.PlanningProduction;

public class SqlWorkSchedulingRepository : WorkSchedulingRepository
{
	public SqlWorkSchedulingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
