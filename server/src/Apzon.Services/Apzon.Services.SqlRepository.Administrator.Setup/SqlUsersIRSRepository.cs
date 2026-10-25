using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

internal class SqlUsersIRSRepository : UsersIRSRepository
{
	public SqlUsersIRSRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
