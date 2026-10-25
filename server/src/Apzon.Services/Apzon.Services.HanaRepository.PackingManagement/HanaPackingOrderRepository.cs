using Apzon.Services.Repository.PackingManagement;

namespace Apzon.Services.HanaRepository.PackingManagement;

public class HanaPackingOrderRepository : PackingOrderRepository
{
	public HanaPackingOrderRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
