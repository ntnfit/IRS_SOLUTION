using Apzon.Services.Repository.Integrated;

namespace Apzon.Services.SqlRepository.Integrated;

public class SqlIntegratedItemRepository : IntegratedItemRepository
{
	public SqlIntegratedItemRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
