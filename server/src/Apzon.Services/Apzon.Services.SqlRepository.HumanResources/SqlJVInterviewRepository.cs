using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.SqlRepository.HumanResources;

public class SqlJVInterviewRepository : JVInterviewRepository
{
	public SqlJVInterviewRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
