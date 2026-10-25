using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.HanaRepository.ProdutionAddon;

public class HanaControlKeyRepository : ControlKeyRepository
{
	public HanaControlKeyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
