using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaCFLRepository : CFLRepository
{
	public HanaCFLRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetAllOcrByDistRuleQuery()
	{
		return "SELECT \"DimCode\", \"DimDesc\", '' \"OcrCode\" FROM ODIM WHERE  \"DimActive\"='Y' order by \"DimCode\";";
	}
}
