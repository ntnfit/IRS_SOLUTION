using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.SqlRepository.HumanResources;

public class SqlJVAnswerRepository : JVAnswerRepository
{
	public SqlJVAnswerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
