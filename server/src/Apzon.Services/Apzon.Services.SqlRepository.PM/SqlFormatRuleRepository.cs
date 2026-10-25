using Apzon.Services.Repository.PM;

namespace Apzon.Services.SqlRepository.PM;

public class SqlFormatRuleRepository : FormatRuleRepository
{
	public SqlFormatRuleRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
