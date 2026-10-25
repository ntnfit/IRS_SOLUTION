using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.SqlRepository.ProdutionAddon;

public class SqlControlKeyRepository : ControlKeyRepository
{
	public SqlControlKeyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
