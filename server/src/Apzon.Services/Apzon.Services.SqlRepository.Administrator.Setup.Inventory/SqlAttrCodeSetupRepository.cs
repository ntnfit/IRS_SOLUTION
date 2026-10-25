using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.SqlRepository.Administrator.Setup.Inventory;

internal class SqlAttrCodeSetupRepository : AttrCodeSetupRepository
{
	public SqlAttrCodeSetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
