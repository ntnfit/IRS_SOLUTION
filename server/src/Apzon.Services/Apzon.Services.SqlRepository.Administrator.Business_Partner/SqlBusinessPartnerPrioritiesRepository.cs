using Apzon.Services.Repository.Administrator.Business_Partner;

namespace Apzon.Services.SqlRepository.Administrator.Business_Partner;

public class SqlBusinessPartnerPrioritiesRepository : BusinessPartnerPrioritiesRepository
{
	public SqlBusinessPartnerPrioritiesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
