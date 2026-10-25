using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.SqlRepository.BusinessPartner;

public class SqlCreditCardTypeRepository : CreditCardTypeRepository
{
	public SqlCreditCardTypeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
