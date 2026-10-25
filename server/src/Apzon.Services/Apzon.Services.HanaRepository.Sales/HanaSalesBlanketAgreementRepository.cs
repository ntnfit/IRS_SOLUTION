using Apzon.Services.Repository.Sales;

namespace Apzon.Services.HanaRepository.Sales;

public class HanaSalesBlanketAgreementRepository : SalesBlanketAgreementRepository
{
	public HanaSalesBlanketAgreementRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
