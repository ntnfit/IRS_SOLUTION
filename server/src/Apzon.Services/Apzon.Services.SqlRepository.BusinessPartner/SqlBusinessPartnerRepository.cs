using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.SqlRepository.BusinessPartner;

public class SqlBusinessPartnerRepository : BusinessPartnerRepository
{
	public SqlBusinessPartnerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
