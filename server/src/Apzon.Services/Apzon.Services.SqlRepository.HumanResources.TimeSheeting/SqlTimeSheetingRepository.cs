using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.SqlRepository.HumanResources.TimeSheeting;

public class SqlTimeSheetingRepository : TimeSheetingRepository
{
	public SqlTimeSheetingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
