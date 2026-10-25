using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlVatGroupsRepository : VatGroupsRepository
{
	public SqlVatGroupsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
