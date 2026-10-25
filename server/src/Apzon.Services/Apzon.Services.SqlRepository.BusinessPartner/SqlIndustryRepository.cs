using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.SqlRepository.BusinessPartner;

public class SqlIndustryRepository : IndustryRepository
{
	public SqlIndustryRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
