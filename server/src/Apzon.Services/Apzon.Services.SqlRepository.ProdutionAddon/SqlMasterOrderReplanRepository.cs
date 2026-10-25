using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.SqlRepository.ProdutionAddon;

public class SqlMasterOrderReplanRepository : MasterOrderReplanRepository
{
	public SqlMasterOrderReplanRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
