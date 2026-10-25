using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

public class SqlUsersRepository : UsersRepository
{
	public SqlUsersRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
