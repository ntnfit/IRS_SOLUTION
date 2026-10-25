using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.HanaRepository.BusinessPartner;

public class HanaOCashDiscountRepository : OCashDiscountRepository
{
	public HanaOCashDiscountRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
