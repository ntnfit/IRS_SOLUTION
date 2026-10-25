using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.HanaRepository.HumanResources.Salary;

public class HanaLeaveParameterRepository : LeaveParameterRepository
{
	public HanaLeaveParameterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
