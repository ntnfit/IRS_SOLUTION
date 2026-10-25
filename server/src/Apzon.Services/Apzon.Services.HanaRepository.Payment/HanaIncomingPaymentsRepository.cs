using Apzon.Services.Repository.Payment;

namespace Apzon.Services.HanaRepository.Payment;

public class HanaIncomingPaymentsRepository : IncomingPaymentsRepository
{
	public HanaIncomingPaymentsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
