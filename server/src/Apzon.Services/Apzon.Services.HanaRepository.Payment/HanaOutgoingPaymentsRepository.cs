using Apzon.Services.Repository.Payment;

namespace Apzon.Services.HanaRepository.Payment;

public class HanaOutgoingPaymentsRepository : OutgoingPaymentsRepository
{
	public HanaOutgoingPaymentsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
