using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.HanaRepository.ProdutionAddon;

public class HanaTinhGiayRepository : TinhGiayRepository
{
	public HanaTinhGiayRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
