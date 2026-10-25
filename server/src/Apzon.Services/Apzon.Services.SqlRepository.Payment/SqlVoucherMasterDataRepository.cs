using Apzon.Services.Repository.Payment;

namespace Apzon.Services.SqlRepository.Payment;

public class SqlVoucherMasterDataRepository : VoucherMasterDataRepository
{
	public SqlVoucherMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
