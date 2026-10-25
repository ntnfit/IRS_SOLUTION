using Apzon.Services.Repository.Payment;

namespace Apzon.Services.SqlRepository.Payment;

public class SqlOutPaymentRepository : OutPaymentRepository
{
	public SqlOutPaymentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
