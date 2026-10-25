using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

internal class SqlIrsConfigRepository : IrsConfigRepository
{
	public SqlIrsConfigRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
