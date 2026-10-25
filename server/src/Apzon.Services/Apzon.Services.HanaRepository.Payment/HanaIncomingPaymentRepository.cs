using Apzon.Services.Repository.Payment;

namespace Apzon.Services.HanaRepository.Payment;

public class HanaIncomingPaymentRepository : IncomingPaymentRepository
{
	public HanaIncomingPaymentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
