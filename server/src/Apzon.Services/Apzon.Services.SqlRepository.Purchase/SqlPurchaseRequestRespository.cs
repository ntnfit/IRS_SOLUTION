using Apzon.Services.Repository.Purchase;

namespace Apzon.Services.SqlRepository.Purchase;

public class SqlPurchaseRequestRespository : PurchaseRequestRespository
{
	public SqlPurchaseRequestRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
