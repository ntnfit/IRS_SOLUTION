using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaPriceListsRepository : PriceListsRepository
{
	public HanaPriceListsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string ShowPriceListQuery()
	{
		return "select \"ListNum\" as \"PriceListNo\" ,\"ListName\" as \"PriceListName\",\"UserSign\" from  OPLN;";
	}

	protected override string GetMaxCodeQuery()
	{
		return "SELECT IFNULL(MAX(\"ListNum\"), 0) AS \"PriceListNo\" FROM OPLN;";
	}

	protected override string GetPriceListQuery()
	{
		return "SELECT a.\"ItemCode\", b.\"ListNum\" AS \"PriceListNo\", IFNULL(b.\"ListName\", '') AS \"PriceListName\", IFNULL(a.\"Price\", 0) AS \"Price\" , IFNULL(b.\"IsGrossPrc\", '') AS \"IsGrossPrc\", IFNULL(a.\"Currency\", '') AS \"Currency\" FROM ITM1 a LEFT OUTER JOIN OPLN b ON a.\"PriceList\" = b.\"ListNum\"\r\n                 where  1= 1  ";
	}
}
