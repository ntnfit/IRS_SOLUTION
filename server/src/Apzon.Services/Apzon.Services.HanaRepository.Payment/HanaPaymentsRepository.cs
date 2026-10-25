using Apzon.Services.Repository.Payment;

namespace Apzon.Services.HanaRepository.Payment;

internal class HanaPaymentsRepository : PaymentsRepository
{
	public HanaPaymentsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
