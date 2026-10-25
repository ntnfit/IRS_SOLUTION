using Apzon.Services.Repository.Payment;

namespace Apzon.Services.HanaRepository.Payment;

public class HanaPaymentRequestRepository : PaymentRequestRepository
{
	public HanaPaymentRequestRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
