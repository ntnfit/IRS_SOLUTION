using Apzon.Services.Repository.Payment;

namespace Apzon.Services.HanaRepository.Payment;

public class HanaPaymentWizardRepository : PaymentWizardRepository
{
	public HanaPaymentWizardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
