using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

public class SqlCFLRepository : CFLRepository
{
	public SqlCFLRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
