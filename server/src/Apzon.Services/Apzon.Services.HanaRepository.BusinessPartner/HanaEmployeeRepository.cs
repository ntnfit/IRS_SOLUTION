using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.HanaRepository.BusinessPartner;

public class HanaEmployeeRepository : SaleEmployeeRepository
{
	public HanaEmployeeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
