using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.SqlRepository.BusinessPartner;

public class SqlBpAddressRepository : BpAddressRepository
{
	public SqlBpAddressRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
