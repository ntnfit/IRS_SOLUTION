using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.HanaRepository.Administrator.Setup.Inventory;

public class HanaAttrCodeSetupRepository : AttrCodeSetupRepository
{
	public HanaAttrCodeSetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
