using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlRequesterRepository : RequesterRepository
{
	public SqlRequesterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
