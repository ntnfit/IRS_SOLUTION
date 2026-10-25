using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

internal class SqlContextActionRepository : ContextActionRepository
{
	public SqlContextActionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
