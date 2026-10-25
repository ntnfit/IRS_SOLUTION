using Apzon.Services.Repository.Payment;

namespace Apzon.Services.SqlRepository.Payment;

public class SqlPaymentReconRepository : PaymentReconRepository
{
	public SqlPaymentReconRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
