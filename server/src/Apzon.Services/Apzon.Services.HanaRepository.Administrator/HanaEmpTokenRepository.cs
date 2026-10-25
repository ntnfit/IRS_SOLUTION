using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaEmpTokenRepository : EmpTokenRepository
{
	public HanaEmpTokenRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
