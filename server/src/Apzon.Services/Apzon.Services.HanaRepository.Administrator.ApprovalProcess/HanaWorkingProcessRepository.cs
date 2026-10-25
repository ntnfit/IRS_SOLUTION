using Apzon.Services.Repository.Administrator.ApprovalProcess;

namespace Apzon.Services.HanaRepository.Administrator.ApprovalProcess;

public class HanaWorkingProcessRepository : WorkingProcessRepository
{
	public HanaWorkingProcessRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
