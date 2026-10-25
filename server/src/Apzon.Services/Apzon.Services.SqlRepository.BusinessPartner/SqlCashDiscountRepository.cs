using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.SqlRepository.BusinessPartner;

public class SqlCashDiscountRepository : CashDiscountRepository
{
	public SqlCashDiscountRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
