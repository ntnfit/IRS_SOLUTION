using Apzon.Services.Repository.Payment;

namespace Apzon.Services.SqlRepository.Payment;

public class SqlOutgoingPaymentsRepository : OutgoingPaymentsRepository
{
	public SqlOutgoingPaymentsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
