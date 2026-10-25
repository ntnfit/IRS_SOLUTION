using Apzon.Services.Repository.Administrator.Banking;

namespace Apzon.Services.HanaRepository.Administrator.Banking;

public class HanaCreditCardPaymentMehodsRepository : CreditCardPaymentMehodsRepository
{
	public HanaCreditCardPaymentMehodsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
