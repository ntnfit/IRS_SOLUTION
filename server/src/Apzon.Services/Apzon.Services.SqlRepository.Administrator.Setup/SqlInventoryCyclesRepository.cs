using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlInventoryCyclesRepository : InventoryCyclesRepository
{
	private const string Doctemp = "#tempdoc";

	public SqlInventoryCyclesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
