using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.HanaRepository.HumanResources;

public class HanaEmpRelativesRepository : EmpRelativesRepository
{
	public HanaEmpRelativesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetAllRecordsQuery()
	{
		return "SELECT * FROM APZ_OHER WHERE \"empID\" = empId;";
	}

	protected override string ExistFileInDocQuery()
	{
		return "select \"FileName\" from atc1 where \"AbsEntry\"=(select \"AtcEntry\" from apz_oher where \"relaID\" = relaID and \"empID\"=empID) \r\n\t                and \"FileExt\"=fileExt and \"FileName\"=fileName and \"AbsEntry\" is not null;";
	}

	protected override string GetNextIdQuery()
	{
		return "select max(\"relaID\") from apz_oher where \"empID\" = ";
	}
}
