using Apzon.Services.Repository.Administrator.Plugin;

namespace Apzon.Services.SqlRepository.Administrator.Plugin;

public class SqlPluginManagerRepository : PluginManagerRepository
{
	public SqlPluginManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
