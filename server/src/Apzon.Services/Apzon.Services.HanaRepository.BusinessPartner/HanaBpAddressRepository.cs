using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.HanaRepository.BusinessPartner;

public class HanaBpAddressRepository : BpAddressRepository
{
	public HanaBpAddressRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
