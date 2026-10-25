using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.SqlRepository.ProdutionAddon;

public class SqlApzBillOfMaterialsRepository : ApzBillOfMaterialsRepository
{
	public SqlApzBillOfMaterialsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
