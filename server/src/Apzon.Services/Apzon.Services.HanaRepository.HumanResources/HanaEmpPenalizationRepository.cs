using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.HanaRepository.HumanResources;

public class HanaEmpPenalizationRepository : EmpPenalizationRepository
{
	public HanaEmpPenalizationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
