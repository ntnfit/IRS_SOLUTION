using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

public class SqlCustomsGroupRepository : CustomsGroupRepository
{
	public SqlCustomsGroupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
