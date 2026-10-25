using Apzon.Services.Repository.Payment;

namespace Apzon.Services.SqlRepository.Payment;

public class SqlPaymentRequestRepository : PaymentRequestRepository
{
	public SqlPaymentRequestRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
