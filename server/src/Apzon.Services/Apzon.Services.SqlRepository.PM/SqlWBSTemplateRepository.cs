using Apzon.Services.Repository.PM;

namespace Apzon.Services.SqlRepository.PM;

public class SqlWBSTemplateRepository : WBSTemplateRepository
{
	public SqlWBSTemplateRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
