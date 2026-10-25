using Apzon.Services.Repository.Administrator.Business_Partner;

namespace Apzon.Services.SqlRepository.Administrator.Business_Partner;

public class SqlGroupCustomerVenderRepository : GroupCustomerVenderRepository
{
	public SqlGroupCustomerVenderRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
