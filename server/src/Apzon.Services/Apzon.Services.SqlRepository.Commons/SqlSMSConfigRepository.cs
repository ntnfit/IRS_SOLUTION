using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

internal class SqlSMSConfigRepository : SMSConfigRepository
{
	public SqlSMSConfigRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
