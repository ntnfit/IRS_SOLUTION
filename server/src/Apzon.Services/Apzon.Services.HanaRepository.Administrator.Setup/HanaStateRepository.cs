using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaStateRepository : StateRepository
{
	public HanaStateRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
