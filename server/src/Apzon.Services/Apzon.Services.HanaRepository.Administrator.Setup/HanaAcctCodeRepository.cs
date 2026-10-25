using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaAcctCodeRepository : AcctCodeRepository
{
	public HanaAcctCodeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
