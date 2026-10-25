using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaWeightRepository : WeightRepository
{
	public HanaWeightRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetMaxQuery()
	{
		return "SELECT MAX(ifnull(\"UnitCode\",0)) as \"UnitCode\" FROM  OWGT;";
	}
}
