using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaQuestionMasterDataRepository : QuestionMasterDataRepository
{
	public HanaQuestionMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetNoQuery()
	{
		return "select IFNULL(MAX(\"DocEntry\"), 0) + 1 from APZ_D_OCHM;";
	}
}
