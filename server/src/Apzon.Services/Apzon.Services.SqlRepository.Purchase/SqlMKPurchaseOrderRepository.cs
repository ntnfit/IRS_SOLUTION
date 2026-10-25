using Apzon.Services.Repository.Purchase;

namespace Apzon.Services.SqlRepository.Purchase;

public class SqlMKPurchaseOrderRepository : MKPurchaseOrderRepository
{
	public SqlMKPurchaseOrderRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
