using Apzon.Services.Repository.Purchase;

namespace Apzon.Services.HanaRepository.Purchase;

public class HanaMKPurchaseOrderRepository : MKPurchaseOrderRepository
{
	public HanaMKPurchaseOrderRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
