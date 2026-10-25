using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaSalesEmployeeBuyerRepository : SalesEmployeeBuyerRepository
{
	public HanaSalesEmployeeBuyerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetDefaultCodeQuery()
	{
		return "Select IFNULL(\"DfltSlp\",0) From OADM;";
	}

	protected override string GetOSLPQuery()
	{
		return "SELECT a.*, b.\"empID\" AS \"empID1\"\r\n\t                , CASE WHEN b.\"lastName\" IS NULL OR LTRIM(RTRIM(b.\"lastName\")) = '' THEN IFNULL(b.\"firstName\", '')\r\n\r\n                    ELSE CASE WHEN b.\"firstName\" IS NULL OR LTRIM(RTRIM(b.\"firstName\")) = '' THEN IFNULL(b.\"firstName\", '') ELSE IFNULL(b.\"lastName\", '') || ', ' || IFNULL(b.\"firstName\", '') END END AS \"empID2\"\r\n                FROM OSLP a LEFT OUTER JOIN OHEM b ON a.\"SlpCode\" = b.\"salesPrson\" AND a.\"SlpCode\" <> -1; ";
	}
}
