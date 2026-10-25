using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.SqlRepository.Inventory;

public class SqlOCYCRepository : OCYCRepository
{
	public SqlOCYCRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
