using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaUploadImageRepository : UploadImageRepository
{
	public HanaUploadImageRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
