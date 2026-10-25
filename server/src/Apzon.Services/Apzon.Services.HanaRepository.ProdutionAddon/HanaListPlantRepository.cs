using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.HanaRepository.ProdutionAddon;

public class HanaListPlantRepository : ListPlantRepository
{
	public HanaListPlantRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
