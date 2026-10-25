using Apzon.Services.Repository.Administrator.Setup.Inventory;

namespace Apzon.Services.SqlRepository.Administrator.Setup.Inventory;

internal class SqlBinFieldActivationRepository : BinFieldActivationRepository
{
	public SqlBinFieldActivationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
