using Apzon.Services.Repository.DMS;

namespace Apzon.Services.SqlRepository.DMS;

public class SqlChannelRepository : ChannelRepository
{
	public SqlChannelRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
