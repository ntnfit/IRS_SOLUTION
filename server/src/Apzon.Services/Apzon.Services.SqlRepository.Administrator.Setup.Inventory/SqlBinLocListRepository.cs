using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.SqlRepository.Administrator.Setup.Inventory;

internal class SqlBinLocListRepository : BinLocListRepository
{
	public SqlBinLocListRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
