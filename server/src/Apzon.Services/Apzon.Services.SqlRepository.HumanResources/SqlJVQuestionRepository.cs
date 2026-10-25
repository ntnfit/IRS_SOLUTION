using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.SqlRepository.HumanResources;

public class SqlJVQuestionRepository : JVQuestionRepository
{
	public SqlJVQuestionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
