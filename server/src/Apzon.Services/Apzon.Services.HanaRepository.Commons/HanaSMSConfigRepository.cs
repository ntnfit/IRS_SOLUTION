using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaSMSConfigRepository : SMSConfigRepository
{
	public HanaSMSConfigRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
