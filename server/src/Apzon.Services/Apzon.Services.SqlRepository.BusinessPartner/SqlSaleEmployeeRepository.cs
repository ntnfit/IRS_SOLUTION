using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.SqlRepository.BusinessPartner;

public class SqlSaleEmployeeRepository : SaleEmployeeRepository
{
	public SqlSaleEmployeeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
