using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.SqlRepository.ProdutionAddon;

public class SqlResourcesRepository : ResourcesRepository
{
	public SqlResourcesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
