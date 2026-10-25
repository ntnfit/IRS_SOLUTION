using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.SqlRepository.BusinessPartner;

public class SqlOCashDiscountRepository : OCashDiscountRepository
{
	public SqlOCashDiscountRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
