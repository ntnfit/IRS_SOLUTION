using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaImageAlbumRepository : ImageAlbumRepository
{
	public HanaImageAlbumRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
