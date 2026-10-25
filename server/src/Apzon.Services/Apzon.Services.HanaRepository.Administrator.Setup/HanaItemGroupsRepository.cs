using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaItemGroupsRepository : ItemGroupsRepository
{
	public HanaItemGroupsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetMaxCodeQuery()
	{
		return "SELECT IFNULL( MAX(\"ItmsGrpCod\" ),0) as \"ItmsGrpCod\"\r\n                                    FROM OITB ;";
	}

	protected override string DeleteDocQuery()
	{
		return " DELETE FROM OITB WHERE \"ItmsGrpCod\" = 0;\r\n            DELETE FROM OIGW WHERE \"ItmsGrpCod\" = 0;\r\n            UPDATE ONNM SET \"AutoKey\" = (SELECT MAX(\"ItmsGrpCod\") FROM OITB) +1 WHERE \"ObjectCode\" = '52';";
	}

	protected override string GetItemGroupWarehouseQuery()
	{
		return "SELECT \"WhsCode\", \"WhsName\" FROM owhs WHERE IFNULL(\"Locked\", 'N') = 'N' AND IFNULL(\"Nettable\", 'Y') = 'Y' AND IFNULL(\"BinActivat\", 'N') = 'Y';";
	}
}
