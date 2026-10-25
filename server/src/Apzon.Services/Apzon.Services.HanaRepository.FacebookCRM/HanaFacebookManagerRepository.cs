using Apzon.Services.Repository.FacebookCRM;

namespace Apzon.Services.HanaRepository.FacebookCRM;

public class HanaFacebookManagerRepository : FacebookManagerRepository
{
	public HanaFacebookManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
