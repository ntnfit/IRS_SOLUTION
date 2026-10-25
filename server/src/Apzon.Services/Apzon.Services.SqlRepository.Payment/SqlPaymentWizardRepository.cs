using Apzon.Services.Repository.Payment;

namespace Apzon.Services.SqlRepository.Payment;

public class SqlPaymentWizardRepository : PaymentWizardRepository
{
	public SqlPaymentWizardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
