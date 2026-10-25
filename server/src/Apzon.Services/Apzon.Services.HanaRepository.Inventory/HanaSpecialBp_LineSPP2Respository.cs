using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.HanaRepository.Inventory;

public class HanaSpecialBp_LineSPP2Respository : SpecialBp_LineSPP2Respository
{
	public HanaSpecialBp_LineSPP2Respository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetMaxSPP2LNumQuery(string itemCode, string cardCode, int lineNum)
	{
		return "SELECT IFNULL(MAX(\"SPP2LNum\"), 0) AS \"SPP2LNum\"\r\n                            FROM SPP2 WHERE \"ItemCode\" = '" + itemCode + "'  AND \"CardCode\"= '" + cardCode + "' AND  \"SPP1LNum\" = '" + lineNum + "' ;";
	}

	protected override string SearchLineSPP2Query(string itemCode, string cardCode, int SPP1LineNum)
	{
		string text = "SELECT DISTINCT a.\"ItemCode\", a.\"CardCode\", a.\"SPP1LNum\" AS \"LINENUM\", a.\"SPP2LNum\", a.\"Amount\", a.\"Discount\", a.\"Price\", IFNULL(a.\"Currency\", 'VND') AS \"Currency\" FROM SPP2 a WHERE 1 = 1 ";
		if (!string.IsNullOrEmpty(itemCode))
		{
			text = text + "AND a.\"ItemCode\" = '" + itemCode + "' ";
		}
		if (!string.IsNullOrEmpty(cardCode))
		{
			text = text + "AND a.\"CardCode\" = '" + cardCode + "' ";
		}
		if (SPP1LineNum != -1)
		{
			text = text + "AND a.\"SPP1LNum\" = '" + SPP1LineNum + "' ";
		}
		return text + ";";
	}
}
