using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaProjectRepository : ProjectRepository
{
	public HanaProjectRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
