using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

public class SqlUDTRepository : UDTRepository
{
	public SqlUDTRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
