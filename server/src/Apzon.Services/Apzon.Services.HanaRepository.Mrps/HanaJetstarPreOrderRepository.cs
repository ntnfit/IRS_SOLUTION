using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.HanaRepository.Mrps;

public class HanaJetstarPreOrderRepository : JetstarPreOrderRepository
{
	public HanaJetstarPreOrderRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
