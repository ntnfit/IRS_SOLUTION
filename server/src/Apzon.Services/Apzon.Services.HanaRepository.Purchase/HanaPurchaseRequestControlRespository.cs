using Apzon.Services.Repository.Purchase;

namespace Apzon.Services.HanaRepository.Purchase;

public class HanaPurchaseRequestControlRespository : PurchaseRequestControlRespository
{
	public HanaPurchaseRequestControlRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
