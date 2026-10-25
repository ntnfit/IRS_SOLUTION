using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.SqlRepository.HumanResources.TimeSheeting;

public class SqlWorkScheduleSetupRepository : WorkScheduleSetupRepository
{
	public SqlWorkScheduleSetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
