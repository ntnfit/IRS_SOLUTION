using Apzon.Services.Repository.Service;

namespace Apzon.Services.HanaRepository.Service;

public class HanaServiceContractRepository : ServiceContractRepository
{
	public HanaServiceContractRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
