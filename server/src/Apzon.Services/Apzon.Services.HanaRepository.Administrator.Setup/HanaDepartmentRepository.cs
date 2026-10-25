using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaDepartmentRepository : DepartmentRepository
{
	public HanaDepartmentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string ShowDepartmentQuery()
	{
		return "select \"Code\",\"Name\",IFNULL( \"Remarks\",'') as \"Remarks\" from OUDP;";
	}
}
