using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.SqlRepository.BusinessPartner;

public class SqlCardGroupRepository : CardGroupRepository
{
	public SqlCardGroupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
