using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.SqlRepository.HumanResources.Salary;

public class SqlLeaveParameterRepository : LeaveParameterRepository
{
	public SqlLeaveParameterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
