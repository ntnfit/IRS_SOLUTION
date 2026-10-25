using Apzon.Services.Repository.Administrator.Plugin;

namespace Apzon.Services.HanaRepository.Administrator.Plugin;

public class HanaPluginManagerRepository : PluginManagerRepository
{
	public HanaPluginManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
