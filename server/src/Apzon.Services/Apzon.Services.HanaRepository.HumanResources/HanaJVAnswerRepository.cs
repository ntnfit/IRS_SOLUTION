using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.HanaRepository.HumanResources;

public class HanaJVAnswerRepository : JVAnswerRepository
{
	public HanaJVAnswerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
