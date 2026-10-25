using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.SqlRepository.HumanResources.TimeSheeting;

public class SqlWorkPeriodRepository : WorkPeriodRepository
{
	public SqlWorkPeriodRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
