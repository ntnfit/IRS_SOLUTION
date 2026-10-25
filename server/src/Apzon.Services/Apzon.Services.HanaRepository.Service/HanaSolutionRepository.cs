using Apzon.Services.Repository.Service;

namespace Apzon.Services.HanaRepository.Service;

public class HanaSolutionRepository : SolutionRepository
{
	public HanaSolutionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
