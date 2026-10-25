using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

public class SqlUDRRepository : UDRRepository
{
	public SqlUDRRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
