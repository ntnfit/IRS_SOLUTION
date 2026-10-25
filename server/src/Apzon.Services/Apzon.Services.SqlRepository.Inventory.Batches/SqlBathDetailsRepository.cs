using Apzon.Services.Repository.Inventory.Batches;

namespace Apzon.Services.SqlRepository.Inventory.Batches;

public class SqlBathDetailsRepository : BatchDetailsRepository
{
	public SqlBathDetailsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
