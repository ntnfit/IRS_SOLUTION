using Apzon.Services.Repository.PM;

namespace Apzon.Services.HanaRepository.PM;

public class HanaWBSTemplateRepository : WBSTemplateRepository
{
	public HanaWBSTemplateRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
