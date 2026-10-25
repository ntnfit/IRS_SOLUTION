using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaUserDefaultRepository : UserDefaultRepository
{
	public HanaUserDefaultRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
