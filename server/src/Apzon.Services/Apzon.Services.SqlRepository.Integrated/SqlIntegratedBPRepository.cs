using Apzon.Services.Repository.Integrated;

namespace Apzon.Services.SqlRepository.Integrated;

public class SqlIntegratedBPRepository : IntegratedBPRepository
{
	public SqlIntegratedBPRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
