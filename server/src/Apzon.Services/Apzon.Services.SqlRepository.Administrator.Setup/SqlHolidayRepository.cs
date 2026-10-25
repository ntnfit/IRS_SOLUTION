using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlHolidayRepository : HolidayRepository
{
	public SqlHolidayRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
