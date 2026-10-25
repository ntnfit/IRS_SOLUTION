using Apzon.Services.Repository.Service;

namespace Apzon.Services.HanaRepository.Service;

public class HanaServiceCallsRepository : ServiceCallsRepository
{
	public HanaServiceCallsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
