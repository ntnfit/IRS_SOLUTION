using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.HanaRepository.BusinessPartner;

public class HanaCreditCardTypeRepository : CreditCardTypeRepository
{
	public HanaCreditCardTypeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
