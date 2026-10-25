using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.SqlRepository.HumanResources;

public class SqlJobVacancyRepository : JobVacancyRepository
{
	public SqlJobVacancyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
