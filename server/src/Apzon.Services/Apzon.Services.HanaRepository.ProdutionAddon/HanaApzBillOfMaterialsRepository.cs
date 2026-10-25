using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.HanaRepository.ProdutionAddon;

public class HanaApzBillOfMaterialsRepository : ApzBillOfMaterialsRepository
{
	public HanaApzBillOfMaterialsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
