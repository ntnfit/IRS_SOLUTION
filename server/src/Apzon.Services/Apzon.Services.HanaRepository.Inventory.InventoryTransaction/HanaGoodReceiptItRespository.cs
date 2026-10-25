using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.HanaRepository.Inventory.InventoryTransaction;

public class HanaGoodReceiptItRespository : GoodReceiptItRespository
{
	public HanaGoodReceiptItRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetBranchQuery(string requester)
	{
		string text = "SELECT A.\"Requester\", A.\"ReqName\", C.\"Name\", IFNULL(C.\"Code\", 0) AS \"Code\" \r\n                        FROM OIGE A\r\n                        LEFT OUTER JOIN OUBR C ON C.\"Code\" = A.\"Branch\"\r\n                        WHERE 1 = 1";
		if (!string.IsNullOrEmpty(requester))
		{
			text = text + " AND A.\"Requester\" ='" + requester + "'";
		}
		return text + ";";
	}

	protected override string GetInStockQuery(string itemCode)
	{
		string text = "SELECT IFNULL(A.\"OnHand\", 0) AS \"QuantityOnStock\" \r\n                            FROM OITM A\r\n                            LEFT OUTER JOIN IGE1 B ON A.\"ItemCode\" = B.\"ItemCode\"\r\n                                WHERE 1 = 1 ";
		if (!string.IsNullOrEmpty(itemCode))
		{
			text = text + " AND A.\"ItemCode\" ='" + itemCode + "'";
		}
		return text + ";";
	}

	protected override string GetMaxDocEntryQuery()
	{
		return "SELECT IFNULL(MAX(\"DocEntry\"), 0) AS \"DocEntry\" FROM OIGN;";
	}

	protected override string GetItemInStockAWarehouseQuery()
	{
		return "SELECT SUM(IFNULL(\"OnHand\", 0)) FROM OITW WHERE \"ItemCode\" = @itemcode AND \"WhsCode\" = @whscode;";
	}
}
