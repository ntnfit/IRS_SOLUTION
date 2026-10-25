using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.SqlRepository.BusinessPartner;

public class SqlPaymentTermRepository : PaymentTermRepository
{
	public SqlPaymentTermRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
