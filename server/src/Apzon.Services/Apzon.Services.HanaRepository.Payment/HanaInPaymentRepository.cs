using Apzon.Services.Repository.Payment;

namespace Apzon.Services.HanaRepository.Payment;

public class HanaInPaymentRepository : InPaymentRepository
{
	public HanaInPaymentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
