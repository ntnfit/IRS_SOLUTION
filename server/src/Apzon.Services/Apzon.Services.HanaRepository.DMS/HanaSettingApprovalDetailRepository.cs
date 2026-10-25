using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaSettingApprovalDetailRepository : SettingApprovalDetailRepository
{
	public HanaSettingApprovalDetailRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
