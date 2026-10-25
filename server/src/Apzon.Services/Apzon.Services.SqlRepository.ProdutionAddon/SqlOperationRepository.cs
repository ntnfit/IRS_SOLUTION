using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.SqlRepository.ProdutionAddon;

public class SqlOperationRepository : OperationRepository
{
	public SqlOperationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
