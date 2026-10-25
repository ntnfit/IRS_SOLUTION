using Apzon.Services.Repository.Payment;

namespace Apzon.Services.HanaRepository.Payment;

public class HanaCouponItemMasterDataRepository : CouponMasterDataRepository
{
	public HanaCouponItemMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
