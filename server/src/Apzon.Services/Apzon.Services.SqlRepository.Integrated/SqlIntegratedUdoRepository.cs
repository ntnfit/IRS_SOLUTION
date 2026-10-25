using Apzon.Services.Repository.Integrated;

namespace Apzon.Services.SqlRepository.Integrated;

public class SqlIntegratedUdoRepository : IntegratedUdoRepository
{
	public SqlIntegratedUdoRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
