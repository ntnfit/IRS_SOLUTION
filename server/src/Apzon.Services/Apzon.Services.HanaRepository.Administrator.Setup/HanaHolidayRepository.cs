using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaHolidayRepository : HolidayRepository
{
	public HanaHolidayRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
