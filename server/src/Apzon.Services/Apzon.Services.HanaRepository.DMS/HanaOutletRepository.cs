using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

internal class HanaOutletRepository : OutletRepository
{
	public HanaOutletRepository(IDatabaseService databaseService, HanaUnitOfWork unitOfWork)
		: base(databaseService, unitOfWork)
	{
	}
}
