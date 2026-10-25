using Apzon.Services.Repository.PackingManagement;

namespace Apzon.Services.SqlRepository.PackingManagement;

public class SqlPackingOrderRepository : PackingOrderRepository
{
	public SqlPackingOrderRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
