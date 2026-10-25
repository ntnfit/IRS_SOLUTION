using Apzon.Services.Repository.Production.PlanningProduction;

namespace Apzon.Services.SqlRepository.PlanningProduction;

public class SqlShiftRepository : ShiftRepository
{
	public SqlShiftRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
