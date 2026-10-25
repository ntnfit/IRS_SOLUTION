using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaBranchesRepository : BranchesRepository
{
	public HanaBranchesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
