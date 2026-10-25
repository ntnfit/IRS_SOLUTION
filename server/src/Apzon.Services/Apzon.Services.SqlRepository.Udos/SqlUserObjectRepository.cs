using Apzon.Services.Repository.Udos;

namespace Apzon.Services.SqlRepository.Udos;

public class SqlUserObjectRepository : UserObjectRepository
{
	public SqlUserObjectRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
