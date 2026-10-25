using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.HanaRepository.HumanResources;

public class HanaStatesRepository : StatesRepository
{
	public HanaStatesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetNewRolesQuery()
	{
		return "select \"typeID\", \"name\" ,\"descriptio\",\"locked\"  From OHTY;";
	}

	protected override string RemoveRolesQuery()
	{
		return "DELETE  FROM OHTY WHERE \"typeID\" = typeID and typeID NOT IN( SELECT \"roleID\" FROM HEM6);";
	}

	protected override string GetBranchsQuery()
	{
		return "SELECT \"Code\", \"Name\" FROM OUBR;";
	}

	protected override string GetCountryCodeQuery()
	{
		return "SELECT \"Code\", \"Name\" FROM OCRY;";
	}

	protected override string RemoveTeamSetupQuery()
	{
		return "DELETE  FROM OHTM WHERE \"teamID\" = teamID and teamID NOT IN( SELECT \"teamID\" FROM HTM1);";
	}

	protected override string RemoveEmpPosQuery()
	{
		return "DELETE  FROM OHPS WHERE \"posID\" = PosID;";
	}

	protected override string RemoveEmpStatusQuery()
	{
		return "DELETE  FROM OHST WHERE \"statusID\" = statusID;";
	}

	protected override string RemoveTerminationReasonsQuery()
	{
		return "DELETE  FROM OHTR WHERE \"reasonID\" = reasonID;";
	}

	protected override string RemoveDepartmentQuery()
	{
		return "DELETE  FROM OUDP WHERE \"Code\" = code;";
	}

	protected override string RemoveBranchesQuery()
	{
		return "Delete from OUBR Where \"Code\" = code;";
	}

	protected override string RemoveStatesQuery()
	{
		return "Delete from OCST where \"Code\" = code;";
	}

	protected override string LoadStatesQuery()
	{
		return "select * from OCST where \"Country\" = Country;";
	}
}
