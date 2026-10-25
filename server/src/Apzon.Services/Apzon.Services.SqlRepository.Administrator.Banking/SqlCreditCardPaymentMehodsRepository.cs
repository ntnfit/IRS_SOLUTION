using Apzon.Services.Repository.Administrator.Banking;

namespace Apzon.Services.SqlRepository.Administrator.Banking;

public class SqlCreditCardPaymentMehodsRepository : CreditCardPaymentMehodsRepository
{
	public SqlCreditCardPaymentMehodsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
