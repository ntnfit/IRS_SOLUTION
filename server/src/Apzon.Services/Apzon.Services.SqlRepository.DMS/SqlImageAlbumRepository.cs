using Apzon.Services.Repository.DMS;

namespace Apzon.Services.SqlRepository.DMS;

public class SqlImageAlbumRepository : ImageAlbumRepository
{
	public SqlImageAlbumRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
