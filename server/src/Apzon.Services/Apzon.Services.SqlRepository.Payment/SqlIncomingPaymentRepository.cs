using Apzon.Services.Repository.Payment;

namespace Apzon.Services.SqlRepository.Payment;

public class SqlIncomingPaymentRepository : IncomingPaymentRepository
{
	public SqlIncomingPaymentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
