using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaChannelRepository : ChannelRepository
{
	public HanaChannelRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
