using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

internal class SqlDefineNewRepository : DefineNewRepository
{
	public SqlDefineNewRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
