using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.SqlRepository.HumanResources.TimeSheeting;

public class SqlWorkScheduleRegisterRepository : WorkScheduleRegisterRepository
{
	public SqlWorkScheduleRegisterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
