using Apzon.Services.Repository.PM;

namespace Apzon.Services.SqlRepository.PM;

public class SqlProjectConfirmRepository : ProjectConfirmRepository
{
	public SqlProjectConfirmRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
