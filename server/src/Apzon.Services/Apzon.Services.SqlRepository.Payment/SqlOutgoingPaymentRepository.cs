using Apzon.Services.Repository.Payment;

namespace Apzon.Services.SqlRepository.Payment;

public class SqlOutgoingPaymentRepository : OutgoingPaymentRepository
{
	public SqlOutgoingPaymentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
