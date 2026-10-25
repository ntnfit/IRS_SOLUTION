using Apzon.Services.Repository.FacebookCRM;

namespace Apzon.Services.HanaRepository.FacebookCRM;

public class HanaFacebookCRMRepository : FacebookCRMRepository
{
	public HanaFacebookCRMRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
