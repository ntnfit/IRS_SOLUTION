using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.HanaRepository.ProdutionAddon;

public class HanaResourcesRepository : ResourcesRepository
{
	public HanaResourcesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
