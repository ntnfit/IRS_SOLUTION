using Apzon.Services.Repository.Payment;

namespace Apzon.Services.SqlRepository.Payment;

public class SqlCouponItemMasterDataRepository : CouponMasterDataRepository
{
	public SqlCouponItemMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
