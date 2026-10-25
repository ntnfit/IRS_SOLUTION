using Apzon.Services.Repository.Payment;

namespace Apzon.Services.HanaRepository.Payment;

public class HanaPaymentReconRepository : PaymentReconRepository
{
	public HanaPaymentReconRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
