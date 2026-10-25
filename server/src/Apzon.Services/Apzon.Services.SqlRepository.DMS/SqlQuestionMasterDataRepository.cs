using Apzon.Services.Repository.DMS;

namespace Apzon.Services.SqlRepository.DMS;

public class SqlQuestionMasterDataRepository : QuestionMasterDataRepository
{
	public SqlQuestionMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
