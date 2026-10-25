using Apzon.Services.Repository.PackingManagement;

namespace Apzon.Services.HanaRepository.PackingManagement;

public class HanaCreateTypeCaseRepository : CreateTypeCaseRepository
{
	public HanaCreateTypeCaseRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
