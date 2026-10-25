using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.SqlRepository.HumanResources.TimeSheeting;

public class SqlWorkShiftRepository : WorkShiftRepository
{
	public SqlWorkShiftRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
