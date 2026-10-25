using Apzon.Services.Repository.Sales;

namespace Apzon.Services.SqlRepository.Sales;

public class SqlSalesBlanketAgreementRepository : SalesBlanketAgreementRepository
{
	public SqlSalesBlanketAgreementRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
