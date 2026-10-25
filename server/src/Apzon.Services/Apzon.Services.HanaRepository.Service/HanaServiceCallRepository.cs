using Apzon.Services.Repository.Service;

namespace Apzon.Services.HanaRepository.Service;

public class HanaServiceCallRepository : ServiceCallRepository
{
	public HanaServiceCallRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
