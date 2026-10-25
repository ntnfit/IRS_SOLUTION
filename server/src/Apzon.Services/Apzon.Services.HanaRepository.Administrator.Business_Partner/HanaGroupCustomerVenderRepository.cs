using Apzon.Services.Repository.Administrator.Business_Partner;

namespace Apzon.Services.HanaRepository.Administrator.Business_Partner;

public class HanaGroupCustomerVenderRepository : GroupCustomerVenderRepository
{
	public HanaGroupCustomerVenderRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
