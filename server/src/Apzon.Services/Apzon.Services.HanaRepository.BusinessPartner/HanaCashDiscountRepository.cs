using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.HanaRepository.BusinessPartner;

public class HanaCashDiscountRepository : CashDiscountRepository
{
	public HanaCashDiscountRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
