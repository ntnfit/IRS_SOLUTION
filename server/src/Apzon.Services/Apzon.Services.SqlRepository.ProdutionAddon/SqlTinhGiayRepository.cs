using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.SqlRepository.ProdutionAddon;

public class SqlTinhGiayRepository : TinhGiayRepository
{
	public SqlTinhGiayRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
