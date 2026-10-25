using Apzon.Services.Repository.Administrator.SystemInitialization;

namespace Apzon.Services.HanaRepository.Administrator.SystemInitialization;

public class HanaMenuStructureRepository : MenuStructureRepository
{
	public HanaMenuStructureRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
