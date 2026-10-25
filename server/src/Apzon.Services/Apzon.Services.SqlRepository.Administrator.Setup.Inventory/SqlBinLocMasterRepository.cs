using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.SqlRepository.Administrator.Setup.Inventory;

internal class SqlBinLocMasterRepository : BinLocMasterRepository
{
	public SqlBinLocMasterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
