using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.HanaRepository.HumanResources;

public class HanaJobVacancyRepository : JobVacancyRepository
{
	public HanaJobVacancyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
