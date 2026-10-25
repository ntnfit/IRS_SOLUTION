using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

internal class HanaDMSSettingUDFRepository : DMSSettingUDFRepository
{
	public HanaDMSSettingUDFRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
