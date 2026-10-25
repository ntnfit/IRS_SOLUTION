using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

public class SqlUDORepository : UDORepository
{
	public SqlUDORepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
