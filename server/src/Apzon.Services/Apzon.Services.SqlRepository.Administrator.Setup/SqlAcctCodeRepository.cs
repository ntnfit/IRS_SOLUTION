using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlAcctCodeRepository : AcctCodeRepository
{
	public SqlAcctCodeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
