using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaSaleTerritoryRepository : SaleTerritoryRepository
{
	public HanaSaleTerritoryRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
