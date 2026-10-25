using Apzon.Services.Repository.Payment;

namespace Apzon.Services.HanaRepository.Payment;

public class HanaOutPaymentRepository : OutPaymentRepository
{
	public HanaOutPaymentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
