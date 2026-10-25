using Apzon.Services.Repository.Purchase;

namespace Apzon.Services.SqlRepository.Purchase;

public class SqlPurchaseRequestControlRespository : PurchaseRequestControlRespository
{
	public SqlPurchaseRequestControlRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
