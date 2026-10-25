using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaItemPropertiesRepository : ItemPropertiesRepository
{
	public HanaItemPropertiesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string LoadDocItemPropertiesQuery()
	{
		return "SELECT IFNULL(\"ItmsTypCod\", 0) AS \"ItmsTypCod\",\r\n                IFNULL(\"ItmsGrpNam\",'') AS \"ItmsGrpNam\",\r\n                IFNULL(\"UserSign\",1) AS \"UserSign\"\r\n            FROM oitg; ";
	}
}
