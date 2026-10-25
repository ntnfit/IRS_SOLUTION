using Apzon.Services.Repository.Payment;

namespace Apzon.Services.HanaRepository.Payment;

public class HanaCashItemMasterDataRepository : CashItemMasterDataRepository
{
	public HanaCashItemMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
