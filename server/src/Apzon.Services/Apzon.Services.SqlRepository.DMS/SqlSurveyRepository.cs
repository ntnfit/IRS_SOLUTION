using Apzon.Services.Repository.DMS;

namespace Apzon.Services.SqlRepository.DMS;

public class SqlSurveyRepository : SurveyRepository
{
	public SqlSurveyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
