using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.HanaRepository.Inventory;

public class HanaSpecialBp_LineSPP1Respository : SpecialBp_LineSPP1Respository
{
	public HanaSpecialBp_LineSPP1Respository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string SearchLineSPP1Query(string itemCode, string cardCode)
	{
		string text = "SELECT DISTINCT a.\"ItemCode\", a.\"CardCode\", a.LINENUM, a.\"Price\", IFNULL(a.\"Currency\", 'VND') AS \"Currency\", a.\"Discount\", a.\"ListNum\", a.\"FromDate\", a.\"ToDate\", a.\"AutoUpdt\", a.\"Expand\" FROM SPP1 a WHERE 1 = 1 ";
		if (!string.IsNullOrEmpty(itemCode))
		{
			text = text + "AND a.\"ItemCode\" = '" + itemCode + "' ";
		}
		if (!string.IsNullOrEmpty(cardCode))
		{
			text = text + "AND a.\"CardCode\" = '" + cardCode + "' ";
		}
		return text + ";";
	}

	protected override string GetMaxLineNumQuery(string itemCode, string cardCode)
	{
		return "SELECT IFNULL(MAX(LINENUM), 0) AS \"LINENUM\" FROM SPP1 WHERE \"ItemCode\" = '" + itemCode + "'  AND  \"CardCode\"= '" + cardCode + "';";
	}
}
