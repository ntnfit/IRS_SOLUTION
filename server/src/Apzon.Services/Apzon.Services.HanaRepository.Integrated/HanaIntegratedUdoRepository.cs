using Apzon.Services.Repository.Integrated;

namespace Apzon.Services.HanaRepository.Integrated;

public class HanaIntegratedUdoRepository : IntegratedUdoRepository
{
	public HanaIntegratedUdoRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
