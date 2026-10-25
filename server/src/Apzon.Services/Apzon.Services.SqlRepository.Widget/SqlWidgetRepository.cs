using Apzon.Services.Repository.Widget;

namespace Apzon.Services.SqlRepository.Widget;

public class SqlWidgetRepository : WidgetRepository
{
	public SqlWidgetRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
