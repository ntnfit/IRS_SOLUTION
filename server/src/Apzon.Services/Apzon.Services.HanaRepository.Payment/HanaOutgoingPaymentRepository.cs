using Apzon.Services.Repository.Payment;

namespace Apzon.Services.HanaRepository.Payment;

public class HanaOutgoingPaymentRepository : OutgoingPaymentRepository
{
	public HanaOutgoingPaymentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
