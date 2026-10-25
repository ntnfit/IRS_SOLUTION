using Apzon.Services.Interfaces.Administrator.Setup.Inventory;

namespace Apzon.Services.Repository.Administrator.Setup.Inventory;

public class WHSubManagerRepository : BaseService<IWHSubManager>, IWHSubManager
{
	public WHSubManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
