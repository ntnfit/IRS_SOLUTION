using Apzon.Services.Repository.Production;

namespace Apzon.Services.HanaRepository.Production;

public class HanaBillOfMasterialsRepository : BillOfMasterialsRepository
{
	public HanaBillOfMasterialsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetItemNameQuery()
	{
		return "select * from oitm Where  \"ItemCode\" = itemCode;";
	}

	protected override string GetItemCFLQuery()
	{
		return "select \"ItemCode\", \"ItemName\", \"OnHand\" from OITM where \"validFor\"='Y' AND \"InvntItem\"='Y' and \"ItemCode\" not in(select \"Code\" from oitt);";
	}

	protected override string DeleteDocQuery()
	{
		return "DELETE FROM ITT1 WHERE \"Father\" = code;\r\n                                        delete from oitt where \"Code\"=code;";
	}

	protected override string SearchQuery()
	{
		return "a.\"{0}\" LIKE N'%'||@{0}||'%' ";
	}

	protected override string SearchLineQuery()
	{
		return "SELECT ifnull(\"Father\",'') as \"Father\",ifnull(\"ChildNum\",0) as \"ChildNum\" ,ifnull(\"Code\",'') as \"Code\" \r\n                                    ,ifnull(\"Quantity\",0) as \"Quantity\" ,ifnull(\"Warehouse\",'') as \"Warehouse\" ,ifnull(\"Price\",0) as \"Price\" \r\n                                    ,ifnull(\"Currency\",'') as \"Currency\" ,ifnull(\"PriceList\",0) as \"PriceList\" ,ifnull(\"OrigPrice\",0) as \"OrigPrice\" ,ifnull(\"OrigCurr\",'') as \"OrigCurr\" ,ifnull(\"IssueMthd\",'') as \"IssueMthd\"\r\n                                    ,ifnull(\"Uom\",'') as \"Uom\" ,ifnull(\"Comment\",'') as \"Comment\" ,ifnull(\"LogInstanc\",0) as \"LogInstanc\" ,ifnull(\"Object\",'') as \"Object\" \r\n                                    ,ifnull(\"OcrCode\",'') as \"OcrCode\" ,ifnull(\"OcrCode2\",'') as \"OcrCode2\" ,ifnull(\"OcrCode3\",'') as \"OcrCode3\" ,ifnull(\"OcrCode4\",'') as \"OcrCode4\" \r\n                                    ,ifnull(\"OcrCode5\",'') as \"OcrCode5\" ,ifnull(\"PrncpInput\",'') as \"PrncpInput\" ,ifnull(\"Project\",'') as \"Project\" ,ifnull(\"Type\",0) as \"Type\" \r\n                                    ,ifnull(\"WipActCode\",'') as \"WipActCode\" ,ifnull(\"AddQuantit\",0) as \"AddQuantit\" ,ifnull(\"LineText\",'') as \"LineText\"\r\n                                 FROM ITT1 a WHERE a.\"Father\" = father;";
	}

	protected override string CheckValidItemTypeQuery()
	{
		return "select \r\n                                    ifnull(\"PrchseItem\",'N') \"PrchseItem\"\r\n                                    ,ifnull(\"SellItem\",'N') \"SellItem\"\r\n                                    ,ifnull(\"InvntItem\",'N') \"InvntItem\"\r\n                                from OITM where \"ItemCode\"=Code;";
	}

	protected override string GetActiveDimensionCountQuery()
	{
		return "SELECT Count(*) FROM ODIM WHERE  \"DimActive\"='Y';";
	}

	protected override string GetItemDetailsIQuery()
	{
		return "SELECT \r\n                                    ifnull(\"ItemName\",'') \"ItemName\"\r\n\t                                ,ifnull(\"InvntryUom\",'') \"InvntryUom\"\r\n\t                                ,ifnull(\"OnHand\",0) \"OnHand\"\r\n\t                                ,ifnull(\"OnOrder\",0) \"OnOrder\"\r\n\t                                ,ifnull(\"DfltWH\",'') \"DefaultWarehouse\"\r\n\t                                ,ifnull(\"IssueMthd\",'') \"IssueMthd\"\r\n\t                             FROM OITM where \"ItemCode\"=Code;";
	}

	protected override string GetItemDetailsRQuery()
	{
		return "SELECT \r\n                                    ifnull(\"ResName\",'') \"ResName\"\r\n\t                                ,ifnull(\"UnitOfMsr\",'') \"InvntryUom\"\r\n\t                                 --,ifnull(\"OnHand\",0) \"OnHand\"\r\n\t                                --,ifnull(\"OnOrder\",0) \"OnOrder\"\r\n\t                                ,ifnull(\"DfltWH\",'') \"DefaultWarehouse\"\r\n\t                                ,ifnull(\"IssueMthd\",'') \"IssueMthd\"\r\n\t                             FROM ORSC where \"ResCode\"=Code;";
	}

	protected override string IsValidCodeItemQuery()
	{
		return "SELECT 1 FROM OITM where \"ItemCode\"=Code and\r\n(\"validFor\"='Y' and \"frozenFor\"='N' and current_date between ifnull(\"validFrom\", current_date) and ifnull(\"validTo\", current_date) or\r\n\"validFor\"='N' and \"frozenFor\"='Y' and current_date not between ifnull(\"frozenFrom\", current_date) and ifnull(\"frozenTo\", current_date) or\r\n\"validFor\"='Y' and \"frozenFor\"='Y' and current_date between ifnull(\"validFrom\", current_date) and ifnull(\"validTo\", current_date)\r\nand current_date not between ifnull(\"frozenFrom\", current_date) and ifnull(\"frozenTo\", current_date));";
	}

	protected override string IsValidCodeResourceQuery()
	{
		return "SELECT 1 FROM ORSC where \"ResCode\"=Code;";
	}

	protected override string IsABOMQuery()
	{
		return "SELECT 1 FROM OITT where \"Code\"=Code;";
	}

	protected override string CheckDocQuery()
	{
		return "SELECT a.\"Code\" FROM OITT a WHERE a.\"Code\" = code;";
	}

	protected override string UpdateItemPriceQuery()
	{
		return "Update ITM1 Set \"Price\"=price Where \"ItemCode\"=itemCode AND \"PriceList\"=priceList;";
	}

	protected override string GetList_BillOfMsterQuery()
	{
		return "select a.*, b.\"ItemName\" from oitt a left join oitm b on a.\"Code\" = b.\"ItemCode\" ;";
	}
}
