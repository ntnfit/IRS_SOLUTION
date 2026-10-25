using Apzon.Services.Repository.DashBoard;

namespace Apzon.Services.HanaRepository.DashBoard;

public class HanaCategoriesDABRepository : CategoriesDABRepository
{
	public HanaCategoriesDABRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetXMLByDasIdQuery()
	{
		return "select \"Layout\", \"LayoutDefault\" from APZ_DAB1 where \"Id\" = @dasId and (\"UserSign\" = @userId OR IFNULL(\"UserSign\", 0) = 0);";
	}

	protected override string GetUpdateCaptionQuery()
	{
		return "Declare dashboardId int = 0;\r\n                     SELECT (SELECT IFNULL(MAX(\"Id\"), 0) + 1 FROM APZ_DAB1) into dashboardId from DUMMY;\r\n                     INSERT INTO APZ_ODAB(\"Id\", \"Name\", \"Description\", \"Valid\") VALUES(@Id, @Name, @Name, 1);\r\n                     INSERT INTO APZ_DAB1(\"Id\", \"CatId\", \"ValidFor\") VALUES (dashboardId, @Id, 1);";
	}
}
