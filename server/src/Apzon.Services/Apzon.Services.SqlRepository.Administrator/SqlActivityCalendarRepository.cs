using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

internal class SqlActivityCalendarRepository : ActivityCalendarRepository
{
	public SqlActivityCalendarRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
