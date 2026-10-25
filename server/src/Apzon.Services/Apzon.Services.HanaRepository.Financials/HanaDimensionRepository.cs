using Apzon.Services.Repository.Financials;

namespace Apzon.Services.HanaRepository.Financials;

public class HanaDimensionRepository : DimensionRepository
{
	public HanaDimensionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
