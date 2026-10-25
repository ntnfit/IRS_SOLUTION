using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.SqlRepository.HumanResources;

public class SqlHREducationRepository : HREducationRepository
{
	public SqlHREducationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
