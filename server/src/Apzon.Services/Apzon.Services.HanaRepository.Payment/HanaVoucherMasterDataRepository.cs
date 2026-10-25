using Apzon.Services.Repository.Payment;

namespace Apzon.Services.HanaRepository.Payment;

public class HanaVoucherMasterDataRepository : VoucherMasterDataRepository
{
	public HanaVoucherMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
