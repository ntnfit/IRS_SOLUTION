using Apzon.Services.Repository.PackingManagement;

namespace Apzon.Services.HanaRepository.PackingManagement;

public class HanaPrintBillWrapRepository : PrintBillWrapRepository
{
	public HanaPrintBillWrapRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
