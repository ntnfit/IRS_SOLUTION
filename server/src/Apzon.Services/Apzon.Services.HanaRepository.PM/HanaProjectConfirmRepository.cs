using Apzon.Services.Repository.PM;

namespace Apzon.Services.HanaRepository.PM;

public class HanaProjectConfirmRepository : ProjectConfirmRepository
{
	public HanaProjectConfirmRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
