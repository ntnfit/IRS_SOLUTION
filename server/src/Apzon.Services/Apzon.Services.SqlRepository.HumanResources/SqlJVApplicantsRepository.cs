using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.SqlRepository.HumanResources;

public class SqlJVApplicantsRepository : JVApplicantsRepository
{
	public SqlJVApplicantsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
