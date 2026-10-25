using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.SqlRepository.ProdutionAddon;

public class SqlListPlantRepository : ListPlantRepository
{
	public SqlListPlantRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
