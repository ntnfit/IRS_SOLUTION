using Apzon.Services.Repository.Administrator.Banking;

namespace Apzon.Services.HanaRepository.Administrator.Banking;

public class HanaCreditCardRepository : CreditCardRepository
{
	public HanaCreditCardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
