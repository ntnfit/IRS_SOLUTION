using Apzon.Services.Repository.DMS;

namespace Apzon.Services.SqlRepository.DMS;

public class SqlUploadImageRepository : UploadImageRepository
{
	public SqlUploadImageRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
