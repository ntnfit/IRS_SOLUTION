using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.SqlRepository.HumanResources.TimeSheeting;

public class SqlHolidaySetupRepository : HolidaySetupRepository
{
	public SqlHolidaySetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
