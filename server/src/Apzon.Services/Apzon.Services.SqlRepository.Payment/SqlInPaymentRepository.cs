using Apzon.Services.Repository.Payment;

namespace Apzon.Services.SqlRepository.Payment;

public class SqlInPaymentRepository : InPaymentRepository
{
	public SqlInPaymentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
