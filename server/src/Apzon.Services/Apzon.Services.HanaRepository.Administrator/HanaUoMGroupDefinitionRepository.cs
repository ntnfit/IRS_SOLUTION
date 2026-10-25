using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaUoMGroupDefinitionRepository : UoMGroupDefinitionRepository
{
	public HanaUoMGroupDefinitionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
