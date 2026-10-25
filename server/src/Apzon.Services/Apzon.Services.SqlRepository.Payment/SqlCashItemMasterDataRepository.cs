using Apzon.Services.Repository.Payment;

namespace Apzon.Services.SqlRepository.Payment;

public class SqlCashItemMasterDataRepository : CashItemMasterDataRepository
{
	public SqlCashItemMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
