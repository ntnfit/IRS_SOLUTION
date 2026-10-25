using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.HanaRepository.ProdutionAddon;

public class HanaMasterOrderReplanRepository : MasterOrderReplanRepository
{
	public HanaMasterOrderReplanRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
