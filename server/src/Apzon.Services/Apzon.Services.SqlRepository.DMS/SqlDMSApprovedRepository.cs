using Apzon.Services.Repository.DMS;

namespace Apzon.Services.SqlRepository.DMS;

public class SqlDMSApprovedRepository : DMSApprovedRepository
{
	public SqlDMSApprovedRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
