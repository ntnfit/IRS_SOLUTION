using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaWarehouseRepository : WarehouseRepository
{
	public HanaWarehouseRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string CflQuery()
	{
		return "SELECT *\r\n                    FROM OWHS\r\n                    WHERE(IFNULL(\"Nettable\", 'Y') = 'Y' OR IFNULL(\"DropShip\", 'N') = 'Y')\r\n                          AND IFNULL(\"Inactive\", 'N') = 'N'\r\n                          AND IFNULL(\"Locked\", 'N') = 'N'; ";
	}

	protected override string ShowWarehouseQuery()
	{
		return "select \"WhsCode\",\"WhsName\" FROM OWHS; ";
	}

	protected override string CflDocumentQuery()
	{
		return "SELECT * FROM OWHS WHERE (IFNULL(\"Nettable\", 'Y') = 'Y' OR IFNULL(\"DropShip\", 'N') = 'Y') AND IFNULL(\"Inactive\", 'N') = 'N' AND IFNULL(\"Locked\", 'N') = 'N';";
	}
}
