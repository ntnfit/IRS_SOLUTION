using Apzon.Services.Repository.Inventory.PriceLists;

namespace Apzon.Services.HanaRepository.Inventory.PriceLists;

public class HanaDiscountGroupRepository : DiscountGroupRepository
{
	public HanaDiscountGroupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string ExistsTableQuery()
	{
		return "\r\n                        DECLARE TableName VARCHAR(100);\r\n                        DECLARE isExists bigint ;\r\n                        select ifnull(TablePara,'') into TableName from dummy;\r\n                        SELECT COUNT(1)  into isExists FROM TABLE_COLUMNS WHERE TABLE_NAME = $TableName;\r\n                        select isExists from dummy;\r\n                  ";
	}

	protected override string GetMaxAbsEntryQuery()
	{
		return "SELECT ifnull(max(\"AbsEntry\"),0) FROM OEDG;";
	}

	protected override string GetAllBPEntryQuery()
	{
		return "select top 1 ifnull(\"AbsEntry\",0) FROM OEDG WHERE \"Type\"='A';";
	}
}
