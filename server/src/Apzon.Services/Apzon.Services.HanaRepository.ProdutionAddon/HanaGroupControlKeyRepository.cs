using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.HanaRepository.ProdutionAddon;

public class HanaGroupControlKeyRepository : GroupControlKeyRepository
{
	public HanaGroupControlKeyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
