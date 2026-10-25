using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.SqlRepository.ProdutionAddon;

public class SqlGroupControlKeyRepository : GroupControlKeyRepository
{
	public SqlGroupControlKeyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
