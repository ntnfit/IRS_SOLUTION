using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaPrintLayoutRepository : PrintLayoutRepository
{
	public HanaPrintLayoutRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
