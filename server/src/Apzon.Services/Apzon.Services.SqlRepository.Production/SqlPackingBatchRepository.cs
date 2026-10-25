using Apzon.Services.Repository.Production;

namespace Apzon.Services.SqlRepository.Production;

public class SqlPackingBatchRepository : PackingBatchRepository
{
	public SqlPackingBatchRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
