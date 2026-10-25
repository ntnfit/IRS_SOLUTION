using Apzon.Services.Repository.Payment;

namespace Apzon.Services.SqlRepository.Payment;

public class SqlIncomingPaymentsRepository : IncomingPaymentsRepository
{
	public SqlIncomingPaymentsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
