using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaSurveyRepository : SurveyRepository
{
	public HanaSurveyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
