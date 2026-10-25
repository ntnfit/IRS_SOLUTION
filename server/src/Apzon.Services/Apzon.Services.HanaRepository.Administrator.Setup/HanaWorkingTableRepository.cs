using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaWorkingTableRepository : WorkingTableRepository
{
	public HanaWorkingTableRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
