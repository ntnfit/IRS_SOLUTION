using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.SqlRepository.HumanResources.TimeSheeting;

public class SqlTimeSheetingHourRepository : TimeSheetingHourRepository
{
	public SqlTimeSheetingHourRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
