using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaContactUsRepository : ContactUsRepository
{
	public HanaContactUsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
