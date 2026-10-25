using Apzon.Services.Repository.Integrated;

namespace Apzon.Services.SqlRepository.Integrated;

public class SqlIntegratedRepository : IntegratedRepository
{
	public SqlIntegratedRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
