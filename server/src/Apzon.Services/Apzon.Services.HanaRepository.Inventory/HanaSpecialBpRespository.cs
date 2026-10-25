using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.HanaRepository.Inventory;

public class HanaSpecialBpRespository : SpecialBpRespository
{
	public HanaSpecialBpRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetItemByItemCodeQuery()
	{
		return "SELECT \"ItemCode\", \"ItemName\", \r\n                        (SELECT \"Price\" FROM ITM1 WHERE \"ItemCode\" = a.\"ItemCode\" AND \"PriceList\" = @ListNum) AS \"Price\", 0 AS \"Discount\", 1 AS \"ListNum\", 'Y' AS \"AutoUpdt\", \r\n\t\t                (SELECT IFNULL(\"PrimCurr\", '') FROM OPLN WHERE \"ListNum\" = @ListNum) AS \"Currency\", 'Y' AS \"EXPAND\", \r\n\t\t                (SELECT \"Price\" FROM ITM1 WHERE \"ItemCode\" = a.\"ItemCode\" AND \"PriceList\" = @ListNum) AS \"PrimPrice\", \r\n\t\t                (SELECT \"AddPrice1\" FROM ITM1 WHERE \"ItemCode\" = a.\"ItemCode\" AND \"PriceList\" = @ListNum) AS \"AddPrice1\", \r\n\t\t                (SELECT \"AddPrice2\" FROM ITM1 WHERE \"ItemCode\" = a.\"ItemCode\" AND \"PriceList\" = @ListNum) AS \"AddPrice2\", \r\n\t\t                (SELECT \"ItmsGrpNam\" FROM OITB WHERE a.\"ItmsGrpCod\" = OITB.\"ItmsGrpCod\") AS \"ItmsGrpCod\", \r\n\t\t                (SELECT \"UomName\" FROM OUOM WHERE a.\"IUoMEntry\" = OUOM.\"UomEntry\") AS \"UoMName\"\r\n                    FROM OITM a\r\n                    WHERE a.\"ItemCode\" = @ItemCode; ";
	}
}
