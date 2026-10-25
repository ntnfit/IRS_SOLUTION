using Apzon.Services.Repository.DashBoard;

namespace Apzon.Services.SqlRepository.DashBoard;

public class SqlCategoriesDABRepository : CategoriesDABRepository
{
	public SqlCategoriesDABRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
