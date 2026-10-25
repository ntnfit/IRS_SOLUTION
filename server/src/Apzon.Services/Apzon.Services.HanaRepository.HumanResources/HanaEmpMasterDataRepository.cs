using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.HanaRepository.HumanResources;

public class HanaEmpMasterDataRepository : EmpMasterDataRepository
{
	public HanaEmpMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetTeamMembersQuery()
	{
		return "select \"teamID\",\"name\" from OHTM;";
	}

	protected override string ShowBankAccByCountryCodQuery()
	{
		return "select \"BankCode\" as \"Code\",\"BankName\" as \"Name\" from ODSC WHERE \"CountryCod\" = CountryCod;";
	}

	protected override string ExistFileInDocQuery()
	{
		return "select \"FileName\" from atc1 where \"AbsEntry\"=(select \"AtcEntry\" from ohem where \"empID\"=empID) \r\n\t                and \"FileExt\"=fileExt and \"FileName\"=fileName and \"AbsEntry\" is not null;";
	}

	protected override string GetListUserOfEmpQuery()
	{
		return "Select  string_agg(TO_NVARCHAR(\"userId\") || ';') FROM OHEM WHERE \"empID\" in (select data from split('1;2', ';'));";
	}

	protected override string SearchQuery()
	{
		return "";
	}

	protected override string GetOwnerShipQuery(string query)
	{
		return "declare usrId int = ifnull((select USERID from OUSR where USER_CODE = @userCode), -1);\r\n\t                          call apz_func_OwnerOverShip(@userCode, '171', ownerView);\r\n                            " + query + " and (\"UserSign\" = $usrId or ifnull(\"U_OwnerCode\", -1) = -1 or \"U_OwnerCode\" in (select distinct \"OwnerId\" from $ownerView))";
	}
}
