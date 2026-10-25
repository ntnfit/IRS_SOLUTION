using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

internal class SqlORWRepository : ORWRepository
{
	public SqlORWRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
