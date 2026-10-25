using Apzon.Services.Repository.Integrated;

namespace Apzon.Services.SqlRepository.Integrated;

public class SqlIntegratedInventoryTransferRepository : IntegratedInventoryTransferRepository
{
	public SqlIntegratedInventoryTransferRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
